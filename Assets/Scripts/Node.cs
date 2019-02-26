using System.Collections.Generic;

public enum NodeStatus
{
	Success,
	Failure,
	Running
}

public class Node
{
	public virtual List<Node> getChildren ()
	{
		return null;
	}
	
	public virtual NodeStatus run (float time)
	{
		return NodeStatus.Success;
	}
	
}

