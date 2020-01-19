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
    public float projectileForce = 10.0f;
    public bool singleShot = false;
    public Transform spawnPoint;

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
    }

    public void Fire(bool value)
    {
        if (!this.fired)
            this.fire = value;
    }

    private void CreateProjectile()
    {
        Vector3 position = this.spawnPoint.position;
        Quaternion rotation = this.spawnPoint.rotation;

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

        Vector3 roundDirection = Quaternion.Euler(0.0f, this.spread, 0.0f) * this.spawnPoint.forward;
        rotation = Quaternion.LookRotation(roundDirection);

        Multiplayer.instance.FireRound(this.playerId, this.roundLifetime, this.damagePerRound, position, rotation, this.projectileForce, true);

        if (this.singleShot)
            this.roundsFired = this.numberOfRounds;
        else
            this.roundsFired++;
    }

    private void SingleShot()
    {
        this.fired = true;
        for (int i = 0; i < this.numberOfRounds; i++)
        {
            this.CreateProjectile();
        }
    }

    private void MultiShot()
    {
        this.fireTimer += Time.deltaTime;

        if (this.fireTimer >= this.spawnRate)
        {
            this.CreateProjectile();

            if (this.roundsFired >= this.numberOfRounds)
            {
                this.fire = false;
                this.fired = true;
            }
        }
    }

    void Update()
    {
        if (this.fire && !this.fired)
        {
            if (this.singleShot)
                this.SingleShot();
            else
                this.MultiShot();
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
