using System;
using System.Collections.Generic;
using UnityEngine;

// Defines the current stats of the cat, which are displayed as bars in the HUD and influence behavior
public class CatAchievements
{
	public CatAchievements(int num_pets=0, int num_brushes=0, float time_played=0F)
	{
		this.num_pets = num_pets;
		this.num_brushes = num_brushes;
		this.time_played = time_played;

		unlocked = new LinkedList<CatAchievement>();

		locked = new LinkedList<CatAchievement>();

		// Add acheivements for reaching counters 
		for (int factor = 1, i = 0; i < 4; ++i, factor *= 10)
		{
			locked.AddFirst(new PetAchievement(factor));
			locked.AddFirst(new BrushAchievement(factor));
			locked.AddFirst(new MinutesPlayedAchievement(factor));
		}

		GetNewUnlocks(true);

		if (AdoptionCenter.IsActive()) {
			return;
		}

		this.ui = GameObject.Find("achievement_panel").GetComponent<AchievementUI>();
	}

	// Counters / trackers
	public int num_pets;
	public int num_brushes;
	public float time_played;

	// UI
	private AchievementUI ui;

	// Unlocked / locked
	private LinkedList<CatAchievement> locked;
	private LinkedList<CatAchievement> unlocked;
	
	public void GetNewUnlocks(bool first_run = false)
	{
		for(LinkedListNode<CatAchievement> node = locked.First; node != null;)
		{
			if (node.Value.Unlocked(this)) {
				LinkedListNode<CatAchievement> next = node.Next;
				locked.Remove(node);
				if (!first_run) {
					Debug.Log(string.Format("Unlocked {0}", node.Value));
					ui.OnUnlock(node.Value);
				}
				unlocked.AddFirst(node);
				node = next;
			} else {
				node = node.Next;
			}
		}
	}

	public override string ToString()
	{
		return string.Format("CatAchievements(num_pets={0}, num_brushes={1} time_played={2}",
							 num_pets, num_brushes, time_played);
	}

	public void Save()
	{
		PlayerPrefs.SetInt("acheivements.num_pets", num_pets);
		PlayerPrefs.SetInt("acheivements.num_brushes", num_brushes);
		PlayerPrefs.SetFloat("acheivements.time_played", time_played);
	}

	public static CatAchievements Load()
	{
		return new CatAchievements(PlayerPrefs.GetInt("acheivements.num_pets"),
		                           PlayerPrefs.GetInt("acheivements.num_brushes"),
								   PlayerPrefs.GetFloat("acheivements.time_played"));
	}
}