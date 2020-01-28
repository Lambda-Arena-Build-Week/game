using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static AssetManager instance = null;

    public List<Projectile> rounds = new List<Projectile>();
    public List<GameObject> impacts = new List<GameObject>();
    public List<GameObject> bloodImpacts = new List<GameObject>();

    public string ammoPrefab = "Projectile";
    public string impactPrefab = "BulletHole";
    public string bloodPrefab = "Blood";

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

            GameObject impact = (GameObject)Instantiate(Resources.Load("Prefabs/Weapons/" + impactPrefab));
            this.impacts.Add(impact);

            GameObject blood = (GameObject)Instantiate(Resources.Load("Prefabs/Weapons/" + bloodPrefab));
            this.bloodImpacts.Add(blood);

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

    public void CreateImpact(Vector3 pos, Vector3 normal)
    {
        impacts.Sort((a, b) => a.activeInHierarchy.CompareTo(b.activeInHierarchy));

        impacts[0].transform.position = pos;
        impacts[0].transform.rotation = Quaternion.FromToRotation(Vector3.up, normal); 
        impacts[0].SetActive(true);
    }

    public void CreateBloodImpact(Vector3 pos, Vector3 normal)
    {
        bloodImpacts.Sort((a, b) => a.activeInHierarchy.CompareTo(b.activeInHierarchy));

        bloodImpacts[0].transform.position = pos;
        bloodImpacts[0].transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
        bloodImpacts[0].SetActive(true);
    }
}
