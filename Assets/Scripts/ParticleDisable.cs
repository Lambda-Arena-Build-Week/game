using UnityEngine;
using System.Collections;

public class ParticleDisable : MonoBehaviour {

	public ParticleSystem particle = null;
 
	void OnEnable () {
		ParticleSystem.EmissionModule em = particle.emission;
		em.enabled = true; 
	}
	

	void Update () 
	{
        if (particle != null && !particle.isPlaying )
        {
            ParticleSystem.EmissionModule em = particle.emission;
            em.enabled = false;

            this.gameObject.SetActive(false);
		}
	}
}
