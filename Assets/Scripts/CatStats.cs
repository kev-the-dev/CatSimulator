using System;
using UnityEngine;

// Defines the current stats of the cat, which are displayed as bars in the HUD and influence behavior
public class CatStats
{
	public CatStats(float energy = MAX, float fullness = MAX, float fun = MAX, float hygiene = MAX, float bond = MAX)
	{
		this.energy = energy;
		this.fullness = fullness;
		this.fun = fun;
		this.hygiene = hygiene;
		this.bond = bond;
	}

	// Maximum value of each individual stat
	public const float MAX = 1.0F;
	// Minimimu value of each indiviual stat
	public const float MIN = 0.0F;
	

	// TODO: use autoproperties to enforce MIN/MAX
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
	// How connected cat is with the owner
	private float bond;
	public float Bond {get {return bond;} set {bond = Math.Min(MAX, Math.Max(value, MIN));}}

	// The total happieness/contentness of the cat. When == MAX, all desires of cat are satisfied
	public float happieness()
	{
		return (energy + fullness + fun + hygiene) / (4.0F * MAX);
	}
	
	public override string ToString()
	{
		return string.Format("CatStats(Energy={0} Fullness={1} Fun={2} Hygiene={3} Bond={4} Happieness={5})",
							 energy, fullness, fun, hygiene, bond, happieness());
	}
	
	public void Save()
	{
		PlayerPrefs.SetFloat("stats.energy", Energy);
		PlayerPrefs.SetFloat("stats.fullness", Fullness);
		PlayerPrefs.SetFloat("stats.fun", Fun);
		PlayerPrefs.SetFloat("stats.hygiene", Hygiene);
		PlayerPrefs.SetFloat("stats.bond", Bond);
	}
	
	public static CatStats Load()
	{
		return new CatStats(PlayerPrefs.GetFloat("stats.energy"),
		                    PlayerPrefs.GetFloat("stats.fullness"),
							PlayerPrefs.GetFloat("stats.fun"),
							PlayerPrefs.GetFloat("stats.hygiene"),
							PlayerPrefs.GetFloat("stats.bond"));
	}
}