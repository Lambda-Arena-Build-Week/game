using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static AssetManager instance = null;

    public List<Projectile> rounds = new List<Projectile>();
    public string ammoPrefab = "Projectile";
    public int numberOfRoundsToSpawn = 500;

    private void OnEnable()
    {
        if (instance == null)
            instance = this;

        this.SpawnProjectiles();
    }

    private void SpawnProjectiles()
    {
        for (int i = 0; i < this.numberOfRoundsToSpawn; i++)
        {
            GameObject projectile = (GameObject)Instantiate(Resources.Load("Prefabs/Weapons/" + ammoPrefab));
            Projectile projectileScript = projectile.GetComponent<Projectile>();

            this.rounds.Add(projectileScript);
        }
    }

    public Projectile GetProjectile()
    {
        rounds.Sort((a, b) => a.gameObject.activeInHierarchy.CompareTo(b.gameObject.activeInHierarchy));

        if (rounds[0].gameObject.activeInHierarchy)
        {
            GameObject projectile = (GameObject)Instantiate(Resources.Load("Prefabs/Weapons/" + ammoPrefab));
            Projectile projectileScript = projectile.GetComponent<Projectile>();

            this.rounds.Add(projectileScript);
            return projectileScript;
        }

        return rounds[0];
    }
}
