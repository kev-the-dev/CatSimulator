using UnityEngine;
using UnityEngine.UI;


// Container class for stat buffs.
public class Buff
{
	/* Stat Buffs */
	/* 		"Buffs" affect the rate at which stats increase. 
			A buff value of 0 means the stat does not increase.
			A buff value of 1 means the stat increases normally.
			A buff value of < 1 means the stat increases slower than normal.
			A buff value of > 1 means that the stat increases faster than normal.
			
			"Debuffs" affect the rate at which stats decrease.
			A debuff value of 0 means the stat does not decrease.
			A debuff value of 1 means the stat decreases normally.
			A debuff value of < 1 means the stat decreases slower than normal.
			A debuff value of > 1 means the stat decreases faster than normal.
	*/
	
	private float _value;
	public float Value
	{
		get {return _value;} 
		set 
		{
			if (value < 0)
			{
				_value = 0;
			}
			else
			{
				_value = value;
			}
		}
	}
	
	public Buff ( float _buff_value = CatPersonality.DEFAULT_BUFF_VALUE)
	{
		if (_buff_value < 0)
		{
			_value = CatPersonality.DEFAULT_BUFF_VALUE;
		}
		else 
		{
			_value = _buff_value;
		}
		
	}
	
}

public class CatPersonality
{
	public CatPersonality(float hungriness, float tierdness, float playfullness, float cleanlieness, float sociability, float sleep_threshold = 0.2F, float hunger_threshold = 0.25F, float bladder_threshold = 0.2F)
	{
		this.hungriness = hungriness;
		this.tierdness = tierdness;
		this.playfullness = playfullness;
		this.cleanlieness = cleanlieness;
		this.sociability = sociability;
		this.sleep_threshold = sleep_threshold;
		this.hunger_threshold = hunger_threshold;
		this.bladder_threshold = bladder_threshold;

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
		this.bladder_decrease_per_second = CalculateMultipier(0.01F, 0.02F, Random.Range(MIN, MAX));
		this.bladder_increase_when_using_litter_box_per_second = CalculateMultipier(0.1F, 0.2F, Random.Range(MIN, MAX));
		
		this.energy_buff = new Buff();
		this.energy_debuff = new Buff();
		this.fullness_buff = new Buff();
		this.fullness_debuff = new Buff();
		this.fun_buff = new Buff();
		this.fun_debuff = new Buff();
		this.hygiene_buff = new Buff();
		this.hygiene_debuff = new Buff();
		this.bladder_buff = new Buff();
		this.bladder_debuff = new Buff();
		this.bond_buff = new Buff();
		this.bond_debuff = new Buff();
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
	
	public const float DEFAULT_BUFF_VALUE = 1F; // Default multiplier value is normally set to 1, but this can be increased for debugging purposes to make stats increase/decrease faster
	public const float CATNIP_FUN_BUFF = 1.5F; // Multiply this to fun_buff when cat is on catnip
	public const float CATNIP_FUN_DEBUFF = 0F; // Set fun_debuff to this value when cat is on catnip
	public const float CATNIP_ENERGY_DEBUFF = 0.5F; // Multiply this to energy_buff when cat is on catnip
	public const float CATNIP_SPEED_BOOST = 3F;
	
	public Buff energy_buff;
	public Buff energy_debuff;
	public Buff fullness_buff;
	public Buff fullness_debuff;
	public Buff fun_buff;
	public Buff fun_debuff;
	public Buff hygiene_buff;
	public Buff hygiene_debuff;
	public Buff bladder_buff;
	public Buff bladder_debuff;
	public Buff bond_buff;
	public Buff bond_debuff;
	
	// Threshold values
	public float sleep_threshold {get; private set;}		// Cat falls asleep when energy reaches this level
	public float hunger_threshold {get; private set;}		// Cat tries to eat when fullness reaches this level
	public float bladder_threshold {get; private set;}		// Cat uses the litter box when bladder stat reaches this level
	
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
		stats.Bond += dt * bond_increase_per_second * bond_buff.Value;
		// TODO: lower bond if cat is extreamly unhappy?
		stats.Bond += dt * bond_increase_per_happieness_per_second * bond_buff.Value;

		if (CatActivityEnum.BeingPet == activity.current)
		{
			stats.Bond += dt * bond_increase_when_being_pet_per_second * bond_buff.Value;
		}

		// Update fullness
		if (CatActivityEnum.Eating == activity.current) 
		{
			stats.Fullness += dt * fullness_increase_when_eating_per_second * fullness_buff.Value;
		} 
		else 
		{
			stats.Fullness -= dt * fullness_decrease_per_second * fullness_debuff.Value;
		}

		// Update energy
		if (CatActivityEnum.Sleeping == activity.current) 
		{
			stats.Energy += dt * energy_increase_when_sleeping_per_second * energy_buff.Value;
		} 
		else 
		{
			stats.Energy -= dt * energy_decrease_per_second * energy_debuff.Value;
		}

		// Update fun
		if (CatActivityEnum.PlayingWithYarn == activity.current) 
		{
			stats.Fun += dt * fun_increase_when_playing_with_yarn_per_second * fun_buff.Value;
		} 
		else if (CatActivityEnum.FollowingLaser == activity.current) 
		{
			stats.Fun += dt * fun_increase_when_following_laser_per_second * fun_buff.Value;
		} 
		else if (CatActivityEnum.OnCatnip == activity.current) 
		{
			stats.Fun += dt * fun_increase_when_on_catnip_per_second * fun_buff.Value;
		} 
		else 
		{
			stats.Fun -= dt * fun_decrease_per_second * fun_debuff.Value;
		}

		// Update hygiene
		if (CatActivityEnum.BeingBrushed == activity.current) 
		{
			stats.Hygiene += dt * hygiene_increase_when_being_brushed_per_second * hygiene_buff.Value;
		} 
		else 
		{
			// If cat is using the litterbox, its hygiene stat decays twice as fast
			if (CatActivityEnum.UsingLitterbox == activity.current)
			{
				stats.Hygiene -= dt * 2 * hygiene_decrease_per_second * hygiene_debuff.Value;
			}
			else
			{
				stats.Hygiene -= dt * hygiene_decrease_per_second * hygiene_debuff.Value;
			}
		}
		
		// Update bladder
		if (CatActivityEnum.UsingLitterbox == activity.current) 
		{
			stats.Bladder += dt * bladder_increase_when_using_litter_box_per_second * bladder_buff.Value;
		}
		else
		{	// If cat is eating, its bladder stat decays twice as fast
			if (CatActivityEnum.Eating == activity.current)
			{
				stats.Bladder -= dt * 2 * bladder_decrease_per_second * bladder_debuff.Value;
			}
			else 
			{
				stats.Bladder -= dt * bladder_decrease_per_second * bladder_debuff.Value;
			}
		}
	}
	
	// Resets all stat buffs / debuffs to default value
	public void resetStatBuffs()
	{
		this.energy_buff.Value = DEFAULT_BUFF_VALUE;
		this.energy_debuff.Value = DEFAULT_BUFF_VALUE;
		this.fullness_buff.Value = DEFAULT_BUFF_VALUE;
		this.fullness_debuff.Value = DEFAULT_BUFF_VALUE;
		this.fun_buff.Value = DEFAULT_BUFF_VALUE;
		this.fun_debuff.Value = DEFAULT_BUFF_VALUE;
		this.hygiene_buff.Value = DEFAULT_BUFF_VALUE;
		this.hygiene_debuff.Value = DEFAULT_BUFF_VALUE;
		this.bladder_buff.Value = DEFAULT_BUFF_VALUE;
		this.bladder_debuff.Value = DEFAULT_BUFF_VALUE;
		this.bond_buff.Value = DEFAULT_BUFF_VALUE;
		this.bond_debuff.Value = DEFAULT_BUFF_VALUE;
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