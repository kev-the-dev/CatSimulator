using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LitterBox : MonoBehaviour
{
	GameObject poop;
	Cat catScript;
	
    // Start is called before the first frame update
    void Start()
    {
        poop = GameObject.Find("Poop");
		catScript = GameObject.Find("Cat").GetComponent<Cat>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	/*
	void OnMouseDown()
	{
		if (catScript.selected_tool == SelectedTool.LITTERBOXSCOOPER)
		{
			poop.SetActive(true);
		}

	}
	*/
}
