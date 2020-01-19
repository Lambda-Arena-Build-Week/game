using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int playerId;
    public int damageDone;
    public float roundLifetime;
    public Rigidbody rigid = null;

    private float lifetimeTimer = 0.0f;

    private void OnEnable()
    {
        if (!this.rigid)
            this.rigid = this.GetComponent<Rigidbody>();
    }

    public void DisableProjectile()
    {
        this.rigid.angularVelocity = Vector3.zero;
        this.rigid.velocity = Vector3.zero;

        this.lifetimeTimer = 0.0f;

        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        this.lifetimeTimer += Time.deltaTime;

        if (this.lifetimeTimer >= this.roundLifetime)
            this.DisableProjectile();
    }

    public void OnCollisionEnter(Collision collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player == null)
            this.DisableProjectile();
    }
}
