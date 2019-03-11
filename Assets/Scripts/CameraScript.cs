using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
	
	Vector3 defaultCameraPosition;
	Quaternion defaultCameraRotation;
	Transform cameraTransform;
	
    // Start is called before the first frame update
    void Start()
    {
		cameraTransform = GetComponent<Transform>();
        defaultCameraPosition = cameraTransform.position;
		defaultCameraRotation = cameraTransform.rotation;
    }
	
	public void Reset()
	{
		cameraTransform.position = defaultCameraPosition;
		
		cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, defaultCameraRotation, 0.5F);
	}
}
