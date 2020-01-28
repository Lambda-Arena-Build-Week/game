using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StagePieces
{
    WALL       = 0,
    WALL_TORCH = 1,
    WALL_DOOR  = 2,
    FLOOR      = 3,
}

public class Dungeon : MonoBehaviour
{
    public static Dungeon instance = null;
    public List<Room> rooms = new List<Room>();
    public StagePiece[] stage;

    private void OnEnable()
    {
        if (instance == null)
            instance = this;
    }

    public void SetStage(StagePiece[] stagePieces)
    {
        this.stage = stagePieces;
    }

    public void BuildDungeon()
    {
        for (int i = 0; i < stage.Length; i++)
        {
            GameObject room = (GameObject)Instantiate(Resources.Load("Prefabs/Stage/Room"));
            Room roomScript = room.GetComponent<Room>();
            roomScript.CreateRoom(stage[i]);
            room.transform.parent = this.transform;
            this.rooms.Add(roomScript);
        }
    }
 


}