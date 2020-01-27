using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance = null;

    public float respawnRate = 3.0f;

    private float respawnTimer = 0.0f;
    public List<GameObject> powerups = new List<GameObject>();

    private void OnEnable()
    {
        if (instance == null)
            instance = this;
    }

    public void AddItem(GameObject item)
    {
        powerups.Add(item);
    }

    private void EnableItems()
    {
        for (int i = 0; i < powerups.Count; i++)
        {
            powerups[i].SetActive(true);
        }
    }

    private void Update()
    {
        this.respawnTimer += Time.deltaTime;

        if (this.respawnTimer >= this.respawnRate)
        {
            this.respawnTimer = 0.0f;
            this.EnableItems();
        }
    }
}
