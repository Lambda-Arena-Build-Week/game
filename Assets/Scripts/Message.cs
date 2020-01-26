using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Message
{
    public string message;
    public int id;
    public int shooterid;
    public int targetid;
    public Vector3 position;
    public Quaternion rotation;
    public float force;
    public float roundLifeTime;
    public int damagePerRound;
    public Color shirtColor;
    public Color pantsColor;
    public Color skinColor;
    public Color hairColor;
    public Color shoesColor;
    public string weapon;
    public float animSpeed;
    public int spawn;
}

[Serializable]
public struct ChatMessage
{
    public string message;
    public string gametag;
    public string text;
}

[Serializable]
public struct MapPosition
{
    public Vector2 active_room;
}