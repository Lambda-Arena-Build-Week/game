﻿using System.Collections;
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
    public bool connectToGame = false;

    private List<Message> messageQueue = new List<Message>();

    private void OnEnable()
    {
        if (instance == null)
            instance = this;

        this.player.isMenu = false;
        this.connected = false;

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
        string json = JsonUtility.ToJson(message);

        this.Send(json);
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
                    this.FireRound(message.shooterid, message.roundLifeTime, message.damagePerRound, message.position, message.rotation, message.force, false);
                }
                else
                if (messageQueue[i].message == "respawn")
                {
                    if (messageQueue[i].id == this.id)
                    {
                        this.player.gameObject.SetActive(true);
                        this.player.Spawn(messageQueue[i].position);
                    }
                    else
                    {
                        players[messageQueue[i].id].gameObject.SetActive(true);
                        players[messageQueue[i].id].Spawn(messageQueue[i].position);
                    }
                }
                else
                if (messageQueue[i].message == "killplayer" && messageQueue[i].id != this.id)
                {
                    players[messageQueue[i].targetid].KillPlayer();
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
}
