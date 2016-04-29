using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UXLib;

public class Setting_Space : MonoBehaviour {
	
	
	
	public GameObject panel;
	public GameObject chinaLang;
	public GameObject englishLang;
	
	public Text serverState;
	public Text version;
	
	RectTransform setting;
	
	public const string CHINA = "chi";
	public const string ENGLISH = "eng";
	public const string LANGUAGE = "Language";
	
	bool isOpen = false;
	
	UXAndroidManager androidManager;
	
	public float settingOpenWidth;
	public float settingCloseWidth;
	
	
	public Text textobj_player;
	public Text textobj_store;
	public Text textobj_popup;
	
	
	
	void Awake(){
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		GameObject go = GameObject.Find ("AndroidManager");
		androidManager = go.GetComponent<UXAndroidManager> ();
		androidManager.InitAndroid ();
		#endif
	}

	void Start () {
		setting = panel.GetComponent<RectTransform>() as RectTransform;
		
		Debug.Log ("START : " +(ServerList)PlayerPrefs.GetInt("ServerList"));
		if (((ServerList)PlayerPrefs.GetInt ("ServerList")) == ServerList.CN) {
			serverState.text = "China";
		} else if(((ServerList)PlayerPrefs.GetInt ("ServerList")) == ServerList.SG) {
			serverState.text = "Singapore";
		}
		
		if(PlayerPrefs.GetString(LANGUAGE) == CHINA){
			chinaLang.SetActive(true);
			englishLang.SetActive(false);
			
			
			if(settingCloseWidth != 637)
			{
				textobj_player.text = "请下载Game party player";
				textobj_store.text = "请输入房间号码还是拍QR码";
				textobj_popup.text = "开始游戏至少要有两个人";
			}
		}else {
			chinaLang.SetActive(false);
			englishLang.SetActive(true);
			
			
			if(settingCloseWidth != 637)
			{
				textobj_player.text = "Please download the GamePartyPlayer";
				textobj_store.text = "Enter your room number or Scan QR code";
				textobj_popup.text = "Can start the game when\nthere are 2 players or more";
			}
		}
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		version.text = "ver " + androidManager.GetVersionName ("com.cspmedia.gamepartyplayer");
		#endif
	}
	
	void Update () {
		if(isOpen){
			if(Input.GetMouseButtonDown(0)) {
				Debug.Log ("Down ");
				PointerEventData pointer = new PointerEventData(EventSystem.current);
				pointer.position = Input.mousePosition;
				
				List<RaycastResult> raycastResults = new List<RaycastResult>();
				EventSystem.current.RaycastAll(pointer, raycastResults);
				
				if(raycastResults.Count > 0) {
					
					GameObject obj = raycastResults[0].gameObject;
					Debug.Log ( obj.transform.parent);
					
					Transform[] childObj =  gameObject.GetComponentsInChildren<Transform>();
					
					int count = 0;
					
					foreach(Transform tr in childObj){
						if(tr.name == obj.name){
							count ++;
						}
					}					
					if(count == 0)
						setting.transform.DOLocalMoveX(settingCloseWidth,0.5f);
				}
			}
		}
	}
	
	public void SettingOpen(){
		if(isOpen == true){
			isOpen = false;
			setting.transform.DOLocalMoveX(settingCloseWidth,0.5f);
			return;
		}
		isOpen = true;
		setting.transform.DOLocalMoveX(settingOpenWidth,0.5f);
	}
	
	public void LangButton(){
		
		if (chinaLang.activeSelf == true) {
			PlayerPrefs.SetString(LANGUAGE, ENGLISH);
			
			if(settingCloseWidth != 637)
			{
				textobj_player.text = "Please download the GamePartyPlayer";
				textobj_store.text = "Enter your room number or Scan QR code";
				textobj_popup.text = "Can start the game when\nthere are 2 players or more";
			}
			
			chinaLang.SetActive (false);
			englishLang.SetActive (true);
		} else {
			PlayerPrefs.SetString(LANGUAGE, CHINA);
			
			if(settingCloseWidth != 637)
			{
				textobj_player.text = "请下载Game party player";
				textobj_store.text = "请输入房间号码还是拍QR码";
				textobj_popup.text = "开始游戏至少要有两个人";
			}
			
			chinaLang.SetActive (true);
			englishLang.SetActive (false);
		}
	}
}