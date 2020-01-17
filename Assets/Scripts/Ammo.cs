using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public string ammoPrefab = "Projectile";
    public int playerId;
    public int numberOfRounds = 12;
    public int damagePerRound = 5;
    public float fireRate = 0.5f;
    public float spreadAngle = 5.0f;
    public float roundLifetime = 2.0f;
    public float coolDownTime = 1.0f;

    public List<Projectile> rounds = new List<Projectile>();
    private float fireTimer = 0.0f;
    private bool fire = false;
    private bool firing = false;
    private float spawnRate;

    void Start()
    {
        this.spawnRate = this.fireRate / this.numberOfRounds;

        for (int i = 0; i < this.numberOfRounds; i++)
        {
            GameObject projectile = (GameObject)Instantiate(Resources.Load("Prefabs/Weapons/" + ammoPrefab));
            Projectile projectileScript = projectile.GetComponent<Projectile>();

            projectileScript.roundLifetime = this.roundLifetime;
            projectileScript.damageDone = this.damagePerRound;
            projectileScript.playerId = this.playerId;

            this.rounds.Add(projectileScript);
        }
    }

    private Projectile GetRound()
    {
        for (int i = 0; i < rounds.Count; i++)
        {
            if (!rounds[i].gameObject.activeInHierarchy)
                return rounds[i];
        }

        return null;
    }

    public void Fire()
    {
        this.fire = true;
    }

    void Update()
    {
        if (this.fire)
        {

        }
            
    }
}
