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
    List<GameObject> dungeonPieces = new List<GameObject>();

    void Start()
    {
        CreateRoom(Vector3.zero, new Vector2(15.0f, 15.0f));
    }

    private GameObject GetStagePiece(StagePieces wallPiece)
    {
        switch (wallPiece)
        {
            case StagePieces.WALL:
                return (GameObject)Instantiate(Resources.Load("Prefabs/Stage/Wall"));
            case StagePieces.WALL_DOOR:
                return (GameObject)Instantiate(Resources.Load("Prefabs/Stage/WallDoor"));
            case StagePieces.FLOOR:
                return (GameObject)Instantiate(Resources.Load("Prefabs/Stage/Floor"));

        }

        return null;
    }

    private void CreateRoom(Vector3 position, Vector2 roomSize)
    {
        StagePieces wallPieces = StagePieces.WALL_DOOR;
        GameObject wallPieceTop = GetStagePiece(wallPieces);
        GameObject wallPieceBottom = GetStagePiece(wallPieces);
        GameObject wallPieceLeft = GetStagePiece(wallPieces);
        GameObject wallPieceRight = GetStagePiece(wallPieces);
        GameObject floor = GetStagePiece(StagePieces.FLOOR);

        wallPieceTop.transform.position = new Vector3(0.0f, 0.0f, -4.25f);
        wallPieceTop.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        wallPieceBottom.transform.position = new Vector3(0.0f, 0.0f, 4.25f);
        wallPieceBottom.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

        wallPieceLeft.transform.position = new Vector3(4.25f, 0.0f, 0.0f);
        wallPieceLeft.transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
        wallPieceRight.transform.position = new Vector3(-4.25f, 0.0f, 0.0f);
        wallPieceRight.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);

        floor.transform.position = Vector3.zero;

        dungeonPieces.Add(floor);
        dungeonPieces.Add(wallPieceTop);
        dungeonPieces.Add(wallPieceBottom);

        dungeonPieces.Add(wallPieceLeft);
        dungeonPieces.Add(wallPieceRight);
    }
}
