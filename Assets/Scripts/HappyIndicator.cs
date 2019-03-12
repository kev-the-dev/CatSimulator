using UnityEngine;

// Simple script to keep rotation locked to original rotation
public class HappyIndicator : MonoBehaviour
{
	private Quaternion original_rotation;
    // Start is called before the first frame update
    void Start()
    {
        original_rotation = gameObject.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.rotation = original_rotation;
    }
}
