using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using UXLib;
using UXLib.User;
using UXLib.UI;
using UXLib.Util;
using DG.Tweening;

public class RoomNumberWindow : MonoBehaviour {
	

	public TouchScreenKeyboardType keyboardType;
	public static bool qrCodeIsNull = true;
	public GameObject qrScannerButton;
	public GameObject numberPad;
	public GameObject eventSystem;
	public GameObject serverConnect;

	bool isCon = false;
	public static string qrString;
	//UXPlayerLauncherController playerLauncherController;
	UXClientController clientController;
	CommonLang commonLang;

	public static int latest_errCode = -1;

	public Text noti;

	void Start () {

		CommonUtil.ScreenSettingsPortrait();
		//numberPad.SetActive(false);

#if UNITY_ANDROID
		qrScannerButton.SetActive(true);
#elif UNITY_IOS
		qrScannerButton.SetActive(false);
#endif
		clientController = UXClientController.Instance;

		commonLang = CommonLang.instance;
		//Debug.Log (commonLang.langList.Count);
		noti.text = commonLang.langList[1];

		clientController.OnConnected += OnConnected;
		clientController.OnConnectFailed += OnConnectFailed;

		clientController.OnJoinSucceeded += OnJoinSucceeded;
		clientController.OnJoinFailed += OnJoinFailed;
		clientController.OnDisconnected += OnDisconnected;

		if(clientController.IsConnected() == false){
			serverConnect.SetActive(true);
			clientController.Connect();
		}
		
		if(string.IsNullOrEmpty(qrString) == false){
			UXConnectController.SetRoomNumber(int.Parse(qrString));
			clientController.Join("com.cspmedia.runandsteal");
		}

		if(latest_errCode != -1)
		{
			OnJoinFailed (latest_errCode);
			isCon = true;
			latest_errCode = -1;
		}
	}
	
	void Update () {
	
		clientController.Run();

		if(qrCodeIsNull == false){
			qrCodeIsNull = true;
			OKPopUp.popUpType = OKPopUp.POPUP_DESTROY;

			CommonUtil.InstantiateOKPopUp(commonLang.langList[6]);
		}
		if(Application.platform == RuntimePlatform.Android){
			if(Input.GetKeyUp(KeyCode.Escape)){
				YesOrNoPopUp.popUpType = YesOrNoPopUp.APPLICATION_QUIT;
				CommonUtil.InstantiateYesNoPopUP(commonLang.langList[15]);
			}
		}

		if(isCon == true){
			isCon = false;
			serverConnect.SetActive(false);
		}

	}

	void OnDestroy() {
		if(clientController != null){
			clientController.OnConnected -= OnConnected;
			clientController.OnConnectFailed -= OnConnectFailed;
			
			clientController.OnJoinSucceeded -= OnJoinSucceeded;
			clientController.OnJoinFailed -= OnJoinFailed;
			clientController.OnDisconnected -= OnDisconnected;
		}
	}

	public void OnQRCodeScannerButtonUp() {
		Application.LoadLevel("QRCodeScanner");
	}

	public void OnConnectButtonUp(){
		eventSystem.SetActive(false);
		numberPad.SetActive(true);
	}

	void OnConnected(){
		isCon = true;

		Debug.Log("OnConnected");
	}
	
	void OnConnectFailed(){
		OKPopUp.popUpType = OKPopUp.APPLICATION_QUIT;
		
		CommonUtil.InstantiateOKPopUp(commonLang.langList[3]);	
	}

	void OnDisconnected(){
		OKPopUp.popUpType = OKPopUp.APPLICATION_QUIT;
		CommonUtil.InstantiateOKPopUp(commonLang.langList[5]);
	}


	void OnJoinSucceeded(bool isHost){
		Debug.Log("OnJoinSucceeded !!!!!! ");
		Application.LoadLevel("LobbyClient");
	}

	
	void OnJoinFailed(int err){
		if(err == 10001 || err == 20003){
			// Invalied Room Number
			OKPopUp.popUpType = OKPopUp.POPUP_DESTROY;
			CommonUtil.InstantiateOKPopUp(commonLang.langList[8]);
			return;
			
		}else if(err == 10002){
			// The User is already connect.
			OKPopUp.popUpType = OKPopUp.POPUP_DESTROY;
			CommonUtil.InstantiateOKPopUp(commonLang.langList[14]);
			return;
			
		}else if (err == 10003 || err == 20001){
			Debug.Log("Max User");
			// Max User
			OKPopUp.popUpType = OKPopUp.POPUP_DESTROY;
			CommonUtil.InstantiateOKPopUp(commonLang.langList[12] );
			return;

		}else if(err == 10004 ||  err == 20002){
			Debug.Log("Already Start");
			// Already Start
			OKPopUp.popUpType = OKPopUp.POPUP_DESTROY;;
			CommonUtil.InstantiateOKPopUp(commonLang.langList[13] );
			return;
		}
		else {
			OKPopUp.popUpType = OKPopUp.POPUP_DESTROY;
			CommonUtil.InstantiateOKPopUp(commonLang.langList[6] );
		}
	}
}
