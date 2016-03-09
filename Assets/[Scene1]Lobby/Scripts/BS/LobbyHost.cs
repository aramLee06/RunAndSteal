using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UXLib;
using UXLib.Base;
using UXLib.Connect;
using UXLib.User;
using UXLib.Util;
using UnityEngine.UI;

public class LobbyHost : MonoBehaviour
{
	// Game 
	public static string GAME_PACKAGE_NAME = "com.cspmedia.runandsteal";

    UXHostController hostController;
    UXAndroidManager androidManager;

	public Text roomNumberTxt;
	public Text logText;
	public GameObject freeLabel;

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
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.SetResolution(1920, 1080, true);

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
        hostController.OnUserLeaved += OnUserLeaved;

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

		hostController.OnJoinPremiumUser += OnJoinPremiumUser;
		hostController.OnLeavePremiumUser += OnLeavePremiumUser;

        hostController.SetAutoStart(2, 1);
        if (result == false)
        {
            
            hostController.SetMaxUser(2); // for GOOGLE
            f2pLabel.SetActive(true);

        }
        else
        {
			hostController.SetMaxUser(2); // for GOOGLE
			f2pLabel.SetActive(true);
        }
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

        blackOut.SetActive(false);

        iTween.MoveTo(Camera.main.gameObject, new Vector3(0, 0, -10), 4.0f);
    }

    void OnLeavePremiumUser () {}

    void OnJoinPremiumUser ()
    {
		hostController.SetMaxUser (6);
		if (freeLabel != null) {
			freeLabel.SetActive (false);
		}

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
			maxPlayer = hostController.GetMaxUser();
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

        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            //			PopupManager.Instance().OpenPopup(POPUP_TYPE.POPUP_EXITCONFIRM);
        }


		if(freeLabel != null) {
			if (UXHostController.room.IsPremium) {
				freeLabel.SetActive (false);
			} else {
				freeLabel.SetActive (true);
			}
		}
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

		CheckPremiumUser ();
    }

	void CheckPremiumUser() {
		
		if (UXHostController.room.IsPremium) {
			hostController.SetMaxUser (6);
			if (freeLabel != null) {
				freeLabel.SetActive (false);
			}
		} else {
			hostController.SetMaxUser (2);
			if (freeLabel != null) {
				freeLabel.SetActive (true);
			}
		}

	}

    void OnUserLeaved(int userIndex)
    {
        connectedUser[userIndex] = false;

        for (int i = 0; i < connectedUser.Length; i++)
        {
            if (connectedUser[i] == true)
            {
                roomMasterIndex = i;
                break;
            }

            // All Disconnected
            roomMasterIndex = -1;
        }

        if (roomMasterIndex == -1)
        {
            hostController.SendEndGame();
            Application.Quit();
        }

        selectedPlayerCharacter[userIndex] = (int)CHARACTER_TYPE.CHARACTER_DISCONNECTED;

        totalScore[userIndex] = 0;
        for (int i = 0; i < itemScore.GetLength(1); i++)
        {
            itemScore[userIndex, i] = 0;
        }
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

    void OnReceived(int userIndex, string msg)
    {
        UXLog.SetLogMessage("OnReceived > userIndex : " + userIndex + ", msg : " + msg + " == 22");
        Debug.Log("OnReceived > userIndex : " + userIndex + ", msg : " + msg + " == 22");

        string splitchar = ",";
        string[] words = null;

        msg.Trim();
        words = msg.Split(splitchar.ToCharArray(), System.StringSplitOptions.None);

        if (words[0] == "Exit")
        {
            //SendAll("Exit");

            //Debug.Break();
            //hostController.SendEndGame();
            //Application.Quit();

        }

		if (Application.loadedLevelName.Equals("LobbyHost"))
        {
			
			if (freeLabel == null) {
				freeLabel = GameObject.Find ("Free Play");
			} 

			if (UXHostController.room.IsPremium) {
				freeLabel.SetActive (false);
			} else {
				freeLabel.SetActive (true);
			}
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
                if (bigScreen.GetComponent<BS_CharacterSelectLobbyManager>().IsSoldOutCharacter(int.Parse(words[1])) == false)
                {
                    bigScreen.GetComponent<BS_CharacterSelectLobbyManager>().SetSelectedCharacter(userIndex, int.Parse(words[1]));
                    for (int i = 0; i < playerCount; i++)
                    {
                        if (i != userIndex)
                        {
                            SendTo(i, "SoldOut," + words[1]);
                        }
                    }
                }
                else
                {
                    SendTo(userIndex, "SoldOut," + words[1]);
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
                roomMasterIndex = 0;
                hostController.RefreshUserListFromServer();
                hostController.SendEndGame();
                Application.LoadLevel("LobbyHost");
                break;

            case "Pause":
                SendAll("Pause");
                GameObject.Find("PauseUI").GetComponent<Image>().enabled = true;
                GameObject.Find("PauseBlack").GetComponent<Image>().enabled = true;
                Time.timeScale = 0f;
                break;
            case "Resume":
                SendAll("Resume");
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
            hostController.OnUserLeaved -= OnUserLeaved;

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
			hostController.OnJoinPremiumUser -= OnJoinPremiumUser;
			hostController.OnLeavePremiumUser -= OnLeavePremiumUser;
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
    public GameObject f2pLabel;

    private bool[] connectedUser = new bool[6];
    private int roomMasterIndex = 0;

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

    // Big Screen UI
    public GameObject blackOut = null;

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
        SendAll("StartResult," + roomMasterIndex);

        Application.LoadLevel("ResultBS");
    }

	public void screenLog(string str){
		//logText.text += "\n" + str;
	}

	public void screenLogClear(){
		//logText.text = "LOG :";
	}
}