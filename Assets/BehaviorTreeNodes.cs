include Node.cs;
using UnityEngine;
using UnityEngine.AI;


// Primitives

public class ActionNode : Node
{
	private delegate NodeStatus ActionFunction(float time);
	private ActionFunction function;
	
	// When ActionNodes are created, they must be passed the function that they will run (as their action). The function must take the current game time as a parameter, and return a value of type NodeStatus
	public ActionNode (ActionFunction _function) 
	{
		function = _function;
	}
	
	public NodeStatus run(float _time)
	{
		return function(_time);
	}
}

// Decorators
public class LoopNode : Node
{
	// ?
	
	private Node child;
	
	public LoopNode()
	{
		child = null;
	}
	
	public NodeStatus run(float _time)
	{
		if (child.run() != NodeStatus.Success) {
			return NodeStatus.Running;
		}
		
		return NodeStatus.Success;
	}
	
	public void addChild(Node _child)
	{
		child = _child;
	}
}

public class WaitNode : Node
{ 
	private float waitTime { get; set; }; // The amount of time (in seconds) that the node should wait
	private float startTime { get; set; }; // The time at which the WaitNode was invoked
	private Node child;
	
	public WaitNode(float _waitTime)
	{
		waitTime = _waitTime;
		child = null;
	}
	
	public NodeStatus run(float _time)
	{
		if (Time.time < startTime + waitTime) {
			return NodeStatus.Running;
		}
		
		return NodeStatus.Success;
	}
	
	public void addChild(Node _node) {
		child = _child;
	}
	
	
}

public class InverterNode : Node
{
	
}

// Composites
public class SequenceNode : Node
{
	
}

public class SelectorNode : Node
{
	
}

public class ParallelNode : Node
{
	
}

