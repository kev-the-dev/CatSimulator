using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class GoToObject : PrimitiveNode
{
	private GameObject destination {get; set;}
	private Vector3 destinationPosition;
	private NavMeshAgent catAgent;
	private Transform catTransform;
	
	private bool setDestinationResult;
	
	public GoToObject (GameObject _destination)
	{
		destination = _destination;
		destinationPosition = destination.GetComponent<Transform>().position;
		
		catAgent = GetComponent<NavMeshAgent>();
		catTransform = GetComponent<Transform>();
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
	
	private bool setDestinationResult;
	
	public GoToPoint (Vector3 _point)
	{
		point = _point;
		catAgent = GetComponent<NavMeshAgent>();
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
