using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RejectButton : MonoBehaviour {
    public void RejectButtonAction()
    {
		Debug.Log("Rejecting");
		GameObject.Find("Cat").GetComponent<BaseCat>().CreateNew();
    }
}