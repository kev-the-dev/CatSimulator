using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
	
	Vector3 defaultCameraPosition;
	Vector3 defaultCameraRotation;
	Transform cameraTransform;
	
    // Start is called before the first frame update
    void Start()
    {
		cameraTransform = GetComponent<Transform>();
        defaultCameraPosition = cameraTransform.position;
		defaultCameraRotation = cameraTransform.eulerAngles;
    }
	
	public void Reset()
	{
		cameraTransform.position = defaultCameraPosition;
		
		cameraTransform.eulerAngles = defaultCameraRotation;
	}
}
