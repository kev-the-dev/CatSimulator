using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour
{
	LineRenderer laserRenderer;
	public bool poweredOn;
	public Vector3 laserIntersection {get; private set;}
	
    // Start is called before the first frame update
    void Start()
    {
		laserRenderer = GetComponent<LineRenderer>();
        
		// Laser pointer is turned off
		poweredOn = false;
    }

    // Unity will only call Update() if the gameObject is active
    void Update()
    {
		if (poweredOn)
		{
			// Find intersection of cursor and an object
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit))
			{
				// Update laser visualization
				Vector3[] linePoints = {
					Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth / 2, 0, Camera.main.nearClipPlane)),
					hit.point
					};
					
				laserIntersection = hit.point;
			 
				laserRenderer.positionCount = linePoints.Length;
				laserRenderer.SetPositions(linePoints);
			}
		}
		
    }
}
