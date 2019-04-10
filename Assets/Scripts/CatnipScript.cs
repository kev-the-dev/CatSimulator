using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatnipScript : MonoBehaviour
{
	Cat catScript; // Reference to cat class attached to Cat gameobject
	public const float CATNIP_TIME_DURATION = 60F; // How long catnip effects will last, in seconds.
	public GameObject UI_Effects;					// Visual effects to call user's attention to catnip
	
    // Start is called before the first frame update
    void Start()
    {
        catScript = GameObject.Find("Cat").GetComponent<Cat>();
		UI_Effects = GameObject.Find("UIEffects");
		
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
