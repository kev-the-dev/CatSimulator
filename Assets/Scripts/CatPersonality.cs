using UnityEngine;

public class CatPersonality
{
	public CatPersonality(float hungriness, float tierdness, float playfullness, float cleanlieness, float sociability, float sleep_threshold = 0.2F, float hunger_threshold = 0.25F)
	{
		this.hungriness = hungriness;
		this.tierdness = tierdness;
		this.playfullness = playfullness;
		this.cleanlieness = cleanlieness;
		this.sociability = sociability;
		this.sleep_threshold = sleep_threshold;
		this.hunger_threshold = hunger_threshold;
		
		// Debug Values
		/*
		this.bond_increase_per_second = CalculateMultipier(0.01F, 0.02F, this.sociability);
		this.bond_increase_when_being_pet_per_second = CalculateMultipier(0.01F, 0.02F, this.sociability);
		this.bond_increase_per_happieness_per_second = CalculateMultipier(0.01F, 0.02F, this.sociability);
		this.fullness_decrease_per_second = CalculateMultipier(0.1F, 0.11F, this.hungriness);
		this.fullness_increase_when_eating_per_second = CalculateMultipier(0.05F, 0.1F, this.hungriness, true);
		this.energy_decrease_per_second = CalculateMultipier(0.01F, 0.02F, this.tierdness);
		this.energy_increase_when_sleeping_per_second = CalculateMultipier(0.1F, 0.2F, this.tierdness, true);
		this.fun_decrease_per_second = CalculateMultipier(0.01F, 0.02F, this.playfullness);
		this.fun_increase_when_following_laser_per_second = CalculateMultipier(0.05F, 0.1F, this.playfullness);
		this.fun_increase_when_playing_with_yarn_per_second = CalculateMultipier(0.05F, 0.1F, this.playfullness);
		this.fun_increase_when_on_catnip_per_second = CalculateMultipier(0.05F, 0.1F, this.playfullness);
		this.hygiene_decrease_per_second = CalculateMultipier(0.01F, 0.02F, this.cleanlieness);
		this.hygiene_increase_when_being_brushed_per_second = CalculateMultipier(0.1F, 0.2F, this.cleanlieness);
		*/
		
		this.bond_increase_per_second = CalculateMultipier(0.01F, 0.02F, this.sociability);
		this.bond_increase_when_being_pet_per_second = CalculateMultipier(0.01F, 0.02F, this.sociability);
		this.bond_increase_per_happieness_per_second = CalculateMultipier(0.01F, 0.02F, this.sociability);
		this.fullness_decrease_per_second = CalculateMultipier(0.01F, 0.02F, this.hungriness);
		this.fullness_increase_when_eating_per_second = CalculateMultipier(0.05F, 0.1F, this.hungriness, true);
		this.energy_decrease_per_second = CalculateMultipier(0.01F, 0.02F, this.tierdness);
		this.energy_increase_when_sleeping_per_second = CalculateMultipier(0.1F, 0.2F, this.tierdness, true);
		this.fun_decrease_per_second = CalculateMultipier(0.01F, 0.02F, this.playfullness);
		this.fun_increase_when_following_laser_per_second = CalculateMultipier(0.05F, 0.1F, this.playfullness);
		this.fun_increase_when_playing_with_yarn_per_second = CalculateMultipier(0.05F, 0.1F, this.playfullness);
		this.fun_increase_when_on_catnip_per_second = CalculateMultipier(0.05F, 0.1F, this.playfullness);
		this.hygiene_decrease_per_second = CalculateMultipier(0.01F, 0.02F, this.cleanlieness);
		this.hygiene_increase_when_being_brushed_per_second = CalculateMultipier(0.1F, 0.2F, this.cleanlieness);
	}

	// Generates and returns a random cat personality
	public static CatPersonality RandomPersonality()
	{
		return  new CatPersonality(Random.Range(MIN, MAX), Random.Range(MIN, MAX), Random.Range(MIN, MAX), Random.Range(MIN, MAX), Random.Range(MIN, MAX));
	}

	// Maximum value a personality trait can have
	public const float MAX = 1.0F;
	public const float MIN = 0.0F;

	/** Traits:
          the numbers that define a unique personality **/
	// How quickly cat gets hungry, how much food is needed to satisfy it
	public float hungriness;
	// How quickly cat gets tierd, how much sleep is needed to satisfy it
	public float tierdness;
	// How quickly cat gets bored, how much play is needed to satisfy it
	public float playfullness;
	// How qucikly cat loses hygiene, how much cleaning is needed
	public float cleanlieness;
	// How quickly cat bonds with the owner
	public float sociability;

	/** Multipliers 
	      information affect behavior **/
	private float bond_increase_per_second;
	private float bond_increase_per_happieness_per_second;
	private float bond_increase_when_being_pet_per_second;
	private float fullness_decrease_per_second;
	private float fullness_increase_when_eating_per_second;
	private float energy_decrease_per_second;
	private float energy_increase_when_sleeping_per_second;
	private float fun_decrease_per_second;
	private float fun_increase_when_following_laser_per_second;
	private float fun_increase_when_playing_with_yarn_per_second;
	private float fun_increase_when_on_catnip_per_second;
	private float hygiene_decrease_per_second;
	private float hygiene_increase_when_being_brushed_per_second;
	
	// Threshold values
	public float sleep_threshold {get; private set;}		// Cat falls asleep when energy reaches this level
	public float hunger_threshold {get; private set;}		// Cat tries to eat when fullness reaches this level
	
	public override string ToString()
	{
		return string.Format("CatPersonality(hungriness={0}, tierdness={1}, playfullness={2}, sociability={3})",
							 hungriness, tierdness, playfullness, sociability);
	}

	private static float CalculateMultipier(float min, float max, float trait, bool inverted = false)
	{
		if (inverted) {
			trait = MAX - trait;
		}
		return min + (max - min) * trait;
	}

	// Should be called at each update loop. Update cat stats based on time passing
	public void UpdateStats(ref CatStats stats, CatActivity activity, float dt)
	{
		// Update bond
		stats.Bond += dt * bond_increase_per_second;
		// TODO: lower bond if cat is extreamly unhappy?
		stats.Bond += dt * bond_increase_per_happieness_per_second;

		if (CatActivityEnum.BeingPet == activity.current)
		{
			stats.Bond += dt * bond_increase_when_being_pet_per_second;
		}

		// Update fullness
		if (CatActivityEnum.Eating == activity.current) {
			stats.Fullness += dt * fullness_increase_when_eating_per_second;
		} else {
			stats.Fullness -= dt * fullness_decrease_per_second;
		}

		// Update energy
		if (CatActivityEnum.Sleeping == activity.current) {
			stats.Energy += dt * energy_increase_when_sleeping_per_second;
		} else {
			stats.Energy -= dt * energy_decrease_per_second;
		}

		// Update playfullness
		if (CatActivityEnum.PlayingWithYarn == activity.current) {
			stats.Fun += dt * fun_increase_when_playing_with_yarn_per_second;
		} else if (CatActivityEnum.FollowingLaser == activity.current) {
			stats.Fun += dt * fun_increase_when_following_laser_per_second;
		} else if (CatActivityEnum.OnCatnip == activity.current) {
			stats.Fun += dt * fun_increase_when_on_catnip_per_second;
		} else {
			stats.Fun -= dt * fun_decrease_per_second;
		}

		// Update hygiene
		if (CatActivityEnum.BeingBrushed == activity.current) {
			stats.Hygiene += dt * hygiene_increase_when_being_brushed_per_second;
		} else {
			stats.Hygiene -= dt * hygiene_decrease_per_second;
		}
	}
	
	
	public void Save()
	{
		PlayerPrefs.SetFloat("personality.hungriness", hungriness);
		PlayerPrefs.SetFloat("personality.tierdness", tierdness);
		PlayerPrefs.SetFloat("personality.playfullness", playfullness);
		PlayerPrefs.SetFloat("personality.cleanlieness", cleanlieness);
		PlayerPrefs.SetFloat("personality.sociability", sociability);
	}
	
	public static CatPersonality Load()
	{
		return new CatPersonality(PlayerPrefs.GetFloat("personality.hungriness"), 
								  PlayerPrefs.GetFloat("personality.tierdness"),
								  PlayerPrefs.GetFloat("personality.playfullness"),
								  PlayerPrefs.GetFloat("personality.cleanlieness"),
								  PlayerPrefs.GetFloat("personality.sociability"));
	}
}