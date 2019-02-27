using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attatch this script to the cat GameObject
public class CatAI : MonoBehaviour
{
	BehaviorTree behaviorTree;
	
    // Start is called before the first frame update
    void Start()
    {
        behaviorTree = new BehaviorTree();
    }

    // Update is called once per frame
    void Update()
    {
        behaviorTree.run(Time.time);
    }
}


