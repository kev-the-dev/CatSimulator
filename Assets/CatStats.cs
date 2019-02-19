// Defines the current stats of the cat, which are displayed as bars in the HUD and influence behavior
public class CatStats
{
	public CatStats()
	{
		energy = MAX;
		fullness = MAX;
		fun = MAX;
		cleanness = MAX;
		bond = MIN;
	}

	// Maximum value of each individual stat
	public const float MAX = 1.0F;
	// Minimimu value of each indiviual stat
	public const float MIN = 0.0F;

	// How energetic the cat is, when maximized cat has no desire for sleep
	public float energy;
	// Inverse of hunger, when maximized cat has no desire for food
	public float fullness;
	// How much fun cat is having, when maximized cat is having, like, a lot of fun
	public float fun;
	// How clean the cat and its environment is. When maximized, cat is content with its hygiene
	public float cleanness;
	// How connected cat is with the owner
	public float bond;

	// The total happieness/contentness of the cat. When == MAX, all desires of cat are satisfied
	public float happieness()
	{
		return (energy + fullness + fun + cleanness) / (4.0F * MAX);
	}
	
	public override string ToString()
	{
		return string.Format("CatStats(Energy={0} Fullness={1} Fun={2} Cleanness={3} Bond={4} Happieness={5})",
							 energy, fullness, fun, cleanness, bond, happieness());
	}
}