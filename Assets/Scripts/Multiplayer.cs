using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Runtime.InteropServices;
using WebSocketSharp;


public class Multiplayer : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void WebSocketInit(string url);

    [DllImport("__Internal")]
    private static extern void WebSocketSend(string message);

    [DllImport("__Internal")]
    private static extern int State();
#endif

#if !UNITY_WEBGL || UNITY_EDITOR
    private WebSocket ws;
#endif

    public static Multiplayer instance = null;
    public Player player;
    public bool connected = false;
    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    public int id;

    private List<Message> messageQueue = new List<Message>();
    private bool processMessages = true;

    private void OnEnable()
    {
        if (instance == null)
            instance = this;

        this.player.isMenu = false;
        this.connected = false;

#if UNITY_WEBGL && !UNITY_EDITOR
        WebSocketInit("ws://192.168.1.200:8000");
#endif

#if !UNITY_WEBGL || UNITY_EDITOR
        ws = new WebSocket("ws://192.168.1.200:8000");

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("Connected");
            this.connected = true;
        };

        ws.OnError += (sender, e) =>
        {
            Debug.Log("Error: " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            // ws.Send("I'm leaving");
        };

        ws.OnMessage += (sender, e) =>
        {
            this.OnMessage(e.Data);
        };

        ws.Connect();
#endif

        StartCoroutine(ProcessMessages());
    }

    private void OnApplicationQuit()
    {
        Message message = new Message();
        message.message = "playerdisconnect";
        message.id = this.id;
    }

    void FixedUpdate()
    {
        Message message = new Message();
        message.message = "playerupdate";
        message.id = this.id;
        message.animSpeed = player.animSpeed;
        message.position = player.rigid.transform.position;
        message.rotation = player.rigid.transform.rotation;
        string json = JsonUtility.ToJson(message);

        this.Send(json);
    }

    public void Send(string data)
    {
        if (!this.connected)
            return;

#if UNITY_WEBGL && !UNITY_EDITOR
        WebSocketSend(data);
#else
        ws.Send(data);
#endif
    }

    public IEnumerator ProcessMessages()
    {
        while (processMessages)
        {
            for (int i = 0; i < messageQueue.Count; i++)
            {
                if (messageQueue[i].message == "playerspawn")
                {
                    if (players.ContainsKey(messageQueue[i].id) || messageQueue[i].id == this.id)
                        break;

                    GameObject newPlayer = (GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/Player"));
                    Player playerScript = newPlayer.GetComponent<Player>();
                    playerScript.rigid.position = messageQueue[i].position;
                    playerScript.rigid.rotation = messageQueue[i].rotation;
                    playerScript.animSpeed = messageQueue[i].animSpeed;
                    playerScript.animator.SetFloat("speed", playerScript.animSpeed);
                    playerScript.isControlled = false;

                    players.Add(messageQueue[i].id, playerScript);
                }
                else
                if (messageQueue[i].message == "playerupdate")
                {
                    if (messageQueue[i].id == this.id)
                        break;

                    Player playerScript = players[messageQueue[i].id].GetComponent<Player>();

                    playerScript.animSpeed = messageQueue[i].animSpeed;
                    playerScript.animator.SetFloat("speed", playerScript.animSpeed);
                    players[messageQueue[i].id].rigid.MovePosition(Vector3.Lerp(playerScript.rigid.position, messageQueue[i].position, 0.25f));
                    players[messageQueue[i].id].rigid.rotation = messageQueue[i].rotation;
                }
                else
                if (messageQueue[i].message == "playerdisconnect")
                {
                    if (players.ContainsKey(messageQueue[i].id))
                        Destroy(players[messageQueue[i].id]);
                }
                else
                if (messageQueue[i].message == "roundfired" && messageQueue[i].shooterid != this.id)
                {
                    Message message = messageQueue[i];
                    this.FireRound(message.shooterid, message.roundLifeTime, message.damagePerRound, message.position, message.rotation, message.force, false);
                }
            }

            messageQueue.Clear();
            yield return null;
        }
    }

    public void OnConnect()
    {
        this.connected = true;
    }

    public void OnMessage(string message)
    {
        Message msg = (Message)JsonUtility.FromJson(message, typeof(Message));

        if (msg.id == this.id)
            return;

        if (msg.message == "playerupdate")
        {
            if (players.ContainsKey(msg.id))
            {
                msg.message = "playerupdate";
                messageQueue.Add(msg);
            }
            else
            {
                msg.message = "playerspawn";
                messageQueue.Add(msg);
            }
        }
        else
        if (msg.message == "newid")
        {
            this.id = msg.id;
            this.player.id = msg.id;
        }
        else
        if (msg.message == "playerdisconnect")
        {
            messageQueue.Add(msg);
        }
        else
        if (msg.message == "killplayer")
        {
            players[msg.targetid].health = 0;
        }
        else
        if (msg.message == "roundfired" && msg.shooterid != this.id)
            messageQueue.Add(msg);
    }

    public void FireRound(int playerId, float roundLifetime, int damagePerRound, Vector3 position, Quaternion rotation, float force, bool send)
    {
        Projectile projectileScript = AssetManager.instance.GetProjectile();
        projectileScript.roundLifetime = roundLifetime;
        projectileScript.damageDone = damagePerRound;
        projectileScript.playerId = playerId;
        projectileScript.gameObject.transform.position = position;
        projectileScript.gameObject.transform.rotation = rotation;
        projectileScript.gameObject.SetActive(true);
        projectileScript.rigid.AddForce(projectileScript.rigid.transform.forward * force, ForceMode.Impulse);

        if (send)
        {
            Message message = new Message();

            message.message = "roundfired";
            message.damagePerRound = damagePerRound;
            message.roundLifeTime = roundLifetime;
            message.force = force;
            message.position = position;
            message.rotation = rotation;
            message.shooterid = this.id;

            this.Send(JsonUtility.ToJson(message));
        }
    }
}
