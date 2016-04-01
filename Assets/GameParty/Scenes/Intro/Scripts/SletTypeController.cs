using UnityEngine;
using System.Collections;

public class SletTypeController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ScreenMode(){
		Application.LoadLevel ("BS_LogoViewer");
	}

	public void ControllerMode(){
		Application.LoadLevel ("1_Login");
	}
}
