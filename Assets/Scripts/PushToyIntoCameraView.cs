using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushToyIntoCameraView : MonoBehaviour
{
	Vector3 towardsOrigin;
	Vector3 frontOfRoomPosition;
	GameObject[] toys;
	
    // Start is called before the first frame update
    void Start()
    {
		toys = GameObject.FindGameObjectsWithTag("toy");
		frontOfRoomPosition = new Vector3(0F, -2.576599F, -9.8F);
    }
	
	void OnTriggerStay(Collider other_collider)
	{
		//Debug.Log("Entering OnTriggerStay()...");
		
		if (other_collider.CompareTag("toy"))
		{
			towardsOrigin = new Vector3(-1 * other_collider.gameObject.transform.position.x, 0, -1.1F * other_collider.gameObject.transform.position.z);
			
			// Apply force to move toy back into camera view
			Debug.Log("Applying force.");
			other_collider.gameObject.GetComponent<Rigidbody>().AddForce( towardsOrigin.normalized );
		}
		
		//Debug.Log("Exiting OnTriggerStay.");
	}
	
	
}

