using UnityEngine.SceneManagement;
using UnityEngine;

public class AdoptButton : MonoBehaviour {
    public void AdoptButtonAction()
    {
		Debug.Log("Adopting");
		GameObject.Find("Cat").GetComponent<BaseCat>().Save();
		SceneManager.LoadScene("LivingRoom");
    }
}