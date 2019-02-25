using System.Collections.Generic;

public enum NodeStatus
{
	Success,
	Failure,
	Running
}

public abstract class Node
{
	// Classes deriving from Node must have a member function "run" which takes a float as a parameter and returns a type NodeStatus
	private abstract NodeStatus run(float time);
	private abstract List<Node> getChildren();
}

