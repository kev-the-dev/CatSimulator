using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Generic BehaviorTree for a cat.
// Node inherits from Monobehaviour so our Node classes can have access to Unity functions.
public class BehaviorTree
{
	Node root;
	NodeStatus status;
	
	public BehaviorTree (Node _root)
	{
		root = _root;
	}
	
	// Currently, each call to run() will traverse the entire tree.
	// This is to avoid having to store complex state information in between calls. In the future, if our BT becomes very large, we may want to start saving the state of our BT in between function calls.
	public void run (float _startTime)
	{
		status = root.run(_startTime);
	}
	
}

// Possible return statuses of behavior tree nodes
public enum NodeStatus
{
	Success,
	Failure,
	Running
}
