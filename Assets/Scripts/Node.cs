using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// Node inherits from Monobehaviour so our Node classes can have access to Unity functions.
public class Node
{
	protected Context contextObj;
	
	public Node (Context _contextObj)
	{
		contextObj = _contextObj;
	}
	
	public virtual List<Node> getChildren ()
	{
		return null;
	}
	
	public virtual NodeStatus run (float time)
	{
		return NodeStatus.Success;
	}
	
}

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