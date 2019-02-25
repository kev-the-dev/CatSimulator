
public enum NodeStatus
{
	Success,
	Failure,
	Running
}

public interface Node
{
	// Classes deriving from Node must have a member function "run" which takes a float as a parameter and returns a type NodeStatus
	private NodeStatus run(float time);
}

