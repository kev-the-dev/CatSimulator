using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// Node inherits from Monobehaviour so our Node classes can have access to Unity functions.
public class Node : MonoBehaviour
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

