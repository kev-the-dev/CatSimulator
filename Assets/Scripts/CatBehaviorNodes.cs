using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Action Nodes 

public class GoToObjectNode : PrimitiveNode
{
	private GameObject destination {get; set;}
	private Vector3 destinationPosition;
	private NavMeshAgent catAgent;
	private Transform catTransform;
	
	private bool setDestinationResult;
	
	public GoToObjectNode (Context _context, GameObject _destination) : base (_context)
	{
		// If condition == false, the debug message is displayed
		Debug.Assert(_destination != null, "GoToObjectNode.GoToObjectNode(): Passed a null destination to the constructor.");
		destination = _destination;
		
		destinationPosition = destination.GetComponent<Transform>().position;
		
		catAgent = contextObj.parentCat.GetComponent<NavMeshAgent>();
		catAgent.stoppingDistance = 2F;
		catTransform = contextObj.parentCat.GetComponent<Transform>();
	}
	
	public override NodeStatus run (float _time)
	{
		// Update destination GameObject's position in case the GameObject has moved
		destinationPosition = destination.GetComponent<Transform>().position;
		
		// If cat is at (or approximately at) destination...
		if ((catTransform.position - destinationPosition).magnitude <= catAgent.stoppingDistance )
		{
			return NodeStatus.Success; 
		}
		else
		{
			// Attempts to set destination. Returns true if the destination was successfully requested. Returns false if the path is still being calculated or if no path exists.
			setDestinationResult = catAgent.SetDestination(destinationPosition);
			Debug.Log("GoToObjectNode.run(): Travelling to destination...");
			Debug.Log("GoToObjectNode.run(): Distance from destination: " + ((catTransform.position - destinationPosition).magnitude).ToString() );
			
			// If the call to SetDestination failed and the path is not still being calculated, return NodeStatus.Failure
			if ( setDestinationResult == false && !(catAgent.pathPending) )
			{
				Debug.Log("GoToObjectNode.run(): Path not found.");
				return NodeStatus.Failure;
			}
			
			return NodeStatus.Running;
			
		}
			
	}
}


public class GoToPointNode : PrimitiveNode
{
	private Vector3 point;
	private NavMeshAgent catAgent;
	private Transform catTransform;
	
	private bool setDestinationResult;
	
	public GoToPointNode (Context _context, Vector3 _point ) : base (_context)
	{
		point = _point;
		catAgent = _context.parentCat.GetComponent<NavMeshAgent>();
		catAgent.stoppingDistance = 2F;
		catTransform = _context.parentCat.GetComponent<Transform>();
	}
	
	public override NodeStatus run (float _time)
	{
		// If cat is at (or approximately at) destination...
		if ((catTransform.position - point).magnitude <= catAgent.stoppingDistance )
		{
			return NodeStatus.Success; 
		}
		else
		{
			// Attempts to set destination. Returns true if the destination was successfully requested. Returns false if the path is still being calculated or if no path exists.
			setDestinationResult = catAgent.SetDestination(point);
			
			Debug.Log("GoToPointNode.run(): Travelling to destination...");
			Debug.Log("GoToPointNode.run(): Distance from destination: " + ((catTransform.position - point).magnitude).ToString() );
			
			// If the call to SetDestination failed and the path is not still being calculated, return NodeStatus.Failure
			if ( setDestinationResult == false && !(catAgent.pathPending) )
			{
				Debug.Log("GoToPointNode.run(): Path not found.");
				return NodeStatus.Failure;
			}
			
			return NodeStatus.Running;
			
		}
	}
	
}

public class SleepNode : PrimitiveNode
{	
	public SleepNode (Context _context) : base (_context)
	{
		
	}
	
	public override NodeStatus run (float _time)
	{
		// If not already sleeping, go to sleep
		if (contextObj.activity.current != CatActivityEnum.Sleeping)
		{
			contextObj.activity.current = CatActivityEnum.Sleeping;
		}
		// If not fully rested, continue sleeping
		if (contextObj.stats.Energy < CatStats.MAX)
		{
			return NodeStatus.Running;
		}
		
		// Make cat wake up
		contextObj.activity.current = CatActivityEnum.Idle;
		return NodeStatus.Success;
	}
}

public class EatNode : PrimitiveNode
{
	GameObject food;
	
	public EatNode (Context _context, GameObject _food) : base (_context)
	{
		food = _food;
	}
	
	public override NodeStatus run (float _time)
	{
		// If not already eating, make cat eating
		if (contextObj.activity.current != CatActivityEnum.Eating)
		{
			contextObj.activity.current = CatActivityEnum.Eating;
		}
		// If not completely full, keep eating
		if (contextObj.stats.Fullness < CatStats.MAX)
		{
			return NodeStatus.Running;
		}
		
		// Make cat stop eating
		food.SetActive(false);
		contextObj.activity.current = CatActivityEnum.Idle;
		return NodeStatus.Success;
	}
	
}

public class FocusOnUserNode : PrimitiveNode
{
	Cat catScript;
	float maxFocusTimespan;
	CameraScript cameraScript;
	
	public FocusOnUserNode (Context _context, float _maxFocusTimespan) : base (_context)
	{
		maxFocusTimespan = _maxFocusTimespan;
		catScript = contextObj.parentCat.GetComponent<Cat>();
		cameraScript = Camera.main.GetComponent<CameraScript>();
	}
	
	public override NodeStatus run (float _time)
	{
		Debug.Log("FocusOnUserNode.run(): Focusing on user...");
		Camera.main.transform.LookAt(contextObj.parentCat.GetComponent<Transform>()); // Main camera look at cat
		
		// If maxFocusTimespan elapses since the last user interaction...
		if ((Time.time - catScript.time_of_last_user_interaction) > maxFocusTimespan)
		{
			// Switch to autonomous cat behaviors
			catScript.turnOnAutonomousCatBehavior();
			
			contextObj.activity.current = CatActivityEnum.Idle;
			cameraScript.Reset();
			
			return NodeStatus.Success;
		}
		
		return NodeStatus.Running;
	}
}


// Condition checking Nodes

public class CheckEnergyNode : PrimitiveNode
{
	float sleepThreshold;
	
	public CheckEnergyNode (Context _context) : base (_context)
	{
		sleepThreshold = contextObj.personality.sleep_threshold;
	}
	
	public CheckEnergyNode ( Context _context, float _custom_sleep_threshold ) : base (_context)
	{
		sleepThreshold = _custom_sleep_threshold;
	}
	
	public override NodeStatus run ( float _time )
	{
		// If already sleeping, return success
		if (contextObj.activity.current == CatActivityEnum.Sleeping)
		{
			return NodeStatus.Success;
		}
		// If not already sleeping, but cat is tired, return success
		else if (contextObj.stats.Energy < sleepThreshold)
		{
			return NodeStatus.Success;
		}
		
		return NodeStatus.Failure;
	}
}

public class CheckFullnessNode : PrimitiveNode
{
	float hungerThreshold;
	
	public CheckFullnessNode (Context _context) : base (_context)
	{
		hungerThreshold = contextObj.personality.hunger_threshold;
	}
	
	public CheckFullnessNode (Context _context, float _custom_fullness_threshold) : base (_context)
	{
		hungerThreshold = _custom_fullness_threshold;
	}
	
	public override NodeStatus run ( float _time )
	{
		// if already eating, return success
		if (contextObj.activity.current == CatActivityEnum.Eating)
		{
			return NodeStatus.Success;
		}
		// if not already eating, but cat is hungry, return success
		else if (contextObj.stats.Fullness < hungerThreshold)
		{
			return NodeStatus.Success;
		}
		
		return NodeStatus.Failure;
	}
}

public class CheckObjectStatusNode : PrimitiveNode
{
	GameObject obj;
	
	public CheckObjectStatusNode (Context _context, GameObject _obj) : base (_context)
	{
		obj = _obj;
	}
	
	public override NodeStatus run (float _time)
	{
		// If the object is activated, return true
		if (obj.activeSelf == true)
		{
			return NodeStatus.Success;
		}
		
		// Otherwise, return failure
		return NodeStatus.Failure;
		
	}
}
