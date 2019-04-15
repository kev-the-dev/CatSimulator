using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordPlayer : MonoBehaviour
{
    //Script for cilcking on record player to pause/play backgroundMusic

    public AudioSource backgroundMusic;
	GameObject particleEffects;
	Text tooltip_text;
 
                          
    void Start()
    {
		tooltip_text = GameObject.Find("RecordPlayerToolTipText").GetComponent<Text>();
		backgroundMusic = GameObject.Find("BackgroundMusic").GetComponent<AudioSource>();
		particleEffects = GameObject.Find("MusicParticles");
    }
	
	public void OnMouseEnter()
	{
		tooltip_text.text = "Play/Pause Music";
	}
	
	public void OnMouseExit()
	{
		tooltip_text.text = "";
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
