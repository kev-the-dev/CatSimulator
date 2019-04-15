using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushToyIntoCameraView : MonoBehaviour
{
	Vector3 towardsOrigin;
	
    // Start is called before the first frame update
    void Start()
    {
		
    }
	
	void OnTriggerStay(Collider other_collider)
	{
		//Debug.Log("Entering OnTriggerStay()...");
		
		if (other_collider.CompareTag("toy"))
		{
			towardsOrigin = new Vector3(-1 * other_collider.gameObject.transform.position.x, 0, -1 * other_collider.gameObject.transform.position.z);
			
			// Apply force to move toy back into camera view
			Debug.Log("Applying force.");
			other_collider.gameObject.GetComponent<Rigidbody>().AddForce( towardsOrigin.normalized );
		}
		
		//Debug.Log("Exiting OnTriggerStay.");
	}
	
	
}

