using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float fireRate = 0.5f;
    public Transform fireSpawn;
    private float fireTimer = 0.0f;
    private bool fire;
    private bool firing;

    public List<GameObject> particles = new List<GameObject>();
    public string gunParticle;

    private void OnEnable()
    {
        this.firing = false;
        this.fire = false;
        this.fireTimer = 0.0f;

        for (int i = 0; i < 10; i++)
        {
            particles.Add((GameObject)Instantiate(Resources.Load<GameObject>("Prefabs/Weapons/" + gunParticle)));
            particles[particles.Count - 1].name = gunParticle;
        }
    }

    private void FireWeapon()
    {
        particles.Sort((GameObject obj1, GameObject obj2) => obj1.activeInHierarchy.CompareTo(obj2.activeInHierarchy));

        if (!particles[0].activeInHierarchy)
        {
            particles[0].transform.position = this.fireSpawn.position;
            particles[0].transform.rotation = this.fireSpawn.rotation;
            particles[0].transform.parent = this.fireSpawn;
            particles[0].SetActive(true);
        } 
    }

    private void Update()
    {
        if (this.fire)
        {
            if (Mathf.Approximately(this.fireTimer, 0.0f))
            {
                this.FireWeapon();
                this.firing = true;
            }
        }

        if (this.firing)
        {
            this.fireTimer += Time.deltaTime;

            if (this.fireTimer >= this.fireRate)
            {
                this.fireTimer = 0.0f;
                this.firing = false;
            }
        }
    }

    public void Fire(bool value)
    {
        this.fire = value;
    }
}
