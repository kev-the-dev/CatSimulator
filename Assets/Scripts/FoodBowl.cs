using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBowl : MonoBehaviour
{
	GameObject kibble;
	Cat catScript;
	
    // Start is called before the first frame update
    void Start()
    {
        kibble = GameObject.Find("food_in_bowl");
		catScript = GameObject.Find("Cat").GetComponent<Cat>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void OnMouseDown()
	{
		if (catScript.selected_tool == SelectedTool.FOOD)
		{
			kibble.SetActive(true);
		}

	}
}
