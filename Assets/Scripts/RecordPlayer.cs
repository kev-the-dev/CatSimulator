using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordPlayer : MonoBehaviour
{
    //Script for cilcking on record player to pause/play backgroundMusic

    public AudioSource backgroundMusic;
	GameObject particleEffects;
 
                          
    void Start()
    {
		backgroundMusic = GameObject.Find("BackgroundMusic").GetComponent<AudioSource>();
		particleEffects = GameObject.Find("MusicParticles");
    }

    // Update is called once per frame
    public void OnMouseUp()
    {
		Debug.Log("TOGGLE");
        if (!backgroundMusic.isPlaying)
        {
           backgroundMusic.Play();
		   particleEffects.SetActive(true);
        } else {
			backgroundMusic.Stop();
			particleEffects.SetActive(false);
		}
    }
}
