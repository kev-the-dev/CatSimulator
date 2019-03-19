using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour {
    public void StartGameAction()
    {
		if (GameSave.Valid()) {
			SceneManager.LoadScene(1);
		} else {
			SceneManager.LoadScene(2);
		}
    }
}