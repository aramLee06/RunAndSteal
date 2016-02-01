using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UXLib;
using UXLib.UI;
using UXLib.User;
using UXLib.Util;
using System.Timers;


public class LoginWindow : MonoBehaviour {
	public GameObject logoImage;
	public GameObject chineseButton;
	public GameObject englishButton;

	public Text infoText;
	public Text stateText;

	UXPlayerController playerController;
	UXAndroidManager androidManager;

	CommonLang mCommonLang;

	IEnumerator Start () {

		#if UNITY_ANDROID && !UNITY_EDITOR
		GameObject go = GameObject.Find ("AndroidManager");
		androidManager = go.GetComponent<UXAndroidManager> ();
		androidManager.InitAndroid ();

		#endif
		//CommonUtil.ScreenSettingsPortrait();

		if(UXPlayerLauncherController.Instance.IsConnected() == true){
			if(UXConnectController.GetRoomNumber() != -1){
				UXConnectController.SetRoomNumber(-1);
			}
			UXPlayerLauncherController.Instance.Clear();
		}

		playerController = UXPlayerController.Instance;

		yield return CommonLang.instance;

		//chineseButton.SetActive(true);
		//englishButton.SetActive(true);
		logoImage.SetActive(true);
		//ChinaButton ();
		EnglishButton();

		#if UNITY_ANDROID && !UNITY_EDITOR
		infoText.text = androidManager.GetVersionName("com.cspmedia.gamepartyplayer") + "/" + ((ServerList)PlayerPrefs.GetInt("ServerList"));
		#endif

	}

	public void ChinaButton(){
		CommonLang.instance.SeleteLanguage("chi");
		Login();
	}

	public void EnglishButton(){

		CommonLang.instance.SeleteLanguage("eng");
		Login();

	} 

	void Login(){
		//logoImage.SetActive(true);
		//chineseButton.SetActive(false);
		//englishButton.SetActive(false);

		int result = 100;

		Debug.Log (CommonLang.instance.langList.Count);
		//stateText.text = CommonLang.instance.langList[0];
		result = playerController.GetUserCodeFromServer();
		if(result == -1 || result == 100 || result == 0){
			
			OKPopUp.popUpType = OKPopUp.APPLICATION_QUIT;

			CommonUtil.InstantiateOKPopUp(CommonLang.instance.langList[4]);
			return;
		}else if(result == 1){
			Application.LoadLevel ("2_RoomNumber");
		}
	}
}
