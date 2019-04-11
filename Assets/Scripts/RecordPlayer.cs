using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordPlayer : MonoBehaviour
{
    //Script for cilcking on record player to pause/play backgroundMusic

    public AudioSource backgroundMusic;
 
                          
    void Start()
    {
		backgroundMusic = GameObject.Find("BackgroundMusic").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    public void OnMouseUp()
    {
		Debug.Log("TOGGLE");
        if (!backgroundMusic.isPlaying)
        {
           backgroundMusic.Play();
        } else {
			backgroundMusic.Stop();
		}
    }
}
