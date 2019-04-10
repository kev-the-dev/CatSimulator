using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LitterBox : MonoBehaviour
{
	GameObject[] poops;
	Cat catScript;
	
	// Called before Start. 
	void Awake()
	{
		poops = GameObject.FindGameObjectsWithTag("poop");
		catScript = GameObject.Find("Cat").GetComponent<Cat>();
	}
	
    // Start is called before the first frame update
    void Start()
    {
		foreach (GameObject poop in poops)
		{
			poop.SetActive(false);
		}
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void OnMouseDown()
	{
		if (catScript.selected_tool == SelectedTool.LITTER_SCOOPER)
		{
			// Find first active poop
			foreach (GameObject poop in poops)
			{
				if (poop.activeSelf == true)
				{
					catScript.achievements.litter_box_cleaned += 1;
					poop.SetActive(false);
					return;
				}
			}
		}

	}
	
}
