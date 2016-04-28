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

	public Text noti;

	public static int latest_errCode = -1;
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
//		Debug.Log (commonLang.langList.Count);
		noti.text = commonLang.GetWord(1);

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
			clientController.Join("com.cspmedia.hiq");
		}

		if (latest_errCode != -1) 
		{
			OnJoinFailed (latest_errCode);
			latest_errCode = -1;
		}
	}
	
	void Update () {
	
		clientController.Run();

		if(qrCodeIsNull == false){
			qrCodeIsNull = true;
			OKPopUp.popUpType = OKPopUp.POPUP_DESTROY;

			CommonUtil.InstantiateOKPopUp(commonLang.GetWord(6));
		}
		if(Application.platform == RuntimePlatform.Android){
			if(Input.GetKeyUp(KeyCode.Escape)){
				YesOrNoPopUp.popUpType = YesOrNoPopUp.APPLICATION_QUIT;
				CommonUtil.InstantiateYesNoPopUP(commonLang.GetWord(15));
			}
		}

		if(isCon = true){
			isCon = false;
			serverConnect.SetActive(false);
			//Debug.Log (commonLang.langList.Count);
			//noti.text = commonLang.langList[9];
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
		
		CommonUtil.InstantiateOKPopUp(commonLang.GetWord(3));	
	}

	void OnDisconnected(){
		OKPopUp.popUpType = OKPopUp.APPLICATION_QUIT;
		CommonUtil.InstantiateOKPopUp(commonLang.GetWord(5));
	}


	void OnJoinSucceeded(bool isHost){
		Debug.Log("OnJoinSucceeded !!!!!! ");
		Application.LoadLevel("ConnectClient_HiQ");
	}

	
	void OnJoinFailed(int err){
		if(err == 10001 || err == 20001){
			OKPopUp.popUpType = OKPopUp.POPUP_DESTROY;
			CommonUtil.InstantiateOKPopUp(commonLang.GetWord(8));
			return;
			
		}else if(err == 10002){
			OKPopUp.popUpType = OKPopUp.POPUP_DESTROY;
			CommonUtil.InstantiateOKPopUp(commonLang.GetWord(14));
			return;
			
		}else if (err == 10003 || err == 20003){
			Debug.Log("Max User");
			OKPopUp.popUpType = OKPopUp.POPUP_DESTROY;
			CommonUtil.InstantiateOKPopUp(commonLang.GetWord(12) );
			return;
		}else if(err == 10004 ||  err == 20004){
			Debug.Log("Already Start");
			OKPopUp.popUpType = OKPopUp.POPUP_DESTROY;;
			CommonUtil.InstantiateOKPopUp(commonLang.GetWord(13) );
			return;
		}
		else {
			OKPopUp.popUpType = OKPopUp.POPUP_DESTROY;
			CommonUtil.InstantiateOKPopUp(commonLang.GetWord(6) );
		}
	}
}