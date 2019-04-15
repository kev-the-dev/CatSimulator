using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Action Nodes 

public class GoToObjectNode : PrimitiveNode
{
	private GameObject destination {get; set;}
	private Transform destinationTransform;
	private Vector3 destinationPosition;
	private NavMeshAgent catAgent;
	private Transform catTransform;
	private const float roomRadius = 6F; // If a sphere were placed in the room, what would its radius be? (This is an approximation by me (Alex) )
	
	private bool setDestinationResult;
	
	public GoToObjectNode (Context _context, GameObject _destination) : base (_context)
	{
		// If condition == false, the debug message is displayed
		Debug.Assert(_destination != null, "GoToObjectNode.GoToObjectNode(): Passed a null destination to the constructor.");
		destination = _destination;
		destinationTransform = _destination.GetComponent<Transform>();
		catAgent = contextObj.parentCat.GetComponent<NavMeshAgent>();
		catAgent.stoppingDistance = 2F;
		catTransform = contextObj.parentCat.GetComponent<Transform>();
	}
	
	public override NodeStatus run (float _time)
	{
		// Update destination GameObject's position in case the GameObject has moved
		NavMeshHit hit;
		bool result = NavMesh.SamplePosition(destinationTransform.position, out hit, roomRadius, 1);
		
		if (result) 
		{
			destinationPosition = hit.position;
			//Debug.Log("GoToObjectNode.run(): Found a hit. destinationPosition = " + destinationPosition.ToString() );
		}
		else 
		{
			//Debug.Log("GoToObjectNode.run(): No valid position found within NavMesh.");
			return NodeStatus.Failure;
		}
		
		// If cat is at (or approximately at) destination...
		if ((catTransform.position - destinationPosition).magnitude <= catAgent.stoppingDistance )
		{
			return NodeStatus.Success; 
		}
		else
		{
			// Attempts to set destination. Returns true if the destination was successfully requested. Returns false if the path is still being calculated or if no path exists.
			setDestinationResult = catAgent.SetDestination(destinationPosition);
			//Debug.Log("GoToObjectNode.run(): Travelling to destination...");
			//Debug.Log("GoToObjectNode.run(): Distance from destination: " + ((catTransform.position - destinationPosition).magnitude).ToString() );
			
			// If the call to SetDestination failed and the path is not still being calculated, return NodeStatus.Failure
			if ( setDestinationResult == false && !(catAgent.pathPending) )
			{
				//Debug.Log("GoToObjectNode.run(): Path not found.");
				return NodeStatus.Failure;
			}
			
			return NodeStatus.Running;
			
		}
			
	}
}


public class GoToPointNode : PrimitiveNode
{
	private Vector3 point {get; set;}
	private Vector3 pointOnNavMesh;
	private NavMeshAgent catAgent;
	private Transform catTransform;
	private const float roomRadius = 6F; // If a sphere were placed in the room, what would its radius be? (This is an approximation by me (Alex) )
	
	private bool setDestinationResult;
	
	public GoToPointNode (Context _context, Vector3 _point ) : base (_context)
	{
		point = _point;
		
		catAgent = _context.parentCat.GetComponent<NavMeshAgent>();
		catAgent.stoppingDistance = 2F;
		catTransform = _context.parentCat.GetComponent<Transform>();
		
		NavMeshHit hit;
		bool result = NavMesh.SamplePosition(point, out hit, roomRadius, 1);
		
		if (result) 
		{
			pointOnNavMesh = hit.position;
			//Debug.Log("GoToPointNode(): Found a hit. destinationPosition = " + pointOnNavMesh.ToString() );
		}
		else 
		{
			//Debug.Log("GoToPointNode(): No valid position found within NavMesh.");
		}
	}
	
	public override NodeStatus run (float _time)
	{
		// If cat is at (or approximately at) destination...
		if ((catTransform.position - pointOnNavMesh).magnitude <= catAgent.stoppingDistance )
		{
			return NodeStatus.Success; 
		}
		else
		{
			// Attempts to set destination. Returns true if the destination was successfully requested. Returns false if the path is still being calculated or if no path exists.
			setDestinationResult = catAgent.SetDestination(pointOnNavMesh);
			
			// If the call to SetDestination failed and the path is not still being calculated, return NodeStatus.Failure
			if ( (setDestinationResult == false) && !(catAgent.pathPending) )
			{
				return NodeStatus.Failure;
			}
			
			return NodeStatus.Running;
			
		}
	}
	
}

public delegate Vector3 getPointDelegate();

// Make cat go to a point that changes location during runtime
public class GoToDynamicPointNode : PrimitiveNode
{
	
	getPointDelegate getPoint;	// Delegate for a function that returns the point the cat should go to
	
	private Vector3 point {get; set;}
	private Vector3 pointOnNavMesh;
	private NavMeshAgent catAgent;
	private Transform catTransform;
	private const float roomRadius = 6F; // If a sphere were placed in the room, what would its radius be? (This is an approximation by me (Alex) )
	
	private bool setDestinationResult;
	
	public GoToDynamicPointNode (Context _context, getPointDelegate _del ) : base (_context)
	{
		getPoint = _del;
		
		catAgent = _context.parentCat.GetComponent<NavMeshAgent>();
		catAgent.stoppingDistance = 2F;
		catTransform = _context.parentCat.GetComponent<Transform>();
		
	}
	
	public override NodeStatus run (float _time)
	{
		// Get the point that the cat is supposed to go to
		point = getPoint();
		
		NavMeshHit hit;
		bool result = NavMesh.SamplePosition(point, out hit, roomRadius, 1);
		
		if (result) 
		{
			pointOnNavMesh = hit.position;
			//Debug.Log("GoToDynamicPointNode(): Found a hit. destinationPosition = " + pointOnNavMesh.ToString() );
		}
		else 
		{
			//Debug.Log("GoToDynamicPointNode(): No valid position found within NavMesh.");
		}
		
		// If cat is at (or approximately at) destination...
		if ((catTransform.position - pointOnNavMesh).magnitude <= catAgent.stoppingDistance )
		{
			return NodeStatus.Success; 
		}
		else
		{
			// Attempts to set destination. Returns true if the destination was successfully requested. Returns false if the path is still being calculated or if no path exists.
			setDestinationResult = catAgent.SetDestination(pointOnNavMesh);
			
			// If the call to SetDestination failed and the path is not still being calculated, return NodeStatus.Failure
			if ( (setDestinationResult == false) && !(catAgent.pathPending) )
			{
				return NodeStatus.Failure;
			}
			
			return NodeStatus.Running;
			
		}
	}	
}

public class GoToRandomPointNode : PrimitiveNode
{
	private Vector3 point;
	private bool pointSet; // Has a random point been found yet?
	private NavMeshAgent catAgent;
	private Transform catTransform;
	
	private bool setDestinationResult;
	
	private const float roomRadius = 6F; // If a sphere were placed in the room, what would its radius be? (This is an approximation by me (Alex) )
	
	public GoToRandomPointNode (Context _context) : base (_context)
	{
		point = RandomWaypoint();
		pointSet = true;
		
		catAgent = _context.parentCat.GetComponent<NavMeshAgent>();
		catAgent.stoppingDistance = 2F;
		catTransform = _context.parentCat.GetComponent<Transform>();
	}
	
	public override NodeStatus run (float _time)
	{
		if (pointSet)
		{	// If cat is at (or approximately at) the destination point, return Success.
			if ((catTransform.position - point).magnitude <= catAgent.stoppingDistance)
			{
				// Reset variables
				pointSet = false;
				
				return NodeStatus.Success; 
			}		
		}
		else {
			point = RandomWaypoint();
			pointSet = true;
			//Debug.Log("GoToRandomPointNode.run(): Destination is: " + point.ToString() );
		}
		
		// Attempts to set destination. Returns true if the destination was successfully requested. Returns false if the path is still being calculated or if no path exists.
		setDestinationResult = catAgent.SetDestination(point);
		
		//Debug.Log("GoToRandomPointNode.run(): Travelling to destination...");
		//Debug.Log("GoToRandomPointNode.run(): Distance from destination: " + ((catTransform.position - point).magnitude).ToString() );
		
		// If the call to SetDestination failed and the path is not still being calculated, return NodeStatus.Failure
		if ( (setDestinationResult == false) && !(catAgent.pathPending) )
		{
			//Debug.Log("GoToObjectNode.run(): Path not found.");
			return NodeStatus.Failure;
		}
		
		return NodeStatus.Running;

	}
	
	Vector3 RandomWaypoint()
	{
		// Reference: https://answers.unity.com/questions/475066/how-to-get-a-random-point-on-navmesh.html
		
		Vector3 random = UnityEngine.Random.insideUnitSphere * roomRadius;
		
		NavMeshHit hit;
		Vector3 randomPoint;
		
		bool result = NavMesh.SamplePosition(random, out hit, roomRadius, 1);
		
		if (result) 
		{
			randomPoint = hit.position;
		}
		else
		{
			randomPoint = Vector3.zero;
		}
		
		return randomPoint;
	}
	
}

public class SleepNode : PrimitiveNode
{
	GameObject particles;

	public SleepNode (Context _context, GameObject _particles) : base (_context)
	{
		particles = _particles;
		particles.SetActive(false);
	}
	
	public override NodeStatus run (float _time)
	{
		// If not already sleeping, go to sleep
		if (contextObj.activity.current != CatActivityEnum.Sleeping)
		{
			particles.SetActive(true);
			contextObj.activity.current = CatActivityEnum.Sleeping;
		}
		// If not fully rested, continue sleeping
		if (contextObj.stats.Energy < CatStats.MAX)
		{
			return NodeStatus.Running;
		}
		
		// Make cat wake up
		particles.SetActive(false);
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

public class RelieveBladderNode : PrimitiveNode
{
	GameObject[] poops;
	int numberOfPoops;
	
	public RelieveBladderNode (Context _context) : base (_context)
	{
		poops = GameObject.FindGameObjectsWithTag("poop");	// Returns an array of all GameObjects with the tag "poop"
		numberOfPoops = poops.Length; // Total number of elements in the array
		
		if (numberOfPoops == 0)
		{
			Debug.Log("RelieveBladderNode(): No poops found.");
		}
	}
	
	public override NodeStatus run (float _time)
	{
		// If not already relieving self, make cat relieve self
		if (contextObj.activity.current != CatActivityEnum.UsingLitterbox)
		{
			contextObj.activity.current = CatActivityEnum.UsingLitterbox;
		}
		// Check bladder static
		if (contextObj.stats.Bladder < CatStats.MAX)
		{
			return NodeStatus.Running;
		}
		
		// Make cat stop relieving itself
		
		// Find first deactivated poop and activate it
		foreach (GameObject poop in poops)
		{
			if (poop.activeSelf == false) 
			{
				poop.SetActive(true);
				contextObj.activity.current = CatActivityEnum.Idle;
				return NodeStatus.Success;
			}
		}
		
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
	}
	
	public override NodeStatus run (float _time)
	{
		//Debug.Log("FocusOnUserNode.run(): Focusing on user...");
		
		// If maxFocusTimespan elapses since the last user interaction...
		if ((Time.time - catScript.time_of_last_user_interaction) > maxFocusTimespan)
		{
			// Switch to autonomous cat behaviors
			catScript.turnOnAutonomousCatBehavior();
			
			contextObj.activity.current = CatActivityEnum.Idle;
			
			return NodeStatus.Success;
		}
		
		return NodeStatus.Running;
	}
}

public class PlayNode : PrimitiveNode
{
	public PlayNode (Context _context) : base (_context)
	{
		
	}
	
	public override NodeStatus run (float _time)
	{
		contextObj.activity.current = CatActivityEnum.Playing;
		return NodeStatus.Success;
	}
}

public class IdleNode : PrimitiveNode
{
	public IdleNode (Context _context) : base (_context)
	{
		
	}
	
	public override NodeStatus run (float _time)
	{
		contextObj.activity.current = CatActivityEnum.Idle;
		return NodeStatus.Success;
	}
}

public class ChaseLaserNode : PrimitiveNode
{
	public ChaseLaserNode (Context _context) : base (_context)
	{
		
	}
	
	public override NodeStatus run (float _time)
	{
		contextObj.activity.current = CatActivityEnum.FollowingLaser;
		return NodeStatus.Success;
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

public class CheckBladderNode : PrimitiveNode
{
	float bladderThreshold;
	
	public CheckBladderNode (Context _context) : base (_context)
	{
		bladderThreshold = contextObj.personality.bladder_threshold;
	}
	
	public CheckBladderNode (Context _context, float _custom_bladder_threshold) : base (_context)
	{
		bladderThreshold = _custom_bladder_threshold;
	}
	
	public override NodeStatus run ( float _time )
	{
		// if already using litter box, return success
		if (contextObj.activity.current == CatActivityEnum.UsingLitterbox)
		{
			return NodeStatus.Success;
		}
		// If not already using litter box, but cat needs to use the litter box, return Success
		else if (contextObj.stats.Bladder < bladderThreshold)
		{
			return NodeStatus.Success;
		}
		
		return NodeStatus.Failure;
	}
	
}

public class CheckFunNode : PrimitiveNode
{
	float funThreshold;
	
	public CheckFunNode (Context _context) : base (_context)
	{
		funThreshold = contextObj.personality.fun_threshold;
	}
	
	public CheckFunNode (Context _context, float _custom_fun_threshold) : base (_context)
	{
		funThreshold = _custom_fun_threshold;
	}
	
	public override NodeStatus run ( float _time )
	{
		// if already playing
		if (contextObj.activity.current == CatActivityEnum.Playing)
		{
			return NodeStatus.Success;
		}
		// If not already playing, but cat is bored, return Success
		else if (contextObj.stats.Fun < funThreshold)
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
