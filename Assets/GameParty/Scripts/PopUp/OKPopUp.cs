using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UXLib.Util;
using UXLib;

public class OKPopUp : MonoBehaviour {


	public static int popUpType = -1;

	public const int APPLICATION_QUIT = 0;
	public const int POPUP_DESTROY = 1;

	public Text title;
	public Text msg;

	// Use this for initialization
	void Start () {

	}

	public void OnOkButtonMouseUp(){
		if(popUpType == POPUP_DESTROY){
			title.text = "";
			msg.text = "";
			Destroy (gameObject);
			popUpType = -1;
		
			return;
		}else if(popUpType == APPLICATION_QUIT){
		//	UXPlayerLauncherController.Instance.Clear();		
			Application.Quit();
		}
	}
}
