using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attatch this script to the cat GameObject
public class CatAI : MonoBehaviour
{
	BehaviorTree behaviorTree;
	GameObject testObject;
	Context contextObject;
	
    // Start is called before the first frame update
    void Start()
    {
		testObject = GameObject.Find("TestObject");
		contextObject = new Context(gameObject);
		
		
		// Construct the cat's behavior tree
        behaviorTree = new BehaviorTree(
											new SelectorNode( 	contextObject,
																new GoToObject( contextObject, testObject ) 
															)
		
										);
    }

    // Update is called once per frame
    void Update()
    {
        behaviorTree.run(Time.time);
    }
	
}

public class Context
{
	public GameObject parentCat {get;}
	
	public Context (GameObject _parentCat)
	{
		parentCat = _parentCat;
	}
	
	
}
