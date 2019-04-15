
public class CatActivity
{									
	public CatActivityEnum current;
	
	public CatActivity (CatActivityEnum _activity = CatActivityEnum.Idle)
	{
		current = _activity;
	}
	
	public override string ToString()
	{
		return string.Format("CatActivity.currentActivity = {0}", current );
	}
}

public enum CatActivityEnum 
{	
	Idle, 
	Eating, 
	Sleeping, 
	BeingBrushed, 
	BeingPet, 
	FollowingLaser, 
	Playing, 
	UsingLitterbox,
	OnCatnip
}
