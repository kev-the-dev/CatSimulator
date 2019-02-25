include Node.cs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// Primitives
public class ActionNode : Node
{
	private delegate NodeStatus ActionFunction(float time);
	private virtual ActionFunction function; // "virtual" keyword means subclasses of ActionNode can overwrite this member variable and provide it with a different definition (for example, a delegate that requires a different number of function arguments)
	
	// When ActionNodes are created, they must be passed the function that they will run (as their action). The function must take the current game time as a parameter, and return a value of type NodeStatus
	private ActionNode (ActionFunction _function) 
	{
		function = _function;
	}
	
	private override NodeStatus run(float _time)
	{
		return function(_time);
	}
	
	// ActionNodes will never have children
	private override List<Node> getChildren()
	{
		return null;
	}
}

// Decorators
public class LoopNode : Node
{
	// I'm guessing a loop node is supposed to repeatedly run its child until it succeeds.
	// LoopNodes will only ever have one child.
	
	private List<Node> children;
	
	public LoopNode()
	{
		children = new List<Node>();
	}
	
	public override NodeStatus run(float _time)
	{
		if (child.run() != NodeStatus.Success) {
			return NodeStatus.Running;
		}
		
		return NodeStatus.Success;
	}
	
	// Remove existing child (if one exists) and add new child
	public void addChild(Node _child)
	{
		children.Clear();
		children.Add(_child);
	}
	
	public override List<Node> getChildren()
	{
		return children;
	}
}

public class WaitNode : Node
{ 
	private float waitTime { get; set; }; // The amount of time (in seconds) that the node should wait
	private float startTime { get; set; }; // The time at which the WaitNode was invoked
	private List<Node> children;
	
	public WaitNode(float _waitTime)
	{
		waitTime = _waitTime;
		startTime = 0F;
		children = new List<Node>();
	}
	
	public override NodeStatus run(float _time)
	{
		if (Time.time < startTime + waitTime) {
			return NodeStatus.Running;
		}
		
		return NodeStatus.Success;
	}
	
	// Remove existing child (if one exists) and add new child
	public void addChild(Node _child) 
	{
		children.Clear();
		children.Add(_child);
	}
	
	
}

public class InverterNode : Node
{
	private List<Node> children;
	
	public InverterNode()
	{
		children = new List<Node>();
	}
	
	public override NodeStatus run(float _time)
	{
		NodeStatus result;
		
		// If InverterNode has one (and only one) child...
		if (children.Count == 1) {
			result = children[0].run(Time.time);
			
			if (result == NodeStatus.Success) {
				return NodeStatus.Failure;
			}	
			if (result == NodeStatus.Failure) {
				return NodeStatus.Success;
			}
			// If result == NodeStatus.Running...
			return result;			
		}
		// Throw error because Inverter Node has no child(ren) / more than one child
		else {
			throw new System.InvalidOperationException("Error in InverterNode.run(): InverterNodes must have one and only one child.");
		}
		
	}
	
	// Remove existing child (if one exists) and add new child
	public void addChild(Node _child) 
	{
		children.Clear();
		children.Add(_child);
	}
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

