using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordPlayer : MonoBehaviour
{
    //Script for cilcking on record player and loading a popup screen for changing audio track
    //Add to Gameobject recordPlayer

    public GameObject recordPlayerScreen;
 
                          
    void Start()
    {

    }

    // Update is called once per frame
    private void OnMouseOver()
    {

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Pressed left click.");
            recordPlayerScreen.SetActive(true);
        
        }


    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
           recordPlayerScreen.SetActive(false);
        }

    }


}
