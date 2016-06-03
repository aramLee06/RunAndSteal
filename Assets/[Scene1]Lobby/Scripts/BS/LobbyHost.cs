using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UXLib;
using UXLib.Base;
using UXLib.Connect;
using UXLib.User;
using UXLib.Util;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyHost : MonoBehaviour
{
	// Game 
	public static string GAME_PACKAGE_NAME = "com.cspmedia.runandsteal";

    UXHostController hostController;
    UXAndroidManager androidManager;

	public Text roomNumberTxt;
	public List <int> GameUserList = new List<int>();
	public GameObject QR_noOne;
	public GameObject QR_joinedOne;

	[SerializeField]
	LimitTimer timer;

    void Awake()
    {
		Debug.Log ("AWAKE LOBBY HOST!");
        if (FindObjectsOfType(this.GetType()).Length > 1)
        {
            Destroy(this.gameObject);
			iTween.MoveTo(Camera.main.gameObject, new Vector3(0, 0, -10), 4.0f);
			roomNumberTxt.text = UXHostController.GetRoomNumberString () + "";
            return;
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

    }

    void Start()
    {
		if (Screen.orientation == ScreenOrientation.LandscapeRight) {
			Screen.orientation = ScreenOrientation.LandscapeRight;
		} else {
			Screen.orientation = ScreenOrientation.Landscape;
		}
		Screen.SetResolution (1920, 1080, true);
        Debug.Log("LobbyHost Start : " + BS_LogoViewer.BuildType.ToString());

		screenLog (BS_LogoViewer.BuildType.ToString ());

        hostController = UXHostController.Instance;
		screenLog ("IsConnect : " + hostController.IsConnected ());
        if (hostController.IsConnected() == true)
        {
            if (UXHostController.GetRoomNumber() != -1)
                UXHostController.SetRoomNumber(-1);
            hostController.Clear();
        }
			
        int launcherCode = -1;

        bool result = hostController.SetCode(launcherCode);

        if (result == false)
        {
			hostController.CreateRoom(GAME_PACKAGE_NAME, hostController.GetMaxUser());
			Debug.Log ("Room NUM : " + UXHostController.GetRoomNumberString ());
			roomNumberTxt.text = UXHostController.GetRoomNumberString () + "";
			screenLog ("ROOM INFO : " + UXConnectController.ROOM_SERVER_IP + ", " + UXConnectController.ROOM_SERVER_PORT);
			screenLog (UXHostController.GetRoomNumberString ());
        }
        else
        {
			#if UNITY_ANDROID && !UNITY_EDITOR
            int country = androidManager.GetCountryCode();
            PlayerPrefs.SetInt("ServerList", country);
			#endif
			roomNumberTxt.text = UXHostController.GetRoomNumberString () + "";
        }

        hostController.OnConnected += OnConnected;
        hostController.OnConnectFailed += OnConnected;
        hostController.OnJoinFailed += OnJoinFailed;
        hostController.OnJoinSucceeded += OnJoinSucceeded;
        hostController.OnDisconnected += OnDisconnected;

        hostController.OnUserAdded += OnUserAdded;
        hostController.OnUserRemoved += OnUserRemoved;
        hostController.OnNetworkReported += OnNetworkReported;
        hostController.OnUserNetworkReported += OnUserNetworkReported;
        hostController.OnUserLobbyStateChanged += OnUserLobbyStateChanged;
        hostController.OnAutoCountChanged += OnAutoCountChanged;
        hostController.OnUpdateReadyCount += OnUpdateReadyCount;
		hostController.OnUserLeavedInGame += OnUserLeaved;

        hostController.OnGameStart += OnGameStart;
        hostController.OnGameRestart += OnGameRestart;
        hostController.OnGameResult += OnGameResult;
        hostController.OnIndexChanged += OnIndexChanged;

        hostController.OnUserListReceived += OnUserListReceived;
        hostController.OnGameEnd += OnGameEnd;
        hostController.OnExit += OnExit;
        hostController.OnAckFailed += OnAckFailed;

        //==========================================
        hostController.OnHostJoined += OnHostJoined;
        hostController.OnError += OnError;
        hostController.OnReceived += OnReceived;
        //==========================================
        hostController.OnHostDisconnected += hostController_OnHostDisconnected;

		timer.OnLimitTimeOut += TimeOut;


        hostController.SetAutoStart(2, 1);
           
        PopupManager_RaS.IsFreeSetter(true);
        hostController.Connect();

        maxPlayer = hostController.GetMaxUser();
		Debug.Log ("Max Player : " + maxPlayer);
        for (int i = 0; i < playerNumber.Length; i++)
        {
            playerNumber[i].SetActive(false);
        }

		Debug.Log ("LobbyHost :: " + selectedPlayerCharacter.Length);
        for (int i = 0; i < selectedPlayerCharacter.Length; i++)
        {
            selectedPlayerCharacter[i] = (int)CHARACTER_TYPE.CHARACTER_NONE;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
		StartCoroutine(PlayIntroVideo());
#endif

       // blackOut.SetActive(false);

        iTween.MoveTo(Camera.main.gameObject, new Vector3(0, 0, -10), 4.0f);
    }


    void hostController_OnHostDisconnected()
    {
        PopupManager_RaS.Instance.OpenPopup(POPUP_TYPE_RaS.POPUP_HOSTDISCONNECTED);
    }

    void Update()
    {
        if (hostController != null)
        {
            hostController.Run();
        }

        if (isGameStart == false)
        {
			//maxPlayer = hostController.GetMaxUser();
            for (int i = 0; i < 6; i++)
            {
                if (i < playerCount)
                {
                    if (playerNumber[i].activeSelf == false)
                    {
                        playerNumber[i].SetActive(true);
                        Camera.main.GetComponent<AudioSource>().PlayOneShot(numberSound);
                    }
                    UXUserController userController = UXUserController.Instance;
                    UXUser user = (UXUser)userController.GetAt(i);

                    if (user.GetLobbyState() == UXUser.LobbyState.Ready)
                    {
                        playerNumber[i].GetComponentInChildren<SpriteRenderer>().sprite = playerNumberOn[i];
                        connectedUser[i] = true;
                    }
                    else
                    {
                        playerNumber[i].GetComponentInChildren<SpriteRenderer>().sprite = playerNumberOff[i];
                        connectedUser[i] = false;
                    }
                }
                else
                {
                    playerNumber[i].SetActive(false);
                }
            }
        }
		QRPopoup (hostController.GetConnectUserCount());
    }

    void OnConnected()
    {
		hostController.Join(GAME_PACKAGE_NAME);
    }

    void OnJoinFailed(int errCode) {}

    void OnJoinSucceeded(bool isHost)
    {
        UXLog.SetLogMessage("OnJoinSucceed > isHost : " + isHost + " == 3");
        Debug.Log("OnJoinSucceed > isHost : " + isHost + " == 3");

        hostController.RefreshUserListFromServer();
        playerCount = hostController.GetConnectUserCount();
    }

    void OnDisconnected() {}

    void OnUserAdded(int userIndex, int userCode)
    {
		playerCount = hostController.GetConnectUserCount();
    }

    void OnUserRemoved(string name, int code)
    {
        UXLog.SetLogMessage("OnUserRemoved > name : " + name + " , Code : " + code + " == 6");
        Debug.Log("OnUserRemoved > name : " + name + " , Code : " + code + " == 6");
		hostController.RefreshUserListFromServer();
        playerCount = hostController.GetConnectUserCount();
    }



	void OnUserLeaved(int userIndex, int userCode)
	{
		userIndex = GameUserList.IndexOf (userCode);
        connectedUser[userIndex] = false;
		GameUserList [userIndex] = -1;
		roomMasterCode = -1;

		for (int i = 0; i < GameUserList.Count; i++) {
			if (GameUserList [i] != -1) {
				roomMasterCode = i;
				break;
			}
		}

		if (roomMasterCode == -1)// All Disconnected //여기 올일이 없ㅇㅡㅁ
        {
            hostController.SendEndGame();
			//TODO popup 
			//PopupManager_RaS.Instance.OpenPopup(POPUP_TYPE_RaS.POPUP_GAMESCLOSE);
           // Application.Quit();
        }

		int charType = selectedPlayerCharacter [userIndex];
        selectedPlayerCharacter[userIndex] = (int)CHARACTER_TYPE.CHARACTER_DISCONNECTED;

		totalScore[userIndex] = 0;
		for (int i = 0; i < itemScore.GetLength(1); i++)
		{
			itemScore[userIndex, i] = 0;
		}

		if (InGameUserCount() == 1) { //한명만 남은 거
			PopupManager_RaS.Instance.OpenPopup(POPUP_TYPE_RaS.POPUP_ZEROUSER);
			StartCoroutine ("EndGame");
		}

		if (SceneManager.GetActiveScene ().name == "CharacterSelectLobbyBS") {
			CancleSoldOut (userCode, charType);                    
			ClearSelectedCharacter(userIndex);
		}
    }
	public void ClearSelectedCharacter(int userIndex)
	{
		GameObject bigScreen = GameObject.Find("BS");
		if (bigScreen == null)
		{
			return;
		}
		bigScreen.GetComponent<BS_CharacterSelectLobbyManager>().ClearCharacter(userIndex);
	}
	public int InGameUserCount(){
		int count = 0;

		foreach (int code in GameUserList) {
			if (code != -1)
				count += 1;
		}
			
		return count;
	}

	void CancleSoldOut (int userCode, int charType)
	{
		List<int> SendList = new List<int> ();
	
		foreach (int code in GameUserList) {
			if (code != -1) {
				SendList.Add (code);
			}
		}

		hostController.SendDataToCode (SendList.ToArray (), "CancelSoldOut," + charType);
	}

    void OnNetworkReported(int count, float time)
    {
        UXLog.SetLogMessage("OnNetworkReported > count : " + count + ", time : " + time + " == 7");
        Debug.Log("OnNetworkReported > count : " + count + ", time : " + time + " == 7");
    }

    void OnUserNetworkReported(int userIndex, int count, float time)
    {
        UXLog.SetLogMessage("OnUserNetworkReported > userIndex : " + userIndex + ", count : " + count + ", time : " + time + " == 8");
        Debug.Log("OnUserNetworkReported > userIndex : " + userIndex + ", count : " + count + ", time : " + time + " == 8");
    }

    void OnUserLobbyStateChanged(int userIndex, UXUser.LobbyState state)
    {
        UXLog.SetLogMessage("OnUserLobbyStateChanged > userIndex : " + userIndex + ", state : " + state + " == 9");
        Debug.Log("OnUserLobbyStateChanged > userIndex : " + userIndex + ", state : " + state + " == 9");

        if (state == UXUser.LobbyState.Ready)
        {
            Camera.main.GetComponent<AudioSource>().PlayOneShot(numberOnSound);
        }
    }

    void OnAutoCountChanged(int restSecond)
    {
        UXLog.SetLogMessage("OnAutoCountChanged > restSecond:" + restSecond + " == 11");
        Debug.Log("OnAutoCountChanged > restSecond:" + restSecond + " == 11");
    }

    void OnUpdateReadyCount(int ready, int total)
    {
        UXLog.SetLogMessage("OnUpdateReadyCount > ready : " + ready + ", total : " + total + " == 12");
        Debug.Log("OnUpdateReadyCount > ready : " + ready + ", total : " + total + " == 12");
    }

    void OnGameStart()
    {
        UXLog.SetLogMessage("Start Game!!" + " == 13");
        Debug.Log("Start Game!!" + " == 13");

        DontDestroyOnLoad(this.gameObject);

        isGameStart = true;
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(false);
        }
		CopyGameUserList ();

		//timer.active = true;
		if (PlayerPrefs.GetInt ("TimeLimit") == 0) {
			timer.TimerStart ();
		}

        Application.LoadLevel("CharacterSelectLobbyBS");
    }

    void OnGameRestart()
    {
        UXLog.SetLogMessage("Restart Game" + " == 14");
        Debug.Log("Restart Game" + " == 14");

    }

    void OnGameResult()
    {
        UXLog.SetLogMessage("OnGameResult" + " == 15");
        Debug.Log("OnGameResult" + " == 15");
    }

    void OnIndexChanged(int idx)
    {
        UXLog.SetLogMessage("OnIndexChanged > idx : " + idx + " == 16");
        Debug.Log("OnIndexChanged > idx : " + idx + " == 16");
    }

    void OnUserListReceived(List<UXUser> list)
    {
        UXLog.SetLogMessage("OnUserListReceived > list : " + list.Count + " == 17");
        Debug.Log("OnUserListReceived > list : " + list.Count + " == 17");

        playerCount = hostController.GetConnectUserCount();
    }

    void OnGameEnd()
    {
        UXLog.SetLogMessage("Game End" + " == 18");
        Debug.Log("Game End" + " == 18");

        isGameStart = false;
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    void OnExit()
    {
        UXLog.SetLogMessage("Game Exit" + " == 19");
        Debug.Log("Game Exit" + " == 19");
    }

    void OnAckFailed()
    {
        if (isAckOn == true)
        {
            ackFailedCount++;

            if (ackFailedCount >= 3)
            {
                //PopupManager.Instance().OpenPopup(POPUP_TYPE.POPUP_ACKFAILED);
            }
        }
    }

    void OnHostJoined()
    {
        UXLog.SetLogMessage("OnHostJoined" + " == 20");
        Debug.Log("OnHostJoined" + " == 20");
    }

    void OnError(int err, string msg)
    {
        UXLog.SetLogMessage("OnError > err : " + err + ", msg : " + msg + " == 21");
        Debug.Log("OnError > err : " + err + ", msg : " + msg + " == 21");
    }

    void OnReceived(int userCode, string msg)
    {
		int userIndex = GameUserList.IndexOf (userCode);
		
        UXLog.SetLogMessage("OnReceived > userIndex : " + userIndex + ", msg : " + msg + " == 22");
        Debug.Log("OnReceived > userIndex : " + userIndex + ", msg : " + msg + " == 22");

        string splitchar = ",";
        string[] words = null;

        msg.Trim();
        words = msg.Split(splitchar.ToCharArray(), System.StringSplitOptions.None);

		if (words [0] == "TimeLimitPurchase") {
			WocheongSDK.getInstance ().buyItem ();
		}

        if (words[0] == "Exit")
        {
            //SendAll("Exit");

            //Debug.Break();
            //hostController.SendEndGame();
            //Application.Quit();

        }

        GameObject bigScreen = GameObject.Find("BS");
        if (bigScreen == null)
        {
            return;
        }

        switch (words[0])
        {
            // Character Select
            case "CharacterSelect":
                if (bigScreen.GetComponent<BS_CharacterSelectLobbyManager>().IsSoldOutCharacter(int.Parse(words[1])) == false)//선택가능
                {
                    bigScreen.GetComponent<BS_CharacterSelectLobbyManager>().SetSelectedCharacter(userIndex, int.Parse(words[1]));
                    for (int i = 0; i < playerCount; i++)
                    {
                        if (i != userIndex)
                        {
							SendToCode(GameUserList[i], "SoldOut," + words[1]); // 팔렸다고 모두에게 알ㄹㅣ기
                        }
                    }
                }
                else // soldout
                {
                    SendToCode(GameUserList[userIndex], "SoldOut," + words[1]);//이건 머야
                }
                break;

            // Tutorial

            // In Game
            case "JoystickDown":
                bigScreen.GetComponent<BS_InGameManager>().JoystickDown(userIndex, int.Parse(words[1]));
                break;
            case "JoystickUp":
                bigScreen.GetComponent<BS_InGameManager>().JoystickUp(userIndex);
                break;

            // Result
            case "Replay":
                roomMasterCode = -1;
                hostController.RefreshUserListFromServer();
				CopyGameUserList ();
				playerCount = hostController.GetConnectUserCount();
                hostController.SendEndGame();
                Application.LoadLevel("LobbyHost");
                break;

            case "Pause":
                SendAll("Pause_cli");
                GameObject.Find("PauseUI").GetComponent<Image>().enabled = true;
                GameObject.Find("PauseBlack").GetComponent<Image>().enabled = true;
                Time.timeScale = 0f;
                break;
            case "Resume":
                SendAll("Resume_cli");
                GameObject.Find("PauseUI").GetComponent<Image>().enabled = false;
                GameObject.Find("PauseBlack").GetComponent<Image>().enabled = false;
                Time.timeScale = 1f;
                break;

//            case "QROn":
//                GameObject.Find("QR_Back_Host").transform.localScale = Vector2.one;
//                break;
//            case "QROff":
//                GameObject.Find("QR_Back_Host").transform.localScale = Vector2.zero;
//                break;
        }
    }

    void OnDestroy()
    {
        if (hostController != null)
        {
            //hostController.SendExit();
            hostController.Clear();
            UXConnectController.SetRoomNumber(-1);

            hostController.OnConnected -= OnConnected;
            hostController.OnConnectFailed -= OnConnected;
            hostController.OnJoinFailed -= OnJoinFailed;
            hostController.OnJoinSucceeded -= OnJoinSucceeded;
            hostController.OnDisconnected -= OnDisconnected;

            hostController.OnUserAdded -= OnUserAdded;
            hostController.OnUserRemoved -= OnUserRemoved;
            hostController.OnNetworkReported -= OnNetworkReported;
            hostController.OnUserNetworkReported -= OnUserNetworkReported;
            hostController.OnUserLobbyStateChanged -= OnUserLobbyStateChanged;
            hostController.OnAutoCountChanged -= OnAutoCountChanged;
            hostController.OnUpdateReadyCount -= OnUpdateReadyCount;
			hostController.OnUserLeavedInGame -= OnUserLeaved;

            hostController.OnGameStart -= OnGameStart;
            hostController.OnGameRestart -= OnGameRestart;
            hostController.OnGameResult -= OnGameResult;
            hostController.OnIndexChanged -= OnIndexChanged;

            hostController.OnUserListReceived -= OnUserListReceived;
            hostController.OnGameEnd -= OnGameEnd;
            hostController.OnExit -= OnExit;
            hostController.OnAckFailed -= OnAckFailed;

            //==========================================
            hostController.OnHostJoined -= OnHostJoined;
            hostController.OnError -= OnError;
            hostController.OnReceived -= OnReceived;
            //==========================================
            hostController.OnHostDisconnected -= hostController_OnHostDisconnected;

			timer.OnLimitTimeOut += TimeOut;
        }
    }
    void OnGUI()
    {
        GUI.color = Color.white;
        GUI.skin.label.fontSize = 20;
        GUI.skin.button.fontSize = 40;

    }

    /********** for game **********/
    private int playerCount;
    private int maxPlayer;

    private bool isGameStart = false;
    private bool isTutorialWatched = false;

    public UILabel roomNumberLabel = null;

    private bool[] connectedUser = new bool[6];
    private int roomMasterCode = 0;

    private bool isAckOn = true;
    private int ackFailedCount = 0;

    public int GetPlayerCount()
    {
        return playerCount;
    }

    public void SendAll(string str)
    {
        hostController.SendData(str);
    }

    public void SendTo(int player, string msg)
    {
        hostController.SendDataTo(player, msg);
    }

	public void SendToCode (int u_code, string msg)
	{
		hostController.SendDataToCode (u_code, msg);
	}

    // Big Screen UI
    //public GameObject blackOut = null;

    public GameObject[] playerNumber = new GameObject[6];
    public Sprite[] playerNumberOn = new Sprite[6];
    public Sprite[] playerNumberOff = new Sprite[6];

    public AudioClip numberSound = null;
    public AudioClip numberOnSound = null;

    // Big Screen Game Info
    public int[] selectedPlayerCharacter = new int[6];

    public int[,] itemScore = new int[6, 5];
    public int[] totalScore = new int[6];

    IEnumerator PlayIntroVideo()
    {
        isAckOn = false;

        Handheld.PlayFullScreenMovie("intro.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);

        yield return new WaitForSeconds(3.0f);

        isAckOn = true;
    }

    public void StartTutorial()
    {
        if (isTutorialWatched == false)
        {
            isTutorialWatched = true;

            SendAll("StartTutorial");

			timer.TimerStop ();

#if UNITY_ANDROID && !UNITY_EDITOR
			Application.LoadLevel("TutorialBS_Android");
#else
            LobbyHost lobbyHost = GameObject.Find("LobbyHost").GetComponent<LobbyHost>();
            lobbyHost.StartInGame();
            //Application.LoadLevel("TutorialBS_PC");
#endif
        }
        else
        {
            StartInGame();
        }
    }

    public void StartInGame()
    {
        Application.LoadLevel("GameLoadingBS");
    }

    public void StartResult()
    {
		SendAll("StartResult," + GameUserList[roomMasterCode]);

        Application.LoadLevel("ResultBS");
    }

	public void screenLog(string str){
	}

	public void screenLogClear(){
	}
	public void CopyGameUserList (){
		UXUserController userList = UXUserController.Instance;
		GameUserList.Clear ();

		foreach (UXObject obj in userList.GetList()) {
			UXUser user = (UXUser)obj;
			GameUserList.Add (user.GetCode());
		}
	}

	void QRPopoup(int playerCount){
		QR_noOne = QR_noOne == null ? GameObject.Find ("QR_noOne") : QR_noOne;
		QR_joinedOne = QR_joinedOne == null ? GameObject.Find ("QR_joinedOne") : QR_joinedOne;

		if (QR_noOne != null && QR_joinedOne != null) {
			if (playerCount == 0) { 
				QR_noOne.SetActive (true);
				//QR_joinedOne.SetActive (false);
			} else {
				QR_noOne.SetActive (false);
				//QR_joinedOne.SetActive (true);
			}	
		}
	}

	IEnumerator EndGame(){
		yield return new WaitForSeconds (2);
		Application.Quit ();
	}

	public void TimeOut(){
//		text.text = "End!";
		PopupManager_RaS.Instance.OpenPopup(POPUP_TYPE_RaS.POPUP_NOMONEY);
		StartCoroutine ("FreeTimeOut");
	}

	IEnumerator FreeTimeOut(){
		yield return new WaitForSeconds (2);
		PopupManager_RaS.Instance.ClosePopup ();
		hostController.SendData ("Replay");
	}

}