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
	
	public void run (float _startTime)
	{
		// TODO: implement stack-based iterative preorder traversal
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
