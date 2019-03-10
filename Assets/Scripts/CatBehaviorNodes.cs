using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Action Nodes 

public class GoToObject : PrimitiveNode
{
	private GameObject destination {get; set;}
	private Vector3 destinationPosition;
	private NavMeshAgent catAgent;
	private Transform catTransform;
	
	private bool setDestinationResult;
	
	public GoToObject (Context _context, GameObject _destination) : base (_context)
	{
		// If condition == false, the debug message is displayed
		Debug.Assert(_destination != null, "GoToObject.GotoObject(): Passed a null destination to the constructor.");
		destination = _destination;
		
		destinationPosition = destination.GetComponent<Transform>().position;
		
		catAgent = contextObj.parentCat.GetComponent<NavMeshAgent>();
		catTransform = contextObj.parentCat.GetComponent<Transform>();
	}
	
	public override NodeStatus run (float _time)
	{
		// Update destination GameObject's position in case the GameObject has moved
		destinationPosition = destination.GetComponent<Transform>().position;
		
		if (catTransform.position == destinationPosition)
		{
			return NodeStatus.Success; 
		}
		else
		{
			// Attempts to set destination. Returns true if the destination was successfully requested. Returns false if the path is still being calculated or if no path exists.
			setDestinationResult = catAgent.SetDestination(destinationPosition);
			
			// If the call to SetDestination failed and the path is not still being calculated, return NodeStatus.Failure
			if ( setDestinationResult == false && !(catAgent.pathPending) )
			{
				return NodeStatus.Failure;
			}
			
			return NodeStatus.Running;
			
		}
			
	}
}

public class GoToPoint : PrimitiveNode
{
	private Vector3 point {get; set;}
	private NavMeshAgent catAgent;
	private Transform catTransform;
	
	private bool setDestinationResult;
	
	public GoToPoint (Context _context, Vector3 _point ) : base (_context)
	{
		point = _point;
		catAgent = _context.parentCat.GetComponent<NavMeshAgent>();
		catTransform = _context.parentCat.GetComponent<Transform>();
	}
	
	public override NodeStatus run (float _time)
	{
		if (catTransform.position == point)
		{
			return NodeStatus.Success; 
		}
		else
		{
			// Attempts to set destination. Returns true if the destination was successfully requested. Returns false if the path is still being calculated or if no path exists.
			setDestinationResult = catAgent.SetDestination(point);
			
			// If the call to SetDestination failed and the path is not still being calculated, return NodeStatus.Failure
			if ( setDestinationResult == false && !(catAgent.pathPending) )
			{
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

public class FocusOnUserNode : PrimitiveNode
{
	Cat catScript;
	float maxFocusTimespan;
	float timeOfLastUserInteraction;
	bool timerInitiated;
	
	public FocusOnUserNode (Context _context, float _maxFocusTimespan) : base (_context)
	{
		maxFocusTimespan = _maxFocusTimespan;
		timerInitiated = false;
		
		catScript = contextObj.parentCat.getComponent<Cat>();
	}
	
	public override NodeStatus run ()
	{
		if (!timerInitiated)
		{
			timeOfLastUserInteraction = catScript.time_of_last_user_interaction;
			timerInitiated = true;
		}
		
		if ((Time.time - timeOfLastUserInteraction) > maxFocusTimespan)
		{
			
		}
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
