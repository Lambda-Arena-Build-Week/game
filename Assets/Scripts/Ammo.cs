using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public string ammoPrefab = "Projectile";
    public int playerId;
    public int numberOfRounds = 12;
    public int numberOfRoundsToSpawn = 48;
    public int damagePerRound = 5;
    public float fireRate = 0.5f;
    public float spreadAngle = 5.0f;
    public float roundLifetime = 2.0f;
    public float coolDownTime = 1.0f;
    public float projectileForce = 10.0f;
    public Transform spawnPoint;

    public List<Projectile> rounds = new List<Projectile>();
    private float fireTimer = 0.0f;
    private float coolDownTimer = 0.0f;
    private bool fire = false;
    private bool fired = false;
    private float spawnRate;
    private int roundsFired = 0;
    private float spread = 0.0f;
    private bool flipSpread = false;

    void Start()
    {
        this.spawnRate = this.fireRate / this.numberOfRounds;

        for (int i = 0; i < this.numberOfRoundsToSpawn; i++)
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
        if (!this.fired)
            this.fire = true;
    }

    void Update()
    {
        if (this.fire && !this.fired)
        {
            this.fireTimer += Time.deltaTime;

            if (this.fireTimer >= this.spawnRate)
            {
                Projectile projectile = this.GetRound();

                if (projectile != null)
                {
                    projectile.gameObject.transform.position = this.spawnPoint.position;
                    projectile.gameObject.transform.rotation = this.spawnPoint.rotation;

                    if (!this.flipSpread)
                    {
                        this.spread += Random.Range(-this.spreadAngle, this.spreadAngle);
                        this.spread = Mathf.Clamp(this.spread, -this.spreadAngle, this.spreadAngle);
                        this.flipSpread = true;
                    }
                    else
                    {
                        this.spread *= -1.0f;
                        this.spread = Mathf.Clamp(this.spread, -this.spreadAngle, this.spreadAngle);
                        this.flipSpread = false;
                    }

                    projectile.transform.Rotate(new Vector3(0.0f, this.spread, 0.0f));

                    this.fireTimer = 0.0f;
                    projectile.gameObject.SetActive(true);
                    projectile.rigid.AddForce(projectile.rigid.transform.forward * this.projectileForce, ForceMode.Impulse);

                    this.roundsFired++;
                }

                if (this.roundsFired >= numberOfRounds)
                {
                    this.fire = false;
                    this.fired = true;
                }
            }
        }

        if (this.fired)
        {
            this.coolDownTimer += Time.deltaTime;

            if (this.coolDownTimer >= this.coolDownTime)
            {
                this.fired = false;
                this.fireTimer = 0.0f;
                this.coolDownTimer = 0.0f;
                this.roundsFired = 0;
            }
        }
            
    }
}
