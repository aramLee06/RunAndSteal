using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QuitPopUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void OnYesButtonUp(){
		Application.Quit();
	}

	public void OnNoButtonUp(){
		Destroy(gameObject);
	}
}
