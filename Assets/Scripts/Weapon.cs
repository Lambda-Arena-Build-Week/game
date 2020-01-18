using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float fireRate = 0.5f;
    public Transform fireSpawn;
    public int playerId;

    private bool fire;
 
    public Ammo ammo;

    private void OnEnable()
    {  
        this.fire = false;
        this.ammo.playerId = this.playerId;
    }

    private void Update()
    {
        if (this.fire)
        {
            if (this.ammo != null)
                this.ammo.Fire();
        }
    }

    public void Fire(bool value)
    {
        this.fire = value;
    }
}
