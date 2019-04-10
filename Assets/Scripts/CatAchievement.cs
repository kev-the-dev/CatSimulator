public class CatAchievement
{
	public CatAchievement(string name)
	{
		this.name = name;
	}

	public virtual bool Unlocked(CatAchievements achievements)
	{
		return false;
	}
	
	public override string ToString()
	{
		return string.Format("CatAchievement(name={0})", name);
	}

	public string name;
}

public class PetAchievement : CatAchievement
{
	public PetAchievement(int threshold) : base(string.Format("Pet cat {0} time(s)", threshold)) {
		this.threshold = threshold;
	}
	public override bool Unlocked(CatAchievements achievements) { return achievements.num_pets >= threshold;}
	private int threshold;
}

public class BrushAchievement : CatAchievement
{
	public BrushAchievement(int threshold) : base(string.Format("Brush cat {0} time(s)", threshold)) {
		this.threshold = threshold;
	}
	public override bool Unlocked(CatAchievements achievements) { return achievements.num_brushes >= threshold;}
	private int threshold;
}

public class MinutesPlayedAchievement : CatAchievement
{
	public MinutesPlayedAchievement(int threshold) : base(string.Format("Played for {0} minutes(s)", threshold)) {
		this.threshold = threshold;
	}
	public override bool Unlocked(CatAchievements achievements) { return (achievements.time_played / 60F) >= threshold;}
	private int threshold;
}

public class LitterBoxCleanedAchievement : CatAchievement
{
	public LitterBoxCleanedAchievement(int threshold) : base(string.Format("Litter box cleaned {0} times", threshold)) {
		this.threshold = threshold;
	}
	public override bool Unlocked(CatAchievements achievements) { return achievements.litter_box_cleaned >= threshold;}
	private int threshold;
}

