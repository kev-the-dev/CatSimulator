using System.Collections.Generic;

public enum NodeStatus
{
	Success,
	Failure,
	Running
}

abstract public class Node
{
	abstract public List<Node> getChildren ();
	abstract public NodeStatus run (float time);
	
}

