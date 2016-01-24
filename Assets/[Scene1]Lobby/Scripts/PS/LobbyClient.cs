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
	int userCode;
	public static int launcherCode;
	
	UXPlayerController player;
	UXClientController clientController;

	UXAndroidManager androidManager;

	PremiumVersionStore inapp;

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
		androidManager = go.GetComponent<UXAndroidManager> ();
		androidManager.LockWifi();
#endif
	}

	void Start ()
	{
		Screen.orientation = ScreenOrientation.Portrait;
		Screen.SetResolution(720, 1280, true);
		
		blackOut.SetActive(true);

		clientController = UXClientController.Instance;
		player = UXPlayerController.Instance;

		userCode = -1;
		launcherCode = -1;

		inapp = GameObject.Find ("InAppPurchase").GetComponent<PremiumVersionStore> ();

		bool result = clientController.SetCode (userCode, launcherCode);
		result = true;
		if (result == false) {
			PopupManager_RaS.IsFreeSetter (true);
			Application.LoadLevel ("1_Login");
			return;
		} else {
			PopupManager_RaS.IsFreeSetter(false);
		}

		blackOut.SetActive(false);
		
		userCode = player.GetCode ();
		launcherCode = UXClientController.GetRoomNumber();

		playerID = player.GetIndex();

		clientController.OnConnected += OnConnected;
		clientController.OnConnectFailed += OnConnected;
		clientController.OnJoinFailed += OnJoinFailed;
		clientController.OnJoinSucceeded += OnJoinSucceeded;
		clientController.OnDisconnected += OnDisconnected;
		
		clientController.OnUserAdded += OnUserAdded;
		clientController.OnUserRemoved += OnUserRemoved;
		clientController.OnNetworkReported += OnNetworkReported;
		clientController.OnUpdateReadyCount += OnUpdateReadyCount;
		clientController.OnUserLeaved += OnUserLeaved;
		
		clientController.OnGameStart += OnGameStart;
		clientController.OnGameRestart += OnGameRestart;
		clientController.OnGameResult += OnGameResult;
		clientController.OnIndexChanged += OnIndexChanged;
		
		clientController.OnUserListReceived += OnUserListReceived;
		clientController.OnGameEnd += OnGameEnd;
		clientController.OnExit += OnExit;
		clientController.OnAckFailed += OnAckFailed;
		clientController.OnHostDisconnected += OnHostDisconnected;

		inapp.OnPurchaseSuccess += OnPurchaseSuccess;
		
		//==========================================
		clientController.OnHostJoined += OnHostJoined;
		clientController.OnError += OnError;
		clientController.OnReceived += OnReceived;
		//==========================================
			
		if(clientController.IsConnected() == false)
		{
			clientController.Connect();
        }
        else
        {
            clientController.Join("none");
        }

		cancelButton.SetActive(false);
	}

    void Update()
    {
        clientController.Run();
		if(playerID >= 0)
		{
            playerNumber.GetComponent<SpriteRenderer>().sprite = playerNumberSprite[playerID];
        }

		if(Input.GetKeyDown(KeyCode.Escape) == true)
        {
			//PopupManager.Instance().OpenPopup(POPUP_TYPE.POPUP_EXITCONFIRM);
        }

		if(player.GetLobbyState() == UXUser.LobbyState.Ready)
		{
			readyButton.SetActive(false);
			cancelButton.SetActive(true);
		}
		else if(player.GetLobbyState() == UXUser.LobbyState.Wait)
		{
			readyButton.SetActive(true);
			cancelButton.SetActive(false);
		}
	}

	void OnApplicationFocus(bool state)
	{
	}

	void OnConnected()
	{
		Debug.Log("OnServerConnected:-------");

		clientController.Join("none");
		//GameObject.Find ("QR_Back").GetComponent<QROnOff_ras> ().Init ();
	}

	void OnJoinFailed(int errCode)
	{
		Debug.Log ("OnJoinFailed > " + errCode);

		if (errCode == UXConnectController.JE_MAX_USER) 
		{
			//PopupManager.Instance().OpenPopup(POPUP_TYPE.POPUP_MAXUSER);
		}
	}

	void OnJoinSucceeded(bool isHost)
	{
		Debug.Log("OnJoinSucceed > isHost : " + isHost);
		clientController.SetPlayerState (UXUser.LobbyState.Wait);
		playerID = player.GetIndex();
		if (inapp.IsPremiumVersion ()) {
			SendToHost ("PREMIUM,");
		}
	}

	void OnDisconnected()
    {
	}

	void OnUserAdded(int userIndex, int userCode) 
	{
		Debug.Log("OnLobbyUserAdded > userIndex : " + userIndex + ",userCode : " + userCode);

		playerID = player.GetIndex();
	}
	
	void OnUserRemoved(string name, int code)
	{
		Debug.Log("OnUserRemoved > name : " + name + " , Code : " + code);

		playerID = player.GetIndex();
	}

	void OnUserLeaved(int userIndex)
	{

	}
	
	void OnNetworkReported(int count, float time)
	{
		Debug.Log("OnNetworkReported > count : " + count + ", time : " + time);
	}

	void OnUserLobbyStateChanged(int userIndex, UXUser.LobbyState state)
	{
		Debug.Log("OnUserLobbyStateChanged > userIndex : " + userIndex + ", state : " + state);
	}
	
	void OnAutoCountChanged(int restSecond) 
	{
		Debug.Log("OnAutoCountChanged > restSecond:" + restSecond);
	}
	
	void OnUpdateReadyCount(int ready, int total)
	{
		Debug.Log("OnUpdateReadyCount > ready : " + ready + ", total : " + total);
	}
	
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
	
	void OnGameRestart()
	{
		Debug.Log("Restart Game");
	}
	
	void OnGameResult()
	{
		Debug.Log("OnGameResult");
	}
	
	void OnIndexChanged(int idx) 
	{
		Debug.Log ("OnIndexChanged > idx : " + idx);
		
		playerID = idx;
		//GameObject.Find ("QR_Back").GetComponent<QROnOff_ras> ().Init ();
	}
	
	void OnUserListReceived(List<UXUser> list)
	{
		Debug.Log ("OnUserListReceived > list : " + list.Count);
	}
	
	void OnGameEnd()
	{
		Debug.Log("Game End");

		foreach(Transform child in this.transform)
		{
			child.gameObject.SetActive(true);
		}

		cancelButton.SetActive(false);
	}
	
	void OnExit()
	{
		Debug.Log("Game Exit");
	}

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
	
	void OnHostJoined()
	{
		Debug.Log("OnHostJoined");
	}
	
	void OnError(int err, string msg)
	{
		Debug.Log("OnError > err : " + err + ", msg : " + msg);
	}
	
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
			if(playerID == System.Convert.ToInt32(words[1]))
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
			clientController.SendEndGame();
			clientController.SetPlayerState(UXUser.LobbyState.Wait);
			Application.LoadLevel("LobbyClient");
			break;
		}
	}
    
    public void Clear()
	{
        if (clientController != null)
        {
            clientController.OnConnected -= OnConnected;
            clientController.OnConnectFailed -= OnConnected;
            clientController.OnJoinFailed -= OnJoinFailed;
            clientController.OnJoinSucceeded -= OnJoinSucceeded;
            clientController.OnDisconnected -= OnDisconnected;

            clientController.OnUserAdded -= OnUserAdded;
            clientController.OnUserRemoved -= OnUserRemoved;
            clientController.OnNetworkReported -= OnNetworkReported;

            clientController.OnUpdateReadyCount -= OnUpdateReadyCount;
            clientController.OnUserLeaved -= OnUserLeaved;

            clientController.OnGameStart -= OnGameStart;
            clientController.OnGameRestart -= OnGameRestart;
            clientController.OnGameResult -= OnGameResult;
            clientController.OnIndexChanged -= OnIndexChanged;

            clientController.OnUserListReceived -= OnUserListReceived;
            clientController.OnGameEnd -= OnGameEnd;
            clientController.OnExit -= OnExit;
            clientController.OnAckFailed -= OnAckFailed;
            clientController.OnHostDisconnected -= OnHostDisconnected;

            //==========================================
            clientController.OnHostJoined -= OnHostJoined;
            clientController.OnError -= OnError;
            clientController.OnReceived -= OnReceived;
            //==========================================

            //[FOR ONEPAD]
            //GamePadDownLoad.Instance.bundle.Unload(false);

            clientController = null;
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
	private int playerID = -1;
	private bool isRoomMaster = false;

	private int ackFailedCount = 0;

	public int GetPlayerID()
	{
		return playerID;
	}

	public void SendAll(string str)
	{
		clientController.SendData(str);
	}
	
	public void SendToHost(string str)
	{
		clientController.SendDataToHost(str);
	}
	
	public void SendTo(int player, string msg)
	{
		clientController.SendDataTo(player, msg);
	}

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
		clientController.SetPlayerState(UXUser.LobbyState.Ready);
		
		Camera.main.GetComponent<AudioSource>().PlayOneShot(readyButtonSound);
	}
	
	public void CancelButton()
	{
		clientController.SetPlayerState(UXUser.LobbyState.Wait);
		
		Camera.main.GetComponent<AudioSource>().PlayOneShot(readyButtonSound);
	}

	public bool IsRoomMaster()
	{
		return isRoomMaster;
	}

	public void Replay()
	{
		SendAll("Replay");

		clientController.SendEndGame();
		clientController.SetPlayerState(UXUser.LobbyState.Wait);
		Application.LoadLevel("LobbyClient");
	}

	public void OnPurchaseSuccess(){
		SendToHost ("PREMIUM,");
	}
}