using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UXLib;
using UXLib.Connect;
using UXLib.User;
using UXLib.Base;
using UXLib.Util;
using UXLib.UI;

public class LobbyClient : MonoBehaviour
{

	// TODO :  게임 기본 패키지 이름, 게임별로 맞게 변경 필요함
	public static string GAME_PACKAGE_NAME = "com.cspmedia.runandsteal";

	//int userCode;
	//public static int launcherCode;

	int userCode; // 접속용 유저 코드, SDK 1.5에서 삭제 예정	
	public static int roomNumber;

	//UXPlayerController player;
	//UXClientController clientController;
	private UXPlayerController m_PlayerController; 
	private UXClientController m_ClientController;

	//UXAndroidManager androidManager;
	private UXAndroidManager m_AndroidManager;

	public PremiumVersionStore inapp;

	// 현재 플레이어 ID
	private int _i_PlayerID;
	public int i_PlayerID {
		get {
			return this._i_PlayerID;
		}
		set {
			PlayerIndexChanged (value);	
			this._i_PlayerID = value;
		}
	}

	private bool isRoomMaster = false; // 접속한 방의 방장인지?

	private int i_AckFailedCount = 0; // ACK 실패 횟수

	void Awake ()
	{
		if (FindObjectsOfType(this.GetType()).Length > 1)
		{
			Destroy(this.gameObject);
			return;
		}
		Screen.sleepTimeout = SleepTimeout.NeverSleep;

		#if UNITY_ANDROID && !UNITY_EDITOR
		GameObject go = GameObject.Find ("AndroidManager");
		m_AndroidManager = go.GetComponent<UXAndroidManager> ();
		m_AndroidManager.LockWifi();
		#endif
	}

	void Start ()
	{
		Screen.orientation = ScreenOrientation.Portrait;
		Screen.SetResolution(720, 1280, true);
		
		blackOut.SetActive(true);

		m_ClientController = UXClientController.Instance;
		m_PlayerController = UXPlayerController.Instance;

		/*
		userCode = -1;
		launcherCode = -1;

		bool result = m_ClientController.SetCode (userCode, launcherCode);
		result = true;
		if (result == false) {
			PopupManager_RaS.IsFreeSetter (true);
			Application.LoadLevel ("1_Login");
			return;
		} else {
			PopupManager_RaS.IsFreeSetter(false);
		}
		*/

		PopupManager_RaS.IsFreeSetter(false); // TOOD : PopupManager 공통 모듈로 바꿔야함

		userCode = m_PlayerController.GetCode(); // 유저 코드 가져옴
		roomNumber = UXConnectController.GetRoomNumber(); // 방 번호 가져옴

		blackOut.SetActive(false);
		
		//userCode = m_PlayerController.GetCode ();
		//launcherCode = UXClientController.GetRoomNumber();

		i_PlayerID = m_PlayerController.GetIndex();

		m_ClientController.OnConnected += OnConnected;
		m_ClientController.OnConnectFailed += OnConnectFailed;
		m_ClientController.OnJoinSucceeded += OnJoinSucceeded;
		m_ClientController.OnJoinFailed += OnJoinFailed;
		m_ClientController.OnDisconnected += OnDisconnected;
		
		m_ClientController.OnUserAdded += OnUserAdded;
		m_ClientController.OnUserRemoved += OnUserRemoved;
		m_ClientController.OnUserLeaved += OnUserLeaved;
		m_ClientController.OnNetworkReported += OnNetworkReported;
		m_ClientController.OnUpdateReadyCount += OnUpdateReadyCount;
		
		m_ClientController.OnGameStart += OnGameStart;
		m_ClientController.OnGameRestart += OnGameRestart;
		m_ClientController.OnGameResult += OnGameResult;
		m_ClientController.OnIndexChanged += OnIndexChanged;
		
		m_ClientController.OnUserListReceived += OnUserListReceived;
		m_ClientController.OnGameEnd += OnGameEnd;
		m_ClientController.OnExit += OnExit;
		m_ClientController.OnAckFailed += OnAckFailed;
		m_ClientController.OnHostDisconnected += OnHostDisconnected;

		//inapp.OnPurchaseSuccess += OnPurchaseSuccess;
		
		//==========================================
		m_ClientController.OnHostJoined += OnHostJoined;
		m_ClientController.OnError += OnError;
		m_ClientController.OnReceived += OnReceived;
		//==========================================
			
		if(m_ClientController.IsConnected() == false)
		{
			m_ClientController.Connect();
        }
        else
        {
            //m_ClientController.Join("none");
			m_ClientController.Join(GAME_PACKAGE_NAME);
        }

		cancelButton.SetActive(false);

		DontDestroyOnLoad(this.gameObject);
	}

    void Update()
    {
        m_ClientController.Run();
		i_PlayerID = UXRoom.Instance.Player.GetIndex ();
		if(i_PlayerID >= 0)
		{
            playerNumber.GetComponent<SpriteRenderer>().sprite = playerNumberSprite[i_PlayerID];
        }

		if(Input.GetKeyDown(KeyCode.Escape) == true)
        {
			//PopupManager.Instance().OpenPopup(POPUP_TYPE.POPUP_EXITCONFIRM);
        }

		if(m_PlayerController.GetLobbyState() == UXUser.LobbyState.Ready)
		{
			readyButton.SetActive(false);
			cancelButton.SetActive(true);
		}
		else if(m_PlayerController.GetLobbyState() == UXUser.LobbyState.Wait)
		{
			readyButton.SetActive(true);
			cancelButton.SetActive(false);
		}
	}

	void OnApplicationFocus(bool state) {}

	void OnConnected()
	{
		Debug.Log("OnServerConnected:-------");
		//m_ClientController.Join("none");
		//GameObject.Find ("QR_Back").GetComponent<QROnOff_ras> ().Init ();
		m_ClientController.Join(GAME_PACKAGE_NAME);
	}

	void OnConnectFailed()
	{
	}

	void OnJoinFailed(int errCode)
	{
		Debug.Log ("OnJoinFailed > " + errCode);

		//if (errCode == UXConnectController.JE_MAX_USER) 
		//{
			//PopupManager.Instance().OpenPopup(POPUP_TYPE.POPUP_MAXUSER);
		//}
		Clear();
		RoomNumberWindow.latest_errCode = errCode;
		Application.LoadLevel("2_RoomNumber");
	}

	void OnJoinSucceeded(bool isHost)
	{
		Debug.Log("OnJoinSucceed > isHost : " + isHost);
		/*
		m_ClientController.SetPlayerState (UXUser.LobbyState.Wait);
		i_PlayerID = m_PlayerController.GetIndex();
		Debug.Log ("ISPREMIUM : " + inapp.IsPremiumVersion ());
		if (inapp.IsPremiumVersion ()) {
			SendToHost ("PREMIUM,");
		}
		*/

		AfterJoin();
	}

	void OnDisconnected() {}

	void OnUserAdded(int userIndex, int userCode) 
	{
		i_PlayerID = m_PlayerController.GetIndex();
		//Debug.Log("OnLobbyUserAdded > userIndex : " + userIndex + ",userCode : " + userCode + " , PlayerID : " + i_PlayerID);
	}
	
	void OnUserRemoved(string name, int code)
	{
		i_PlayerID = m_PlayerController.GetIndex();
		// Reset State of Ready Button
		m_PlayerController.SetLobbyState(UXUser.LobbyState.Wait);
		Debug.Log("OnUserRemoved > name : " + name + " , Code : " + code + " , PlayerID : " + i_PlayerID);
	}

	void OnUserLeaved(int userIndex) {
		i_PlayerID = userIndex;
	}
	
	void OnNetworkReported(int count, float time) {}

	void OnUserLobbyStateChanged(int userIndex, UXUser.LobbyState state) {}
	
	void OnAutoCountChanged(int restSecond)  {}

	void OnUpdateReadyCount(int ready, int total) {}
	
	void OnGameStart() 
	{
		Debug.Log ("Start Game!!");

		DontDestroyOnLoad(this.gameObject);

		readyButton.SetActive(true);
		cancelButton.SetActive(true);

		foreach(Transform child in this.transform)
		{
			child.gameObject.SetActive(false);
		}
		Application.LoadLevel("CharacterSelectLobbyPS");
	}
	
	void OnGameRestart() {}
	
	void OnGameResult() {}
	
	void OnIndexChanged(int idx) 
	{
		Debug.Log ("OnIndexChanged > idx : " + idx);
		
		i_PlayerID = idx;
		//GameObject.Find ("QR_Back").GetComponent<QROnOff_ras> ().Init ();
	}
	
	void OnUserListReceived(List<UXUser> list) {}
	
	void OnGameEnd()
	{
		Debug.Log("Game End");

		foreach(Transform child in this.transform)
		{
			child.gameObject.SetActive(true);
		}

		cancelButton.SetActive(false);
	}
	
	void OnExit() {}

	void OnAckFailed ()
	{
		ackFailedCount++;
		
		if(ackFailedCount >= 3)
		{
			//PopupManager.Instance().OpenPopup(POPUP_TYPE.POPUP_ACKFAILED);
		}
	}
	
	void OnHostDisconnected ()
    {
        Debug.Log("HOST DISCONNECTED##################################################");
        PopupManager_RaS.Instance.OpenPopup(POPUP_TYPE_RaS.POPUP_HOSTDISCONNECTED);
	}
	
	void OnHostJoined() {}
	
	void OnError(int err, string msg) {}
	
	void OnReceived(int userIndex, string msg)
	{
		Debug.Log("OnReceived > userIndex : " + userIndex + ", msg : " + msg);

		string   splitchar  = ",";
		string[] words      = null;
		
		msg.Trim ();
		words = msg.Split (splitchar.ToCharArray(), System.StringSplitOptions.None);


        if (words[0] == "Exit")
        {
            PopupManager_RaS.Instance.CloseGame();
        }

		/*
		if (Application.loadedLevelName == "LobbyClient") {
			switch(words[0])
			{
				case "QROn":
					GameObject.Find("QR_Back").GetComponent<QROnOff_ras>().SetQROnOff(true);
					break;
				case "QROff":
					GameObject.Find("QR_Back").GetComponent<QROnOff_ras>().SetQROnOff(false);
					break;
			}
		}
		*/

		GameObject phoneScreen = GameObject.Find ("PS"); 
		if(phoneScreen == null)
		{
			return;
		}
		
		switch(words[0])
		{
			// Character Select
		case "SoldOut":
			phoneScreen.GetComponent<PS_CharacterSelectLobbyManager>().SetCharacterSoldOut(int.Parse(words[1]));
			break;
		case "StartTutorial":
			Application.LoadLevel("TutorialPS");
			break;
			
			// Tutorial
		case "StartInGame":
			Application.LoadLevel("InGamePS");
			break;
			
			// In Game
		case "DiamondSound":
			phoneScreen.GetComponent<PS_InGameManager>().PlayDiamondSound(int.Parse(words[1]));
			break;
		case "RingSound":
			phoneScreen.GetComponent<PS_InGameManager>().PlayRingSound(int.Parse(words[1]));
			break;
		case "GoldSound":
			phoneScreen.GetComponent<PS_InGameManager>().PlayGoldSound(int.Parse(words[1]));
			break;
		case "SilverSound":
			phoneScreen.GetComponent<PS_InGameManager>().PlaySilverSound(int.Parse(words[1]));
			break;
		case "AppleSound":
			phoneScreen.GetComponent<PS_InGameManager>().PlayAppleSound(int.Parse(words[1]));
			break;
		case "SellSound":
			phoneScreen.GetComponent<PS_InGameManager>().PlaySellSound(int.Parse(words[1]));
			break;
		case "SpecialSellSound":
			phoneScreen.GetComponent<PS_InGameManager>().PlaySpecialSellSound(int.Parse(words[1]));
			break;
		case "SpeedSound":
			phoneScreen.GetComponent<PS_InGameManager>().PlaySpeedSound(int.Parse(words[1]));
			break;
		case "MagnetSound":
			phoneScreen.GetComponent<PS_InGameManager>().PlayMagnetSound(int.Parse(words[1]));
			break;
		case "BonusSound":
			phoneScreen.GetComponent<PS_InGameManager>().PlayBonusSound(int.Parse(words[1]));
			break;
		case "StealEmojiSound":
			phoneScreen.GetComponent<PS_InGameManager>().PlayStealEmojiSound();
			break;
		case "AngryEmojiSound":
			phoneScreen.GetComponent<PS_InGameManager>().PlayAngryEmojiSound();
			break;
		case "CrashEmojiSound":
			phoneScreen.GetComponent<PS_InGameManager>().PlayCrashEmojiSound();
			break;
		case "MyScore":
			myScore = System.Convert.ToInt32(words[1]);
			break;
		case "StartResult":
			if(i_PlayerID == System.Convert.ToInt32(words[1]))
			{
				isRoomMaster = true;
			}
			Application.LoadLevel("ResultPS");
			break;
			
		case "Pause":
			GameObject.Find("PauseButton").GetComponent<PauseManager_ras>().GetPause();
			break;
		case "Resume":
			GameObject.Find("PauseButton").GetComponent<PauseManager_ras>().GetResume();
			break;
			
			// Result
		case "MyRank":
			myRank = System.Convert.ToInt32(words[1]);
			break;
		case "Replay":
			m_ClientController.SendEndGame();
			m_ClientController.SetPlayerState(UXUser.LobbyState.Wait);
			Application.LoadLevel("LobbyClient");
			break;
		}
	}
    
    public void Clear()
	{
        if (m_ClientController != null)
        {
            m_ClientController.OnConnected -= OnConnected;
            m_ClientController.OnConnectFailed -= OnConnected;
            m_ClientController.OnJoinFailed -= OnJoinFailed;
            m_ClientController.OnJoinSucceeded -= OnJoinSucceeded;
            m_ClientController.OnDisconnected -= OnDisconnected;

            m_ClientController.OnUserAdded -= OnUserAdded;
            m_ClientController.OnUserRemoved -= OnUserRemoved;
            m_ClientController.OnNetworkReported -= OnNetworkReported;

            m_ClientController.OnUpdateReadyCount -= OnUpdateReadyCount;
            m_ClientController.OnUserLeaved -= OnUserLeaved;

            m_ClientController.OnGameStart -= OnGameStart;
            m_ClientController.OnGameRestart -= OnGameRestart;
            m_ClientController.OnGameResult -= OnGameResult;
            m_ClientController.OnIndexChanged -= OnIndexChanged;

            m_ClientController.OnUserListReceived -= OnUserListReceived;
            m_ClientController.OnGameEnd -= OnGameEnd;
            m_ClientController.OnExit -= OnExit;
            m_ClientController.OnAckFailed -= OnAckFailed;
            m_ClientController.OnHostDisconnected -= OnHostDisconnected;

            //==========================================
            m_ClientController.OnHostJoined -= OnHostJoined;
            m_ClientController.OnError -= OnError;
            m_ClientController.OnReceived -= OnReceived;
            //==========================================

            //[FOR ONEPAD]
            //GamePadDownLoad.Instance.bundle.Unload(false);

            m_ClientController = null;
        }

        GameObject.Destroy(gameObject);
	}

	void OnGUI() 
	{
		GUI.color = Color.white; 
		GUI.skin.label.fontSize = 20;
		GUI.skin.button.fontSize = 40;
		
		// GUI.Label(new Rect(20, 0, 1600, 800), ":"+ UXLog.GetLogMessage());
		// GUI.Label(new Rect(20, 100, 1600, 800), ":"+ launcherCode);

	}

	/********** for game **********/
	//private int i_PlayerID = -1;
	//private bool isRoomMaster = false;

	private int ackFailedCount = 0;

	public int GetPlayerID()
	{
		return i_PlayerID;
	}

	public void SendAll(string str)
	{
		m_ClientController.SendData(str);
	}
	
	public void SendToHost(string str)
	{
		m_ClientController.SendDataToHost(str);
	}
	
	public void SendTo(int m_PlayerController, string msg)
	{
		m_ClientController.SendDataTo(m_PlayerController, msg);
	}

	/*
	public UXUser.LobbyState GetLobbyState()
	{
		return m_PlayerController.GetLobbyState();
	}
	*/

	// Phone Screen UI
	public GameObject blackOut = null;

	public GameObject playerNumber = null;
	public Sprite[] playerNumberSprite = new Sprite[6];

	public GameObject readyButton = null;
	public GameObject cancelButton = null;
	
	public AudioClip readyButtonSound = null;

	// Phone Screen Game Info
	public int myScore = 0;
	public int myRank = 0;
	public int myCharacter = (int)CHARACTER_TYPE.CHARACTER_NONE;
	
	public void ReadyButton()
	{
		m_ClientController.SetPlayerState(UXUser.LobbyState.Ready);
		
		Camera.main.GetComponent<AudioSource>().PlayOneShot(readyButtonSound);
	}
	
	public void CancelButton()
	{
		m_ClientController.SetPlayerState(UXUser.LobbyState.Wait);
		
		Camera.main.GetComponent<AudioSource>().PlayOneShot(readyButtonSound);
	}

	public bool IsRoomMaster()
	{
		return isRoomMaster;
	}

	public void Replay()
	{
		SendAll("Replay");

		m_ClientController.SendEndGame();
		m_ClientController.SetPlayerState(UXUser.LobbyState.Wait);
		Application.LoadLevel("LobbyClient");
	}

	public void OnPurchaseSuccess(){
		//////////////SendToHost ("PREMIUM,");
		UXPlayerController player = UXPlayerController.Instance;
		player.IsPremium = true;
	}

	public void AfterJoin ()
	{
		if (inapp.IsPremiumVersion())
		{
			////////////SendToHost("PREMIUM,");
			UXPlayerController player = UXPlayerController.Instance;
			player.IsPremium = true;
		}
		inapp.OnPurchaseSuccess += OnPurchaseSuccess;
	}

	public void PlayerIndexChanged (int index)
	{
		//Debug.Log("PlayerIndexChanged : " + index + " player. " + i_PlayerID);
		if(index >= 0)
		{
			playerNumber.GetComponent<SpriteRenderer>().sprite = playerNumberSprite[index];
		}
	}


}