using System;
using UnityEngine;
using UnityEngine.UI;

// Defines the current stats of the cat, which are displayed as bars in the HUD and influence behavior
public class CatStats
{
	public CatStats(float energy = MAX, float fullness = MAX, float fun = MAX, float hygiene = MAX, float bladder = MAX, float bond = MAX)
	{
		this.energy = energy;
		this.fullness = fullness;
		this.fun = fun;
		this.hygiene = hygiene;
		this.bladder = bladder;
		this.bond = bond;
		
		Debug.Log("Initializing energy_buff...");
		this.energy_buff = new Buff (true, DEFAULT_BUFF_VALUE, GameObject.Find("EnergySliderGreenEffect"), GameObject.Find("EnergySliderYellowEffect"));
		
		Debug.Log("Initializing energy_debuff...");
		this.energy_debuff = new Buff (false, DEFAULT_BUFF_VALUE, GameObject.Find("EnergySliderGreenEffect"), GameObject.Find("EnergySliderYellowEffect"));
		
		Debug.Log("Initializing fullness_buff...");
		this.fullness_buff = new Buff (true, DEFAULT_BUFF_VALUE, GameObject.Find("FoodSliderGreenEffect"), GameObject.Find("FoodSliderYellowEffect"));
		
		Debug.Log("Initializing fullness_debuff...");
		this.fullness_debuff = new Buff (false, DEFAULT_BUFF_VALUE, GameObject.Find("FoodSliderGreenEffect"), GameObject.Find("FoodSliderYellowEffect"));
		
		Debug.Log("Initializing fun_buff...");
		this.fun_buff = new Buff (true, DEFAULT_BUFF_VALUE, GameObject.Find("FunSliderGreenEffect"), GameObject.Find("FunSliderYellowEffect"));
		
		Debug.Log("Initializing fun_debuff...");
		this.fun_debuff = new Buff (false, DEFAULT_BUFF_VALUE, GameObject.Find("FunSliderGreenEffect"), GameObject.Find("FunSliderYellowEffect"));
		
		Debug.Log("Initializing hygiene_buff...");
		this.hygiene_buff = new Buff (true, DEFAULT_BUFF_VALUE, GameObject.Find("HygieneSliderGreenEffect"), GameObject.Find("HygieneSliderYellowEffect"));
		
		Debug.Log("Initializing hygiene_debuff...");
		this.hygiene_debuff = new Buff (false, DEFAULT_BUFF_VALUE, GameObject.Find("HygieneSliderGreenEffect"), GameObject.Find("HygieneSliderYellowEffect"));
		
		Debug.Log("Initializing bladder_buff...");
		this.bladder_buff = new Buff (true, DEFAULT_BUFF_VALUE, GameObject.Find("BladderSliderGreenEffect"), GameObject.Find("BladderSliderYellowEffect"));
		
		Debug.Log("Initializing bladder_debuff...");
		this.bladder_debuff = new Buff (false, DEFAULT_BUFF_VALUE, GameObject.Find("BladderSliderGreenEffect"), GameObject.Find("BladderSliderYellowEffect"));
		
		Debug.Log("Initializing bond_buff...");
		this.bond_buff = new Buff(true, DEFAULT_BUFF_VALUE);
		
		Debug.Log("Initializing bond_debuff...");
		this.bond_debuff = new Buff(false, DEFAULT_BUFF_VALUE);
		
		updateAllStatBuffVisualEffects();

		if (AdoptionCenter.IsActive()) {
			return;
		}

		this.energy_slider = GameObject.Find("energy_slider").GetComponent <Slider> ();
		this.fullness_slider = GameObject.Find("food_slider").GetComponent <Slider> ();
		this.hygiene_slider = GameObject.Find("hygiene_slider").GetComponent <Slider> ();
		this.fun_slider = GameObject.Find("fun_slider").GetComponent <Slider> ();
		this.bladder_slider = GameObject.Find("bladder_slider").GetComponent <Slider> ();
		this.happy_indicator = GameObject.Find("HeartParticles").GetComponent<Renderer>();
		this.meow_sound = GameObject.Find("meow_sound").GetComponent<AudioSource>();
		
	}

	// Maximum value of each individual stat
	public const float MAX = 1.0F;
	// Minimimu value of each indiviual stat
	public const float MIN = 0.0F;
	public const float HAPPIENESS_INDICATOR_THRESHOLD = MAX * 0.75F;
	public const float MEOW_PERIOD = 10F;
	
	// Stat buff constants
	public const float DEFAULT_BUFF_VALUE = 1F; // Default multiplier value is normally set to 1, but this can be increased for debugging purposes to make stats increase/decrease faster
	public const float CATNIP_FUN_BUFF = 1.5F; // Multiply this to fun_buff when cat is on catnip
	public const float CATNIP_FUN_DEBUFF = 0F; // Set fun_debuff to this value when cat is on catnip
	public const float CATNIP_ENERGY_DEBUFF = 0.5F; // Multiply this to energy_buff when cat is on catnip
	public const float CATNIP_SPEED_BOOST = 3F;

	// How energetic the cat is, when maximized cat has no desire for sleep
	private float energy;
	public float Energy {get {return energy;} set {energy = Math.Min(MAX, Math.Max(value, MIN));}}
	
	// Inverse of hunger, when maximized cat has no desire for food
	private float fullness;
	public float Fullness {get {return fullness;} set {fullness = Math.Min(MAX, Math.Max(value, MIN));}}
	
	// How much fun cat is having, when maximized cat is having, like, a lot of fun
	private float fun;
	public float Fun {get {return fun;} set {fun = Math.Min(MAX, Math.Max(value, MIN));}}
	
	// How clean the cat and its environment is. When maximized, cat is content with its hygiene
	private float hygiene;
	public float Hygiene {get {return hygiene;} set {hygiene = Math.Min(MAX, Math.Max(value, MIN));}}
	
	// How much the cat needs to use the bathroom
	private float bladder;
	public float Bladder {get {return bladder;} set {bladder = Math.Min(MAX, Math.Max(value, MIN));}}
	
	// How connected cat is with the owner
	private float bond;
	public float Bond {get {return bond;} set {bond = Math.Min(MAX, Math.Max(value, MIN));}}

	// UI sliders
	private Slider energy_slider;
	private Slider fullness_slider;
	private Slider fun_slider;
	private Slider hygiene_slider;
	private Slider bladder_slider;
	
	// Happineness indicator
	private Renderer happy_indicator;
	private AudioSource meow_sound;
	private float last_meow;
	
	// Stat buffs
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

	// The total happieness/contentness of the cat. When == MAX, all desires of cat are satisfied
	public float happieness()
	{
		return (energy + fullness + fun + hygiene + bladder) / (5.0F * MAX);
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
	
	public void updateAllStatBuffVisualEffects()
	{
		this.energy_buff.updateVisualEffects();
		this.energy_debuff.updateVisualEffects();
		this.fullness_buff.updateVisualEffects();
		this.fullness_debuff.updateVisualEffects();
		this.fun_buff.updateVisualEffects();
		this.fun_debuff.updateVisualEffects();
		this.hygiene_buff.updateVisualEffects();
		this.hygiene_debuff.updateVisualEffects();
		this.bladder_buff.updateVisualEffects();
		this.bladder_debuff.updateVisualEffects();
		this.bond_buff.updateVisualEffects();
		this.bond_debuff.updateVisualEffects();
	}
	
	private void checkStatBuffs()
	{
		
	}

	// Update the stat bars with the current stats
	public void UpdateUI()
	{
		this.energy_slider.value = energy;
		this.fullness_slider.value = fullness;
		this.fun_slider.value = fun;
		this.hygiene_slider.value = hygiene;
		this.bladder_slider.value = bladder;
		
		if (happieness() > HAPPIENESS_INDICATOR_THRESHOLD) {
			this.happy_indicator.enabled = true;
			if (Time.time - last_meow > MEOW_PERIOD) {
				meow_sound.Play();
				last_meow = Time.time;
			}
		} else {
			this.happy_indicator.enabled = false;
		}
		
		
	}

	public override string ToString()
	{
		return string.Format("CatStats(Energy={0} Fullness={1} Fun={2} Hygiene={3} Bladder={4} Bond={5} Happieness={6})",
							 energy, fullness, fun, hygiene, bladder, bond, happieness());
	}
	
	public void Save()
	{
		PlayerPrefs.SetFloat("stats.energy", Energy);
		PlayerPrefs.SetFloat("stats.fullness", Fullness);
		PlayerPrefs.SetFloat("stats.fun", Fun);
		PlayerPrefs.SetFloat("stats.hygiene", Hygiene);
		PlayerPrefs.SetFloat("stats.bladder", Bladder);
		PlayerPrefs.SetFloat("stats.bond", Bond);
	}
	
	public static CatStats Load()
	{
		return new CatStats(PlayerPrefs.GetFloat("stats.energy"),
		                    PlayerPrefs.GetFloat("stats.fullness"),
							PlayerPrefs.GetFloat("stats.fun"),
							PlayerPrefs.GetFloat("stats.hygiene"),
							PlayerPrefs.GetFloat("stats.bladder"),
							PlayerPrefs.GetFloat("stats.bond"));
	}
}

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
	
	private bool isBuff; // Is this a stat buff or debuff?
	
	// Visual effect references
	private GameObject greenEffect;
	private GameObject yellowEffect;
	
	// Stat buff numerical value
	private float _value;
	public float Value
	{
		get {return _value;} 
		set 
		{
			if (value < 0) {
				_value = 0;
			}
			else {
				_value = value;
			}
			updateVisualEffects();
		}
	}
	
	public Buff ( bool _isBuff, float _buff_value, GameObject _green_effect = null, GameObject _yellow_effect = null)
	{
		if (_buff_value < 0)
		{
			_value = CatStats.DEFAULT_BUFF_VALUE;
		}
		else 
		{
			_value = _buff_value;
		}
		
		isBuff = _isBuff;
		
		greenEffect = _green_effect;
		yellowEffect = _yellow_effect;
		
		if (!_green_effect || !_yellow_effect)
		{
			Debug.Log("Visual effects missing for buff/debuff.");
		}
		
	}
	
	// Turn visual effects on/off if needed
	public void updateVisualEffects()
	{
		if (isBuff)
		{
			if (_value == CatStats.DEFAULT_BUFF_VALUE)
			{
				if (greenEffect) {greenEffect.SetActive(false);}
				if (yellowEffect) {yellowEffect.SetActive(false);}
			}
			// If stats are increasing slower than usual (bad)...
			else if (_value < CatStats.DEFAULT_BUFF_VALUE) 
			{
				if (greenEffect) {greenEffect.SetActive(false);}
				if (yellowEffect) {yellowEffect.SetActive(true);}
			}
			// If stats are increasing faster than usual (good)...
			else if (_value > CatStats.DEFAULT_BUFF_VALUE)
			{
				if (greenEffect) {greenEffect.SetActive(true);}
				if (yellowEffect) {yellowEffect.SetActive(false);}
			}
		}
		else
		{
			if (_value == CatStats.DEFAULT_BUFF_VALUE)
			{
				if (greenEffect) {greenEffect.SetActive(false);}
				if (yellowEffect) {yellowEffect.SetActive(false);}
			}
			// If stats are decreasing slower than usual (good)...
			else if (_value < CatStats.DEFAULT_BUFF_VALUE)
			{
				if (greenEffect) {greenEffect.SetActive(true);}
				if (yellowEffect) {yellowEffect.SetActive(false);}
			}
			// If stats are decreasing faster than usual (bad)...
			else if (_value > CatStats.DEFAULT_BUFF_VALUE)
			{
				if (greenEffect) {greenEffect.SetActive(false);}
				if (yellowEffect) {yellowEffect.SetActive(true);}
			}
		}
	}
	
}

