using UnityEngine.SceneManagement;
using UnityEngine;

public class AdoptButton : MonoBehaviour {
    public void AdoptButtonAction()
    {
		Debug.Log("Adopting");
		GameObject.Find("Cat").GetComponent<Cat>().Save();
		SceneManager.LoadScene(1);
    }
}