using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



// Primitives:
// Nodes that perform an action or check a condition
public class PrimitiveNode : Node
{
	public delegate NodeStatus PrimitiveFunction(float time);
	PrimitiveFunction function;
	
	// When ActionNodes are created, they must be passed the function that they will run (as their action). The function must take the current game time as a parameter, and return a value of type NodeStatus
	public PrimitiveNode (PrimitiveFunction _function) 
	{
		function = _function;
	}
	
	public override NodeStatus run(float _time)
	{
		return function(_time);
	}
	
	// Primitive Nodes will not have children
	public override List<Node> getChildren()
	{
		return null;
	}
}


// Decorators
public class LoopNode : Node
{
	// I'm guessing a loop node is supposed to repeatedly run its child until it succeeds.
	// LoopNodes will only ever have one child.
	
	PrimitiveNode child;
	
	public LoopNode()
	{
		child = null;
	}
	
	public override NodeStatus run(float _time)
	{
		if (child.run(Time.time) != NodeStatus.Success) {
			return NodeStatus.Running;
		}
		
		return NodeStatus.Success;
	}
	
	public void addChild(PrimitiveNode _child)
	{
		child = _child;
	}
	
	public override List<Node> getChildren()
	{
		List<Node> childrenList = new List<Node>();
		childrenList.Add(child);
		
		return childrenList;
	}
}

public class WaitNode : Node
{ 
	float waitTime { get; set; } // The amount of time (in seconds) that the node should wait
	float startTime; // The time at which the node started waiting
	bool startTimeSet;
	PrimitiveNode child; // WaitNodes will only ever have one child.
	
	public WaitNode(float _waitTime)
	{
		waitTime = _waitTime;
		startTime = 0F;
		startTimeSet = false;
		child = null;
	}
	
	// WaitNode will return Success after waiting x number of seconds.
	public override NodeStatus run(float _time)
	{
		if (startTimeSet == false)
		{
			startTime = Time.time;
			startTimeSet = true;
		}
		
		if (Time.time < startTime + waitTime) 
		{
			return NodeStatus.Running;
		}
		// else...
		
		startTimeSet = false;
		return NodeStatus.Success;
	}
	
	public void addChild(PrimitiveNode _child) 
	{
		child = _child;
	}
	
	public override List<Node> getChildren()
	{
		List<Node> childrenList = new List<Node>();
		childrenList.Add(child);
		
		return childrenList;
	}
	
	
}

public class InverterNode : Node
{
	PrimitiveNode child;
	
	public InverterNode()
	{
		child = null;
	}
	
	public override NodeStatus run(float _time)
	{
		NodeStatus result;
		result = child.run(Time.time);
			
		if (result == NodeStatus.Success) {
			return NodeStatus.Failure;
		}	
		if (result == NodeStatus.Failure) {
			return NodeStatus.Success;
		}
		// If result == NodeStatus.Running...
		return result;
		
	}
	
	public void addChild(PrimitiveNode _child) 
	{
		child = _child;
	}
	
	public override List<Node> getChildren()
	{
		List<Node> childrenList = new List<Node>();
		childrenList.Add(child);
		
		return childrenList;
	}
}

// Composites
/*
public class SequenceNode : Node
{
	
}

public class SelectorNode : Node
{
	
}

public class ParallelNode : Node
{
	
}
*/

