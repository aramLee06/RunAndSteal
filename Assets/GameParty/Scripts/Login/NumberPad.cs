using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System.Text.RegularExpressions;
using UXLib;
using UXLib.User;
using UXLib.Util;

public class NumberPad : MonoBehaviour {

	//private string roomNumber = null;
	private bool numberPadState = false;
	private string inputNumber = null;

	public GameObject eventSystem;
	string packageName;

	public Text[] test;

	//UXPlayerLauncherController playerLauncherController;
	UXClientController clientController;
	CommonLang commonLang;

	public static bool bStore = false;
	// Use this for initialization
	void Start () {
		CommonUtil.ScreenSettingsPortrait();
		commonLang = CommonLang.instance;
		clientController = UXClientController.Instance;
	}

	void Update () {
		if(numberPadState == true){
			numberPadState = false;
			commonLang = CommonLang.instance;
			gameObject.transform.DOLocalMoveY(0.0f,0.3f).OnComplete(EventSystemEnable);
		}
	}

	void EventSystemEnable(){
		eventSystem.SetActive(true);
	}

	public void SetRoomNumber(string numberType) {		

		if(numberType == "del") {
			if(inputNumber == null || inputNumber.Length == 0){
				return;
			} else {
				inputNumber = inputNumber.Substring(0,inputNumber.Length-1);
				test[inputNumber.Length].text = "";
				return;
			}
		} else if (numberType == "done") {
			if(string.IsNullOrEmpty(inputNumber) == true){
				OKPopUp.popUpType = OKPopUp.POPUP_DESTROY;
				CommonUtil.InstantiateOKPopUp(commonLang.langList[7] );
				return;
			}
			bool numberCheck = Regex.IsMatch(inputNumber, @"[0-9]{5}$");
			
			if(numberCheck == true){

				UXConnectController.SetRoomNumber(int.Parse (inputNumber));
				UXRoom.Instance.RoomNumber = inputNumber;
				// clientController.Join("none");
				Application.LoadLevel("LobbyClient");
				
				return;
			} else {
				OKPopUp.popUpType = OKPopUp.POPUP_DESTROY;
				CommonUtil.InstantiateOKPopUp(commonLang.langList[8]);
				return;
			}
		}	

		if(inputNumber != null){
			if(inputNumber.Length == 5){
				Debug.Log("return");
				return;
			}
		}
	
		inputNumber += numberType;
		for(int i = 0; i < inputNumber.Length; i++){
			test[i].text = inputNumber[i].ToString();
		}
		
	}
	public void NumberPadQuitButton(){
		numberPadState = false;
		gameObject.transform.DOPause();
		gameObject.transform.DOLocalMoveY(-1300.0f,0.3f).OnComplete(QuitButton);
	}

	void QuitButton(){
		gameObject.SetActive(false);
	}

	void OnEnable() {
		if(string.IsNullOrEmpty(inputNumber) == false){
			for(int i = 0; i < inputNumber.Length; i++){
				test[i].text = "";
			}
			inputNumber = "";
		}
		numberPadState = true;
	}

}
