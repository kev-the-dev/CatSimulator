using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatnipScript : MonoBehaviour
{
	Cat catScript; // Reference to cat class attached to Cat gameobject
	Text tooltip_text;
	public const float CATNIP_TIME_DURATION = 60F; // How long catnip effects will last, in seconds.
	public GameObject UI_Effects;					// Visual effects to call user's attention to catnip
	
    // Start is called before the first frame update
    void Start()
    {
		tooltip_text = GameObject.Find("CatnipToolTipText").GetComponent<Text>();
        catScript = GameObject.Find("Cat").GetComponent<Cat>();
		UI_Effects = GameObject.Find("UIEffects");
    }

	public void OnMouseEnter()
	{
		tooltip_text.text = "Feed Catnip";
	}

	public void OnMouseExit()
	{
		tooltip_text.text = "";
	}

	void OnMouseDown()
	{
		Debug.Log("Clicked on catnip.");
		UI_Effects.SetActive(false);		// Turn off visual effects
		
		// If not currently on catnip, use catnip
		if (!catScript.on_catnip)
		{
			StartCoroutine(catScript.useCatnip());
		}
	}
	
}
