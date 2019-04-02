using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HappyIndicatorController : MonoBehaviour
{
	
	ParticleSystem heartParticles;
	
    // Start is called before the first frame update
    void Start()
    {
		heartParticles = GetComponent<ParticleSystem>();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	public void turnOnHappyParticles()
	{
		heartParticles.Play();
	}
	
	public void turnOffHappyParticles()
	{
		heartParticles.Stop();
	}
}
