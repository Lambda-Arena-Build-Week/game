using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using WebSocketSharp;
using System;

public class Multiplayer : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void WebSocketInit(string url);

    [DllImport("__Internal")]
    private static extern void WebSocketSend(string message);

    [DllImport("__Internal")]
    private static extern int State();

    [DllImport("__Internal")]
    private static extern void SetMapPosX(int message);

    [DllImport("__Internal")]
    private static extern void SetMapPosY(int message);
#endif

#if !UNITY_WEBGL || UNITY_EDITOR
    private WebSocket ws;
#endif

    public static Multiplayer instance = null;
    public Player player;
    public bool connected = false;
    public Dictionary<int, Player> players = new Dictionary<int, Player>();
    public int id;
    public bool connectToGame = false;
    public string currentWeapon = "handgun";
    public AudioSource audioSource;
    public List<AudioClip> audioClips = new List<AudioClip>();
    private List<Message> messageQueue = new List<Message>();
    private Vector2 currentMapPos = new Vector2(10000.0f, 10000.0f);

    private void Start()
    {
        if (instance == null)
            instance = this;

        this.player.isMenu = false;
        this.connected = false;

        StartCoroutine(this.GetStage());     
    }

    private IEnumerator GetStage()
    {
        using (UnityWebRequest request = UnityWebRequest.Get("https://lambdamud-2020-staging.herokuapp.com/api/gameworld/"))
        {
            yield return request.SendWebRequest();

            while (!request.isDone)
                yield return null;

            byte[] result = request.downloadHandler.data;

            string json = System.Text.Encoding.Default.GetString(result);
            Dungeon.instance.stage = JsonHelper.getJsonArray<StagePiece>(json);
             
            Dungeon.instance.BuildDungeon();
            this.CreateWebsocket();
        }
    }

    private void CreateWebsocket()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        WebSocketInit("wss://bwgame-node-be.herokuapp.com");
#endif

#if !UNITY_WEBGL || UNITY_EDITOR
        ws = new WebSocket("wss://bwgame-node-be.herokuapp.com");

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

        };

        ws.OnMessage += (sender, e) =>
        {
            this.OnMessage(e.Data);
        };

        ws.Connect();
#endif
    }


        private void OnApplicationQuit()
    {
        Message message = new Message();
        message.message = "playerdisconnect";
        message.id = this.id;
    }

    void FixedUpdate()
    {
        this.ProcessMessages();

        Message message = new Message();

        message.message = "playerupdate";
        message.id = this.id;
        message.animSpeed = player.animSpeed;
        message.position = player.rigid.transform.position;
        message.rotation = player.rigid.transform.rotation;
        message.pantsColor = player.pantsColor;
        message.hairColor = player.hairColor;
        message.shirtColor = player.shirtColor;
        message.shoesColor = player.shoesColor;
        message.skinColor = player.skinColor;
        message.weapon = this.currentWeapon;
        string json = JsonUtility.ToJson(message);

        this.Send(json);


#if UNITY_WEBGL && !UNITY_EDITOR
        Vector2 pos =  new Vector2(Mathf.CeilToInt((player.transform.position.x / 9.0f) - 0.5f), Mathf.CeilToInt((player.transform.position.z / 9.0f) - 0.5f));
        if (currentMapPos != pos)
        {
            currentMapPos = pos;
            SetMapPosX( (int)(pos.x) );
            SetMapPosY( (int)(pos.y) );
        }
#endif
    }

    public void Send(string data)
    {
        if (!this.connected || !this.connectToGame)
            return;

#if UNITY_WEBGL && !UNITY_EDITOR
        WebSocketSend(data);
#else
        ws.Send(data);
#endif
    }

    public void ProcessMessages()
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
                playerScript.pantsColor = messageQueue[i].pantsColor;
                playerScript.shirtColor = messageQueue[i].shirtColor;
                playerScript.shoesColor = messageQueue[i].shoesColor;
                playerScript.hairColor = messageQueue[i].hairColor;
                playerScript.skinColor = messageQueue[i].skinColor;
                playerScript.ChangeColors();

                if (messageQueue[i].weapon != "handgun")
                    playerScript.SwitchWeapon(messageQueue[i].weapon);

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
                players[messageQueue[i].id].rigid.MovePosition(messageQueue[i].position);
                players[messageQueue[i].id].rigid.rotation = messageQueue[i].rotation;
            }
            else
            if (messageQueue[i].message == "playerdisconnect")
            {
                if (players.ContainsKey(messageQueue[i].id) && messageQueue[i].id != this.id)
                {
                    Destroy(players[messageQueue[i].id].gameObject);
                    players.Remove(messageQueue[i].id);
                }
            }
            else
            if (messageQueue[i].message == "roundfired" && messageQueue[i].shooterid != this.id)
            {
                Message message = messageQueue[i];

                int clip = 0;
                switch (message.damagePerRound)
                {
                    case 20:
                        clip = 0;
                        break;
                    case 10:
                        clip = 1;
                        break;
                    case 5:
                        clip = 2;
                        break;
                }
                   
                AudioSource.PlayClipAtPoint(audioClips[clip], message.position);
                this.FireRound(message.shooterid, message.roundLifeTime, message.damagePerRound, message.position, message.rotation, message.force, false);
            }
            else
            if (messageQueue[i].message == "respawn")
            {
                if (messageQueue[i].id == this.id)
                {
                    this.player.gameObject.SetActive(true);
                    this.player.Spawn(messageQueue[i].spawn);
                }
                else
                {
                    players[messageQueue[i].id].KillPlayer();

                    players[messageQueue[i].id].gameObject.SetActive(true);
                    players[messageQueue[i].id].Spawn(messageQueue[i].spawn);
                }
            }
            else
            if (players.ContainsKey(messageQueue[i].id) && messageQueue[i].message == "switchweapon" && messageQueue[i].id != this.id)
            {
                players[messageQueue[i].id ].SwitchWeapon(messageQueue[i].weapon);
            }
        }

        messageQueue.Clear();

    }

    public void OnConnect()
    {
        this.connected = true;
    }

    public void OnMessage(string message)
    {
        Message msg = (Message)JsonUtility.FromJson(message, typeof(Message));

        if (msg.message == "respawn")
        {
            messageQueue.Add(msg);
        }
        else
        if (msg.message == "newid")
        {
            this.id = msg.id;
            this.player.id = msg.id;
            this.player.startPoint = new Vector3(Dungeon.instance.stage[msg.spawn].x, 0.0f, Dungeon.instance.stage[msg.spawn].y) * 9f;
        }
        else
        if (msg.id == this.id)
            return;
        else
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
        {
            messageQueue.Add(msg);
        }
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
            message.id = this.id;

            this.Send(JsonUtility.ToJson(message));
        }
    }

    public void ConnectToGame()
    {
        this.connectToGame = true;
    }

    public void DisconnectFromGame()
    {
        this.connectToGame = false;
    }

    public void ChatMessage(string msg)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
      //  this.Send(msg);
#endif
    }

    public void CaptureKeyboard(string value)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (value == "0")
            WebGLInput.captureAllKeyboardInput = false;
        else
            WebGLInput.captureAllKeyboardInput = true;
#endif
    }
}
