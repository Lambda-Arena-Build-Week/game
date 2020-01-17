using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallPieces
{
    WALL       = 0,
    WALL_TORCH = 1,
    WALL_DOOR  = 2,
}

public class Dungeon : MonoBehaviour
{
    List<GameObject> dungeonPieces = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        CreateRoom(Vector3.zero, new Vector2(15.0f, 15.0f));
    }

    private GameObject GetWallPiece(WallPieces wallPiece)
    {
        switch (wallPiece)
        {
            case WallPieces.WALL:
                return (GameObject)Instantiate(Resources.Load("Prefabs/Wall"));
            case WallPieces.WALL_DOOR:
                return (GameObject)Instantiate(Resources.Load("Prefabs/WallDoor"));
        }

        return null;
    }

    private void CreateRoom(Vector3 position, Vector2 roomSize)
    {

        WallPieces wallPieces = WallPieces.WALL_DOOR;
        GameObject wallPieceTop = GetWallPiece(wallPieces);
        GameObject wallPieceBottom = GetWallPiece(wallPieces);
        GameObject wallPieceLeft = GetWallPiece(wallPieces);
        GameObject wallPieceRight = GetWallPiece(wallPieces);

        wallPieceTop.transform.position = new Vector3(0.0f, 0.0f, -4.25f);
        wallPieceTop.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        wallPieceBottom.transform.position = new Vector3(0.0f, 0.0f, 4.25f);
        wallPieceBottom.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

        wallPieceLeft.transform.position = new Vector3(4.25f, 0.0f, 0.0f);
        wallPieceLeft.transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
        wallPieceRight.transform.position = new Vector3(-4.25f, 0.0f, 0.0f);
        wallPieceRight.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);

        dungeonPieces.Add(wallPieceTop);
        dungeonPieces.Add(wallPieceBottom);

        dungeonPieces.Add(wallPieceLeft);
        dungeonPieces.Add(wallPieceRight);


    }

}
