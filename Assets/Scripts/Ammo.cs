using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public string ammoPrefab;
    public int numberOfRounds = 12;
    public List<GameObject> rounds = new List<GameObject>();

    public float fireRate = 0.5f;
    private float fireTimer = 0.0f;

    void Start()
    {
        
    }

    private GameObject GetRound()
    {
        rounds.Sort((GameObject obj1, GameObject obj2) => obj1.activeInHierarchy.CompareTo(obj2.activeInHierarchy));

        if (!rounds[0].activeInHierarchy)
            return rounds[0];

        return null;
    }

    void Update()
    {
        
    }
}
