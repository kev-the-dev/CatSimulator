using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Behavior script for the cat. Manages the cats behaviors, stats, and personality
public class Cat : MonoBehaviour
{
	// The cat's current stats, which appear on the HUD bars
	CatStats stats;
	// The cats permenant personality, initialized randomly
	CatPersonality personality;
	// Tracks last time Update() was called for dt calculation
	double last_update_time;

    // Start is called before the first frame update
    void Start()
    {
		// Initialize stats for a completely content cat
		stats = new CatStats();
		// Initialize personality to random values
		personality = CatPersonality.RandomPersonality();

		// Initialize last update time to now
		last_update_time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
		// Calcuate time delta since last update, in seconds, fps-independent calculations
		double dt = Time.time - last_update_time;
		last_update_time = Time.time;

		// Log current state
		Debug.Log(personality);
        Debug.Log(stats);
    }
}
