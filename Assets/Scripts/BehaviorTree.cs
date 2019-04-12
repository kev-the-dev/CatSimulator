using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Generic BehaviorTree for a cat.
// Node inherits from Monobehaviour so our Node classes can have access to Unity functions.
public class BehaviorTree
{
	Node root;
	NodeStatus status;
	public bool paused; // Pause running the behavior tree (in the case of user interaction, etc., so cat will not wander off)
	
	public BehaviorTree (Node _root)
	{
		root = _root;
		paused = false;
	}
	
	public void run(float _startTime)
	{
		// Do not run if tree is paused
		if (!paused)
		{
			// Traverse tree
			status = root.run(_startTime);
		}
	}
	
}



// Possible return statuses of behavior tree nodes
public enum NodeStatus
{
	Success,
	Failure,
	Running
}
