using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// Use this to pass around Unity GameObject related variables to the behavior tree nodes, which do not have direct access to them without a reference (Nodes do not / cannot inherit from Monobehaviour
public class Context
{
	public GameObject parentCat {get; private set;}
	public CatPersonality personality {get; private set;}
	public CatStats stats {get; private set;}
	public CatActivity activity {get; set;}
	
	public Context (GameObject _parentCat, ref CatPersonality _personality, ref CatStats _stats, ref CatActivity _activity)
	{
		parentCat = _parentCat;
		personality = _personality;
		stats = _stats;
		activity = _activity;
		
		Debug.Log("In Context constructor: ");
		Debug.Log(personality);
		Debug.Log(stats);
		Debug.Log(activity);
		Debug.Log("Exiting Context constructor.");
	}
}

// Primitives:
// Nodes that perform an action or check a condition
public class PrimitiveNode : Node
{
	Context contextObj;
	
	public PrimitiveNode ()
	{
		
	}

	public PrimitiveNode (Context _context)
	{
		contextObj = _context;
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
	
	Node child;
	Context contextObj;
	
	public LoopNode( Context _context )
	{
		contextObj = _context;
		child = null;
	}
	
	public LoopNode( Context _context, Node _child )
	{
		contextObj = _context;
		child = _child;
	}
	
	public override NodeStatus run(float _time)
	{
		if (child.run(Time.time) != NodeStatus.Success) {
			return NodeStatus.Running;
		}
		
		return NodeStatus.Success;
	}
	
	public void addChild(Node _child)
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
	Node child; // WaitNodes will only ever have one child.
	Context contextObj;
	
	public WaitNode(Context _context, float _waitTime)
	{
		waitTime = _waitTime;
		startTime = 0F;
		startTimeSet = false;
		child = null;
		contextObj = _context;
	}
	
	public WaitNode(Context _context, float _waitTime, Node _child)
	{
		waitTime = _waitTime;
		startTime = 0F;
		startTimeSet = false;
		child = _child;
		contextObj = _context;
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
	
	public void addChild(Node _child) 
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
	Node child;
	Context contextObj;
	
	public InverterNode( Context _context )
	{
		child = null;
		contextObj = _context;
	}
	
	public InverterNode ( Context _context, Node _child )
	{
		child = _child;
		contextObj = _context;
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
	
	public void addChild(Node _child) 
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

// Runs child nodes in sequence, one after the other, until either a.) all of them have run successfully, in which case the SequenceNode returns Success, or b.) one of the child nodes runs and returns Failure, in which case the SequenceNode also returns failure.
public class SequenceNode : Node
{
	List<Node> children;
	Context contextObj;
	NodeStatus result;
	
	public SequenceNode( Context _context )
	{
		children = new List<Node>();
		contextObj = _context;
	}
	
	// "params" means that this function accepts a variable number of Node objects as its argument. When using this constructor, pass Nodes in a comma separated list.
	public SequenceNode ( Context _context, params Node[] _children )
	{
		children = new List<Node>(_children);
		contextObj = _context;
	}
	
	
	public override NodeStatus run(float time) 
	{
		// If no children, return Failure
		if (children.Count == 0) 
		{
			return NodeStatus.Failure;
		}
		
		foreach (Node child in children)
		{
			result = child.run(Time.time);
			
			if (result == NodeStatus.Failure)
			{
				return NodeStatus.Failure;
			}
			if (result == NodeStatus.Running)
			{
				return NodeStatus.Running;
			}
			
		}
		
		return NodeStatus.Success;
	}
	
	public override List<Node> getChildren()
	{
		return children;
	}
	
	public void addChild(Node _child)
	{
		children.Add(_child);
	}
	
	
}

// Selectors will run each of its children until one of them succeeds, in which case the Selector will return Success. If no children succeed, the Selector will return Failure
public class SelectorNode : Node
{
	List<Node> children;
	Context contextObj;
	NodeStatus result;
	
	public SelectorNode( Context _context )
	{
		children = new List<Node>();
		contextObj = _context;
	}
	
	// "params" means that this function accepts a variable number of Node objects as its argument. When using this constructor, pass Nodes in a comma separated list.
	public SelectorNode ( Context _context, params Node[] _children )
	{
		children = new List<Node>(_children);
		contextObj = _context;
	}
	
	public override NodeStatus run(float _time)
	{
		// If no children, return Failure
		if (children.Count == 0) 
		{
			return NodeStatus.Failure;
		}
		
		foreach (Node child in children)
		{
			result = child.run(Time.time);
			
			if (result == NodeStatus.Success)
			{
				return NodeStatus.Success;
			}
			if (result == NodeStatus.Running)
			{
				return NodeStatus.Running;
			}
		}
		
		return NodeStatus.Failure;
		
	}
	
	public override List<Node> getChildren()
	{
		return children;
	}
	
	public void addChild(Node _child)
	{
		children.Add(_child);
	}
}
/*
// TODO:
// Implement this. Requires the use of threading. 

public class ParallelNode : Node
{
	
}
*/

