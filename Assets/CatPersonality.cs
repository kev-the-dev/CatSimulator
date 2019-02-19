using UnityEngine;

public class CatPersonality
{

	public CatPersonality(float hungriness, float tierdness, float playfullness, float sociability)
	{
		this.hungriness = hungriness;
		this.tierdness = tierdness;
		this.playfullness = playfullness;
		this.sociability = sociability;
	}

	// Maximum value a personality trait can have
	public const float MAX = 1.0F;
	public const float MIN = 0.0F;
	
	public static CatPersonality RandomPersonality()
	{
		CatPersonality new_personality = new CatPersonality(Random.Range(MIN, MAX), Random.Range(MIN, MAX), Random.Range(MIN, MAX), Random.Range(MIN, MAX));
		return new_personality;
	}

	// How quickly cat gets hungry, how much food is needed to satisfy it
	public float hungriness;
	// How quickly cat gets tierd, how much sleep is needed to satisfy it
	public float tierdness;
	// 
	public float playfullness;
	public float sociability;
	
	public override string ToString()
	{
		return string.Format("CatPersonality(hungriness={0}, tierdness={1}, playfullness={2}, sociability={3})",
							 hungriness, tierdness, playfullness, sociability);
	}
}