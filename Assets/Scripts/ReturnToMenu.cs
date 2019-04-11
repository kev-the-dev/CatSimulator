using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour {
    public void ReturnToMenuAction()
    {
		Debug.Log("Foop");
		SceneManager.LoadScene("StartScreen");
    }
}