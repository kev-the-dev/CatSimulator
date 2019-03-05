using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class GoToObject : PrimitiveNode
{
	private Context contextObj;
	private GameObject destination {get; set;}
	private Vector3 destinationPosition;
	private NavMeshAgent catAgent;
	private Transform catTransform;
	
	private bool setDestinationResult;
	
	public GoToObject (Context _context, GameObject _destination)
	{
		contextObj = _context;
		
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
	private Context contextObj;
	private Vector3 point {get; set;}
	private NavMeshAgent catAgent;
	private Transform catTransform;
	
	private bool setDestinationResult;
	
	public GoToPoint (Context _context, Vector3 _point )
	{
		contextObj = _context;
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
