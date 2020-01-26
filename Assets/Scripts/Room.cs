using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    List<GameObject> dungeonPieces = new List<GameObject>();
    List<GameObject> items = new List<GameObject>();

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

    private StagePieces GetStagePiece(string door)
    {
        if (door.Length > 0)
            return StagePieces.WALL_DOOR;
        else
            return StagePieces.WALL;
    }

    public void SpawnItem(StagePiece stagePiece)
    {
        if (stagePiece.items.Length > 0)
        {
            for (int i = 0; i < stagePiece.items.Length; i++)
            {
                GameObject item = null;
                switch (stagePiece.items[i].category)
                {
                    case 1:
                        item = ((GameObject)Instantiate(Resources.Load("Prefabs/PowerUps/HealthPack")));
                        item.transform.parent = this.transform;
                        item.transform.position = Vector3.zero;
                        item.transform.localPosition = new Vector3(3.5f, 0.0f, 3.5f)  ;
                        break;
                    case 2:
                        item = ((GameObject)Instantiate(Resources.Load("Prefabs/PowerUps/ShotgunPowerUp")));
                        item.transform.parent = this.transform;
                        item.transform.position = Vector3.zero;
                        item.transform.localPosition = new Vector3(3.5f, 0.0f, 3.5f);
                        break;
                    case 3:
                        item = ((GameObject)Instantiate(Resources.Load("Prefabs/PowerUps/RiflePowerUp")));
                        item.transform.position = Vector3.zero;
                        item.transform.parent = this.transform;
                        item.transform.localPosition = new Vector3(3.5f, 0.0f, 3.5f) ;
                        break;

                }
                if (item != null)
                    items.Add(item);
            }
        }
    }

    public void CreateRoom(StagePiece stagePiece)
    {
        GameObject wallPieceTop = GetStagePiece(this.GetStagePiece(stagePiece.n_to));
        GameObject wallPieceBottom = GetStagePiece(this.GetStagePiece(stagePiece.s_to));
        GameObject wallPieceLeft = GetStagePiece(this.GetStagePiece(stagePiece.w_to));
        GameObject wallPieceRight = GetStagePiece(this.GetStagePiece(stagePiece.e_to));
        GameObject floor = GetStagePiece(StagePieces.FLOOR);

        wallPieceTop.transform.parent = this.transform;
        wallPieceBottom.transform.parent = this.transform;
        wallPieceLeft.transform.parent = this.transform;
        wallPieceRight.transform.parent = this.transform;
        floor.transform.parent = this.transform;

        wallPieceTop.transform.localPosition = new Vector3(0.0f, 0.0f, -4.25f);
        wallPieceTop.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        wallPieceBottom.transform.localPosition = new Vector3(0.0f, 0.0f, 4.25f);
        wallPieceBottom.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

        wallPieceLeft.transform.localPosition = new Vector3(4.25f, 0.0f, 0.0f);
        wallPieceLeft.transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
        wallPieceRight.transform.localPosition = new Vector3(-4.25f, 0.0f, 0.0f);
        wallPieceRight.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);

        floor.transform.localPosition = Vector3.zero;

        dungeonPieces.Add(floor);
        dungeonPieces.Add(wallPieceTop);
        dungeonPieces.Add(wallPieceBottom);

        dungeonPieces.Add(wallPieceLeft);
        dungeonPieces.Add(wallPieceRight);

        this.gameObject.transform.position = new Vector3(stagePiece.x, 0.0f,stagePiece.y) * 9f;
        this.gameObject.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        this.SpawnItem(stagePiece);
    }
}
