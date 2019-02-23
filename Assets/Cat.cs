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
	// The cat's current activity/behavior/goal
	CatActivity activity;
	// Tracks last time Update() was called for dt calculation
	float last_update_time;

    // Start is called before the first frame update
    void Start()
    {
		// If a previous save exists, load it
		if (PlayerPrefs.HasKey("savetime")) {
			Debug.Log("Previous save found, loading");
			Load();
		// Otherwise create a new random cat and save it
		} else {
			Debug.Log("No previous save found, creating a cat");
			CreateNew();
			Save();
		}

		// Start off eating for testing purposes
		activity = CatActivity.Sleeping;

		// Initialize last update time to now
		last_update_time = Time.time;
		
		Debug.Log(personality);
    }
	
	// Called when there is no save to generate a new random cat
	void CreateNew()
	{		// Initialize stats for a completely content cat
		stats = new CatStats();
		// Initialize personality to random values
		personality = CatPersonality.RandomPersonality();
	}

    // Update is called once per frame
    void Update()
    {
		// TODO: autosave
		//  Save game when S is pressed
        if(Input.GetKeyDown (KeyCode.S))
            Save();
		// Load game when L is pressed
        if (Input.GetKeyDown (KeyCode.L))
            Load();
		// If R is pressed, reset
		if (Input.GetKeyDown (KeyCode.R))
			CreateNew();

		// Calcuate time delta since last update, in seconds, fps-independent calculations
		float dt = Time.time - last_update_time;
		last_update_time = Time.time;

		// Update cat stats with current state
		personality.UpdateStats(ref stats, activity, dt);
		
		// TODO: change activity

		// Log current state
        Debug.Log(stats);
    }

	// Load the cat from a previous save
	public void Load()
	{
		personality = CatPersonality.Load();
		stats = CatStats.Load();
		// TODO: load pose, color, other info
		
		Debug.Log("--- LOADED --");
		Debug.Log(personality);
		Debug.Log(stats);
		Debug.Log("-------------");
	}

	// Save the current cat to a file for later resuming play
	public void Save()
	{
		personality.Save();
		stats.Save();
		// TODO: save pose, color, other info
		PlayerPrefs.SetFloat("savetime", Time.time);
		PlayerPrefs.Save();

		Debug.Log("--- SAVED --");
		Debug.Log(personality);
		Debug.Log(stats);
		Debug.Log("-------------");
	}
}
