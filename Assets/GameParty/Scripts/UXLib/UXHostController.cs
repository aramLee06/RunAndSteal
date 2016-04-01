using UnityEngine;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using UXLib.Base;
using UXLib.Connect;
using UXLib.Util;
using UXLib.User;
using SimpleJSON;
namespace UXLib {
	public class UXHostController : UXConnectController {
		static string REQUEST_ROOM_NUMBER = "g r n"; //쓰레기값. 방만들때 "id":"grn"으로 보낸다.
		public const int DEFAULT_START_DELAY_SEC = 3; /**< Delay second for auto start */ //세팅할때씀
		
		bool isAutoStart = false; /**< Start game automatically when all users are ready , default value is false */ 
		int autoStartCount = 0; /**< Countdown */  
		int autoStartDelaySec = 3; /**< Delay second for auto start */  // 3-(++1)
		int autoStartMinimumUser;  /**< Minimum number for auto start */
		
		System.Threading.Timer startGameTimer; /**< Timer for auto start */
		//int maxUser;
		bool autoStartStarted;
		bool isStartSended;
		public delegate void OnUserStateChangedHandler(int userIndex, UXUser.LobbyState state);
		public delegate void OnAutoCountChangedHandler(int restSecond);
		public delegate void OnAutoStartFailedHandler();
		public delegate void OnJoinPremiumUserHandler ();
		public delegate void OnLeavePremiumUserHandler ();
		
		/** Called when host succeeded in joinig 
			@param isHostJoined True if host was joined, false otherwise
		*/	
		public event OnJoinSucceededHandler OnJoinSucceeded;
		
		/** Called when host failed to join
			@param failCode (JE_FAIL, JE_MAX_USER, JE_ALREADY_START) 
		*/
		public event OnJoinFailedHandler OnJoinFailed;
		
		/** Called when host received network result 
			@userIndex user index
			@count Test count
			@time time
		*/	
		public event OnUserNetworkReportedHandler OnUserNetworkReported; //x
		
		/** Called when user's lobby state was changed 
			@param userIndex user index
			@param state changed lobby state 
			@see UXUser.LobbyState 
		*/		
		public event OnUserLobbyStateChangedHandler OnUserLobbyStateChanged;
		
		/** When auto start is starting the countdown, this envent is called every second
			@param restSecond Rest second to start game 
		*/	
		public event OnAutoCountChangedHandler OnAutoCountChanged; 
		
		/** Called when auto start is failed */
		public event OnAutoStartFailedHandler OnAutoStartFailed;

		/** Called when Joined premium user **/
		public event OnJoinPremiumUserHandler OnJoinPremiumUser;

		/** Called when leaved premium user **/
		public event OnLeavePremiumUserHandler OnLeavePremiumUser;
		
		private static UXHostController instance = null;
		
		/** Singletone */
		public static UXHostController Instance {
			get {
				if (instance == null) {
					instance = new UXHostController();
				}
				return instance;
			}
		}
		
		private UXHostController() { //초기화 하는듯
			Init ();
			
			autoStartCount = 0;
			//maxUser = 0;
			autoStartStarted = false;
			isStartSended = false;
			
			//SetMode (UXConnectController.Mode.Host);
		}
		
		/** Process message queue. It must be called continually */ 
		public void Run() {
			base.Run ();
			
			lock(receiveQueue)
			{
				int count = receiveQueue.Count;
				if (count == 0) {
					return;
				}
				
				for (int i = 0; i < count; i++) {
					ProcessReceivedMessage(receiveQueue[i]); //다쓰고
				}
				receiveQueue.Clear(); //비운당
			}
		}
		
		/* Send join message to server */
		public override void Join(string package_name) {
			string msg = "{\"cmd\":\"join\",\"type\":\"host\",\"l_code\":\"" + room.RoomNumber + "\",\"u_code\":\"" + player.GetCode() + "\",\"package_name\":\"" + package_name + "\", \"max_user\" : " + room.MaxUser + "}"; 
			Send(msg); //{"cmd":"join","type":"host","l_code":"launcherCode","u_code":"-1","name":"host","max_user":"maxUser",package_name":"data"}232
            Debug.Log("Send : "+msg);
		}
		
		/** Send user's ready state when it was changed 
			@param ready - ready user number 
			@param total - total user number 
		*/	
		void SendUpdateReadyCount(int ready, int total) {  
			string sendString = "{\"cmd\":\"update_ready_count\",\"u_code\":\"" + player.GetCode() + "\",\"l_code\":\"" + room.RoomNumber + "\",\"ready\":\"" + ready + "\",\"total\":\"" + total + "\"}" + UXConnectController.DATA_DELIMITER;  //????
			Send (sendString);//{"cmd":"update_ready_count","l_code":"launcherCode","ready":"ready","total":"total"}
            Debug.Log("Send : " + sendString);
		}

		void ProcessReceivedMessage(string data) {
			if (string.IsNullOrEmpty(data) == true || data.Length <= 0) { //data가 없는 경우는 ㅂㅂ 
				return;
			}
			
			var N = JSON.Parse(data);
			string command = N["cmd"];
			
			if (command == "join_result") {
				int result = N ["ack"].AsInt;
				if (result != UXRoomConnect.ACK_RESULT_OK) {
					int errCode = ProcessConnectError (result);
					
					if (OnJoinFailed != null) {
						OnJoinFailed (errCode);
					}
					return;
				} 
				
				isJoined = true;
				isHostJoined = true;

				
				JSONArray users = (JSONArray)N ["user_list"];

				
				if (users.Count > 0) {
					List<UXUser> list = ParseUserList ((JSONArray)users);
					for (int i = 0; i < list.Count; i++) {
						UXUser u = list [i];
						u.SetConnected (true);  //isConnected = ture;로
						//u.GetProfileFromServer (); //host면 x ,pad면 name, image url (정보가져오기)
					}
					
					UXUserController.Instance.CopyList (list);
				}
				
				if (OnJoinSucceeded != null) {
					OnJoinSucceeded (isHostJoined);
				}
			} else if (command == "change_lobby_state_result") {
				int code = N ["u_code"].AsInt;		
				string stateString = N ["state"];

				Debug.Log (code);

				int userIndex = GetUserIndexFromCode (code);
				UXUser.LobbyState state = UXUser.LobbyState.Wait;
				
				if (stateString == "ready") {
					state = UXUser.LobbyState.Ready;
				}

				UXUser userObj = (UXUser)UXUserController.Instance.GetAt (userIndex);
				userObj.SetLobbyState (state);	
				
				OnLobbyStateChanged (userIndex, state);
				
			} else if (command == "report_network_state_result") { //이것들도 안쓰이겠찌이
				
				int count = N ["count"].AsInt;
				string temp = N ["time"];
				float totalTime = float.Parse (temp);
				
				int code = N ["u_code"].AsInt;
				int userIndex = GetUserIndexFromCode (code);
				
				UXUser userObj = (UXUser)UXUserController.Instance.GetAt (userIndex);
				float time = totalTime / ((float)count * 10000000.0f);
				userObj.SetNetworkSpeed (time);
				
				if (OnUserNetworkReported != null) {
					OnUserNetworkReported (userIndex, count, time);
				}
			} else if (command == "premium_user_result") {
				int code = N ["u_code"].AsInt;	
				UXUserController userList = UXUserController.Instance;
				for (int i = 0; i < userList.GetCount (); i++) {
					UXUser user = (UXUser)userList.GetAt (i);
					if (user.GetCode () == code) {
						user.IsPremium = true;
						break;
					}
				}

				if (OnJoinPremiumUser != null) {
					OnJoinPremiumUser ();
				}
			} else if (command == "user_del") {
				int code = N ["u_code"].AsInt;
				UXUserController userList = UXUserController.Instance;
				UXUser user = userList.GetUserByCode (code);
				if (user.IsPremium) {
					if (OnLeavePremiumUser != null) {
						OnLeavePremiumUser ();
					}
				}
				base.ProcessReceivedMessage(data);
			} else {
				base.ProcessReceivedMessage(data);
			}
		}

		private void SendMaxUser(int maxUser) //UXRoom으로 옮김
		{
			/*UXPlayerController player = UXPlayerController.Instance;

			string sendString = "{\"cmd\":\"max_user_set\",\"max_client\":\"" + maxUser + "\",\"l_code\":\"" + launcherCode + "\",\"u_code\":\"" + player.GetCode() + "\"}" + DATA_DELIMITER;
			Send(sendString);   // {"cmd":"max_user_set","max_client":" maxUser","l_code":"launcherCode","u_code":"player.GetCode()"}232
			*/
			//room.MaxUser = maxUser;
		}
		
		
		/** Set maximum user for game
			@param value max user number
		*/	
		public void SetMaxUser(int value) { 
			room.MaxUser=value;
			/*
			Debug.Log ("maxuser setting: "+room.MaxUser);
			//maxUser = value;
			if (isJoined) {
				SendMaxUser (room.MaxUser);	
			}*/
		}
		
		/** Get maximum user for game
			@return max user number
		*/
		public int GetMaxUser() {
			return room.MaxUser;		
		}

		public bool IsPremiumRoom(){
			return room.IsPremium;
		}

		/** Set auto start options
			@param minimumUser minium user
			@param sec delay second
		*/	
		public void SetAutoStart(int minimumUser, int sec=DEFAULT_START_DELAY_SEC) {
			isAutoStart = true;
			autoStartDelaySec = sec;
			autoStartMinimumUser = minimumUser;
		}
		
		/** Return user's ready state
			@param user index
			@return True if user is ready, false otherwise
		*/	
		public bool IsReadyUser(int index) {
			UXUserController users = UXUserController.Instance;
			if (index < 0 || index >= users.GetCount()) { //인덱스는 0,1,2,3인가봐
				return false;
			}
			
			UXUser user = (UXUser)users.GetAt (index);
			
			return (user.GetLobbyState() == UXUser.LobbyState.Ready);
		}
		
		/** Return connnected user's number
			@return user number
		*/
		public int GetConnectUserCount() {
			UXUserController users = UXUserController.Instance;
			return users.GetCount ();
		}
		
		/** Set room number
			@return Room number if room number is valid, -1 otherwise
		*/
		public bool SetCode(int aLcode) { //room number
			
			int lcode = GetRoomNumber();
			
			if (lcode != -1) { //룸넘버 잘 받아옴
                //?
			} else {    //룸넘버를 못받아옴. lcode == -1
				lcode = aLcode;
			}
			
			if (lcode == -1) { //룸넘버를 못받아왔는데 입력으로 온 aLcode가 -1이었던 경우..//lobbyhost_에서 사용됨 방만들기 전에
				return false;
			}	
			
			launcherCode = lcode;
			
			return true;
		}
		
		/** Create room
			@return True if it was crated successfully, false otherwise
		*/
		public bool CreateRoom(string packageName, int maxClient) { //방만든다.
			/**
			JSONClass json = new JSONClass();
			
			json.Add ("id", REQUEST_ROOM_NUMBER); //"grb"
			json.Add ("pw", "");

            string recData = UXRestConnect.Request("launchers/token", UXRestConnect.REST_METHOD_POST, json.ToString()); //launchers/token:방만드는애
			
			if (recData == null) {
				return false;
			}
			
			var N = JSON.Parse(recData);
			
			int rec = N["gp_ack"].AsInt;
			bool result	= (rec == UXRoomConnect.ACK_RESULT_OK);
			if (result == true) {
				launcherCode = N["l_code"].AsInt;
			}
			return result;
			**/

			return room.CreateRoom (packageName, maxClient);
		}
		
		bool CheckUserState() {
			UXUserController users = UXUserController.Instance;
			int connectedUser = 0;
			for (int i = 0; i < users.GetCount (); i++) { 
				UXUser user = (UXUser)users.GetAt (i);
				
				UXUser.LobbyState userState = user.GetLobbyState();
				
				if (userState == UXUser.LobbyState.Ready) {
					connectedUser++;
				} else if (userState == UXUser.LobbyState.Wait) {
					return false;
				}
			}
			
			if(connectedUser >= autoStartMinimumUser){ //왜 너랑?
				return true;
			}
			
			return false;
		}
		
		void OnLobbyStateChanged(int idx, UXUser.LobbyState state) {
			if (OnUserLobbyStateChanged != null) {
				OnUserLobbyStateChanged(idx, state);
			}
			
			bool isAllReady = true;
			
			UXUserController users = UXUserController.Instance;
			int connectedUser = 0;
			
			for (int i = 0; i < users.GetCount (); i++) { 
				UXUser user = (UXUser)users.GetAt (i);
				
				UXUser.LobbyState userState = user.GetLobbyState();
				
				if (userState == UXUser.LobbyState.Ready) {
					connectedUser++;
				} else if (userState == UXUser.LobbyState.Wait) {
					isAllReady = false;
					break;
				}
			}
			SendUpdateReadyCount(connectedUser, users.GetCount ());
			
			if (/*autoStartStarted == true &&*/ isAllReady == false) {
				autoStartCount = 0;
				autoStartStarted = false;
				isStartSended = false;
				//if (startGameTimer != null) {
				//	startGameTimer.Dispose();
				//}
				
				//OnAutoStartFailed();
			}
			if (isAllReady == true && autoStartStarted == false) {
				if (isAutoStart == true && connectedUser >= autoStartMinimumUser) {
					autoStartCount = 0;
					autoStartStarted = true;
					isStartSended = false;
					ClearTimer();
					startGameTimer = new System.Threading.Timer(CountAutoStart, null, 1000, 1000);
				}
			}	
		}
		
		void ClearTimer() {
			if (startGameTimer != null) {
				startGameTimer.Dispose();
				startGameTimer = null; 
			}
		}
		void CountAutoStart(System.Object stateInfo) {
			if (autoStartStarted == false) {
				return;
			}
			if(CheckUserState() == false) {
				autoStartCount = 0;
				autoStartStarted = false;
				isStartSended = false;
				OnAutoStartFailed();
				
				return;
			}
			autoStartCount++;
			
			int restSecond = autoStartDelaySec - autoStartCount; 
			Debug.Log ("RestSecond : " + restSecond + " autoStartCount : " + autoStartCount);
			if (OnAutoCountChanged != null) {
				OnAutoCountChanged(restSecond);
			}
			
			if (restSecond < 0) {
				autoStartCount = 0;
				autoStartStarted = false;
				if (isStartSended == false) {
					ClearTimer();
					SendStartGame();
				}
				isStartSended = true;
			} else {
				ClearTimer();
				startGameTimer = new System.Threading.Timer(CountAutoStart, null, 1000, 1000);
			}
		}
	}
}  