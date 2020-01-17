using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Message
{
    public string message;
    public int id;
    public string shooterid;
    public string targetid;
    public Vector3 position;
    public Quaternion rotation;
    public Color shirtColor;
    public Color pantsColor;
    public Color skinColor;
    public Color hairColor;
    public Color shoesColor;
    public float animSpeed;
}

 