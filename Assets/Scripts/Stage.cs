using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct StagePiece
{
    public string rm_id;
    public string title;
    public string description;
    public string n_to;
    public string s_to;
    public string e_to;
    public string w_to;
    public int x;
    public int y;
    public int player_ct;
    public bool has_item;
    public Item[] items;
}

[Serializable]
public struct Item
{
    public int id;
    public int category;
    public string name;
    public string description;
}