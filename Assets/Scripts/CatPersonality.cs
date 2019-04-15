using UnityEngine;
using UnityEngine.UI;


public class CatPersonality
{
	public CatPersonality(float hungriness, float tierdness, float playfullness, float cleanlieness, float sociability, float sleep_threshold = 0.2F, float hunger_threshold = 0.25F, float bladder_threshold = 0.2F, float fun_threshold = 0.7F)
	{
		this.hungriness = hungriness;
		this.tierdness = tierdness;
		this.playfullness = playfullness;
		this.cleanlieness = cleanlieness;
		this.sociability = sociability;
		this.sleep_threshold = sleep_threshold;
		this.hunger_threshold = hunger_threshold;
		this.bladder_threshold = bladder_threshold;
		this.fun_threshold = fun_threshold;

		// If in adoption mode, set card sliders
		if (AdoptionCenter.IsActive()) {
			GameObject.Find("hungriness_slider").GetComponent<Slider>().value = (int) 100 * this.hungriness;
			GameObject.Find("tierdness_slider").GetComponent<Slider>().value = (int) 100 * this.tierdness;
			GameObject.Find("playfullness_slider").GetComponent<Slider>().value = (int) 100 * this.playfullness;
			GameObject.Find("cleanlieness_slider").GetComponent<Slider>().value = (int) 100 * this.cleanlieness;
			GameObject.Find("sociability_slider").GetComponent<Slider>().value = (int) 100 * this.sociability;
			return;
		}
		
		this.bond_increase_per_second = CalculateMultipier(0.01F, 0.02F, this.sociability);
		this.bond_increase_when_being_pet_per_second = CalculateMultipier(0.01F, 0.02F, this.sociability);
		this.bond_increase_per_happieness_per_second = CalculateMultipier(0.01F, 0.02F, this.sociability);
		this.fullness_decrease_per_second = CalculateMultipier(0.008F, 0.015F, this.hungriness);
		this.fullness_increase_when_eating_per_second = CalculateMultipier(0.05F, 0.1F, this.hungriness, true);
		this.energy_decrease_per_second = CalculateMultipier(0.006F, 0.01F, this.tierdness);
		this.energy_increase_when_sleeping_per_second = CalculateMultipier(0.1F, 0.2F, this.tierdness, true);
		this.fun_decrease_per_second = CalculateMultipier(0.015F, 0.025F, this.playfullness);
		this.fun_increase_when_following_laser_per_second = CalculateMultipier(0.05F, 0.1F, this.playfullness);
		this.fun_increase_when_playing_with_yarn_per_second = CalculateMultipier(0.05F, 0.1F, this.playfullness);
		this.fun_increase_when_on_catnip_per_second = CalculateMultipier(0.05F, 0.1F, this.playfullness);
		this.hygiene_decrease_per_second = CalculateMultipier(0.01F, 0.02F, this.cleanlieness);
		this.hygiene_increase_when_being_brushed_per_second = CalculateMultipier(0.1F, 0.2F, this.cleanlieness);
		this.bladder_decrease_per_second = CalculateMultipier(0.005F, 0.01F, Random.Range(MIN, MAX));
		this.bladder_increase_when_using_litter_box_per_second = CalculateMultipier(0.1F, 0.2F, Random.Range(MIN, MAX));
		
		agent = GameObject.Find("Cat").GetComponent<UnityEngine.AI.NavMeshAgent>();
		
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
	private float bladder_decrease_per_second;
	private float bladder_increase_when_using_litter_box_per_second;
	
	// Threshold values
	public float sleep_threshold {get; private set;}		// Cat falls asleep when energy reaches this level
	public float hunger_threshold {get; private set;}		// Cat tries to eat when fullness reaches this level
	public float bladder_threshold {get; private set;}		// Cat uses the litter box when bladder stat reaches this level
	public float fun_threshold {get; private set;}			// Cat plays with toy when fun stat reaches this level
	
	// Reference to cat's nav mesh agent
	UnityEngine.AI.NavMeshAgent agent;
	
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
		stats.Bond += dt * bond_increase_per_second * stats.bond_buff.Value;
		// TODO: lower bond if cat is extreamly unhappy?
		stats.Bond += dt * bond_increase_per_happieness_per_second * stats.bond_buff.Value;

		if (CatActivityEnum.BeingPet == activity.current)
		{
			stats.Bond += dt * bond_increase_when_being_pet_per_second * stats.bond_buff.Value;
		}

		// Update fullness
		if (CatActivityEnum.Eating == activity.current) 
		{
			stats.Fullness += dt * fullness_increase_when_eating_per_second * stats.fullness_buff.Value;
		} 
		else 
		{
			stats.Fullness -= dt * fullness_decrease_per_second * stats.fullness_debuff.Value;
		}

		// Update energy
		if (CatActivityEnum.Sleeping == activity.current) 
		{
			stats.Energy += dt * energy_increase_when_sleeping_per_second * stats.energy_buff.Value;
		} 
		else 
		{
			stats.Energy -= dt * energy_decrease_per_second * stats.energy_debuff.Value;
		}

		// Update fun
		if (CatActivityEnum.Playing == activity.current) 
		{
			stats.Fun += dt * fun_increase_when_playing_with_yarn_per_second * stats.fun_buff.Value;
			agent.speed = 7F;
		} 
		else if (CatActivityEnum.FollowingLaser == activity.current) 
		{
			stats.Fun += dt * fun_increase_when_following_laser_per_second * stats.fun_buff.Value;
			agent.speed = 7F;
		} 
		else if (CatActivityEnum.OnCatnip == activity.current) 
		{
			stats.Fun += dt * fun_increase_when_on_catnip_per_second * stats.fun_buff.Value;
			agent.speed = 7F;
		} 
		else 
		{
			stats.Fun -= dt * fun_decrease_per_second * stats.fun_debuff.Value;
			agent.speed = 3.5F;
		}

		// Update hygiene
		if (CatActivityEnum.BeingBrushed == activity.current) 
		{
			stats.Hygiene += dt * hygiene_increase_when_being_brushed_per_second * stats.hygiene_buff.Value;
		} 
		else 
		{
			// If cat is using the litterbox, its hygiene stat decays twice as fast
			if (CatActivityEnum.UsingLitterbox == activity.current)
			{
				stats.hygiene_debuff.Value = 2;
				stats.Hygiene -= dt * hygiene_decrease_per_second * stats.hygiene_debuff.Value;
			}
			else
			{
				stats.hygiene_debuff.Value = CatStats.DEFAULT_BUFF_VALUE;
				stats.Hygiene -= dt * hygiene_decrease_per_second * stats.hygiene_debuff.Value;
			}
		}
		
		// Update bladder
		if (CatActivityEnum.UsingLitterbox == activity.current) 
		{
			stats.Bladder += dt * bladder_increase_when_using_litter_box_per_second * stats.bladder_buff.Value;
		}
		else
		{	// If cat is eating, its bladder stat decays twice as fast
			if (CatActivityEnum.Eating == activity.current)
			{
				stats.bladder_debuff.Value = 2;
				stats.Bladder -= dt * bladder_decrease_per_second * stats.bladder_debuff.Value;
			}
			else 
			{
				stats.bladder_debuff.Value = CatStats.DEFAULT_BUFF_VALUE;
				stats.Bladder -= dt * bladder_decrease_per_second * stats.bladder_debuff.Value;
			}
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