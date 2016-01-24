using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UXLib;

public class LogoutPopUp : MonoBehaviour {
	public static int type = 0;
	UXPlayerLauncherController playerLauncherCotroller;
	// Use this for initialization
	void Start () {
		playerLauncherCotroller = UXPlayerLauncherController.Instance;
		playerLauncherCotroller.OnUserLeft += OnUserLeft;
	}
	void OnDestroy(){
		playerLauncherCotroller.OnUserLeft -= OnUserLeft;
	}
	// Update is called once per frame
	void Update () {
	
	}
	void OnUserLeft ()
	{

		//Application.LoadLevel("1_Login");
	}

	public void OnYesButtonUp(){
		if(type == 1){
			playerLauncherCotroller.Leave();
		//	ControlPadWindow.mainUser = false;
			playerLauncherCotroller.StopAckSender();
		//	return;
		}
		playerLauncherCotroller.StopAckSender();
		Application.Quit();
	}

	public void OnNoButtonUp(){
		Destroy(gameObject);
	}
}
