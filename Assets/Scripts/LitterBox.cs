using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LitterBox : MonoBehaviour
{
	GameObject poop;
	Cat catScript;
	
	// Called before Start. 
	void Awake()
	{
		poop = GameObject.Find("Poop");
		catScript = GameObject.Find("Cat").GetComponent<Cat>();
	}
	
    // Start is called before the first frame update
    void Start()
    {
        poop.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void OnMouseDown()
	{
		if (catScript.selected_tool == SelectedTool.LITTER_SCOOPER)
		{
			poop.SetActive(false);
		}

	}
	
}
