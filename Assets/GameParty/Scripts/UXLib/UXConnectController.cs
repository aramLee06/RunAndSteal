using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Timers;

using UXLib.Base;
using UXLib.Connect;
using UXLib.Util;
using UXLib.User;

using System.Threading;
using SimpleJSON;

namespace UXLib {
	public abstract class UXConnectController : UXObject{
		/// REST Server URL

		public static string ROOM_SERVER_IP = "112.74.40.64";

		public static int ROOM_SERVER_PORT = 5000;

		public static string BASE_REST_URL = "http:/112.74.40.64:3000";	//중국

		public static int JE_FAIL = 1; /**< Fail to join */
		public static int JE_MAX_USER = 2; /**< Exceed max user */
		public static int JE_ALREADY_START = 3; /**< Game is already started */
		
		public static char DATA_DELIMITER = (char)232;
		
		protected static int launcherCode = -1;
		
		static string prevReceivedData;
		
		/// Connection mode
		public enum Mode {
			Host,	/**< Host */
			Client, /**< Client */
			None /**< None */
		};


		public enum Language {
			China, English
		}
	


		public static List<string> receiveQueue; 
		protected bool isQueueRunning = false;
		protected bool isJoined = false;
		
		protected bool isSendAck = false;
		protected UXAckSender ackSender;
		
		UXRoomConnect connect;	
		protected Mode connectMode;

		protected Language selectLanguage;

		protected int networkCheckCount;
		protected long[] networkCheckValues;
		
		protected bool isSendNetWorkResult;
		protected bool isHostJoined;
		protected bool isGameStarted = false;
		
		protected string systemUID;
		
		public abstract void Join(string data);
		
		public delegate void OnJoinFailedHandler(int failCode);
		public delegate void OnJoinSucceededHandler(bool isHostJoined);
		public delegate void OnConnectedHandler();
		public delegate void OnConnectFailedHandler();
		public delegate void OnDisconnectedHandler();
		public delegate void OnHostJoinedHandler();
		public delegate void OnErrorHandler(int err, string msg);
		public delegate void OnReceivedHandler(int userIndex, string msg);
		public delegate void OnUserAddedHandler(int userIndex, int code);
		public delegate void OnUserRemovedHandler(string name, int code);
		public delegate void OnUserLobbyStateChangedHandler(int userIndex, UXUser.LobbyState state);
		public delegate void OnUserNetworkReportedHandler(int userIndex, int count, float time); 
		public delegate void OnNetworkReportedHandler(int count, float time);
		public delegate void OnGameStartHandler();
		public delegate void OnGameRestartHandler();
		public delegate void OnGameResultHandler();
		public delegate void OnGameEndHandler();
		public delegate void OnUserLeavedHandler(int index);
		public delegate void OnExitHandler();
		public delegate void OnUpdateReadyCountHandler(int ready, int total);
		public delegate void OnIndexChangedHandler(int idx);
		public delegate void OnUserListReceivedHandler(List<UXUser> list);
		public delegate void OnAckFailedHandler();
		public delegate void OnHostDisconnectedHandler();
				
		/** Called when user connect to server */
		public event OnConnectedHandler OnConnected;
		
		public event OnConnectFailedHandler OnConnectFailed;
		
		/** Called when socket is disconnected */
		public event OnDisconnectedHandler OnDisconnected;
		
		/** Called when user join a room */
		public event OnHostJoinedHandler OnHostJoined;
		
		/** Called when error has occurred
			@param err error code
			@param msg error message
		*/	 
		public event OnErrorHandler OnError;
		
		/** Called when data was received
			@param userIndex sender index
			@param msg message
		*/
		public event OnReceivedHandler OnReceived;
		
		/** Called when user join room
			@param userIndex user index in list
			@param userCode user Code
		*/
		public event OnUserAddedHandler OnUserAdded;
		
		/** Called when connection was terminated
		    @param name user name
		    @param userCode user Code
		*/
		public event OnUserRemovedHandler OnUserRemoved;
		
		/** Called when user leave the game
			@param index user index
		*/
		public event OnUserLeavedHandler OnUserLeaved;
		
		/** Called when user index was changed
			@param idx new index
		*/	
		public event OnIndexChangedHandler OnIndexChanged;
		
		/** Called when host received network test results
			@count test count
			@time 
		*/ 	
		public event OnNetworkReportedHandler OnNetworkReported;
		
		/** Called when start message was received */
		public event OnGameStartHandler OnGameStart;
		
		/** Called when restart message was received */
		public event OnGameRestartHandler OnGameRestart;
		
		public event OnGameResultHandler OnGameResult;
		
		/** Called when result message was received */
		public event OnGameEndHandler OnGameEnd;

		/** Called when countdown signa was received */
		public event OnUpdateReadyCountHandler OnUpdateReadyCount;
		
		/** Called when user list is received
			@param list user list
		*/
		public event OnUserListReceivedHandler OnUserListReceived;
		
		/** Called when exit event is received */
		public event OnExitHandler OnExit;
		
		/** */		
		public event OnAckFailedHandler OnAckFailed;
		
		/** */
		public event OnHostDisconnectedHandler OnHostDisconnected;
		
		public UXConnectController() : base("UXConnectController") {
			receiveQueue = new List<string>();
			systemUID = SystemInfo.deviceUniqueIdentifier;
			connect = new UXRoomConnect();
			PlayerPrefs.SetInt("ServerList",0);
			ServerCheck((ServerList)PlayerPrefs.GetInt("ServerList"));
		}


		public static void ServerCheck(ServerList str){

		
			switch(str){
			case ServerList.CN:{
				ROOM_SERVER_IP = "112.74.40.64";
				#if GOOGLE
				BASE_REST_URL = "http://112.74.40.64:3000";
				ROOM_SERVER_PORT = 5000;
				#else
				BASE_REST_URL = "http://112.74.40.64:3000";
				ROOM_SERVER_PORT = 5000;
				#endif
			}
				break;
			case ServerList.SG : {
				ROOM_SERVER_IP = "52.76.82.48";
				
				#if GOOGLE
				BASE_REST_URL = "http://52.76.82.48:3000";
				ROOM_SERVER_PORT = 5000;
				#else
				BASE_REST_URL = "http://52.76.82.48:3000";
				ROOM_SERVER_PORT = 5000;
				#endif
			}
				break;
			}
		}

		/** Inìtialization 
			@warning This method msub be called before using UXConnectController
		*/	
		public void Init() {
			if (connect != null) {
				connect.OnReceived += OnMessageReceived;
				connect.OnServerConnected += OnServerConnected;
				connect.OnServerConnectFailed += OnServerConnectFailed;
				connect.OnServerDisconnected += OnServerDisconnected;
				connect.OnServerError += OnServerError;
			}	
		}
		
		/** Release event handler and disconnect socket	*/	
		public void Clear() {

			if (connect != null) {
				connect.OnReceived -= OnMessageReceived;
				connect.OnServerConnected -= OnServerConnected;
				connect.OnServerConnectFailed -= OnServerConnectFailed;
				connect.OnServerDisconnected -= OnServerDisconnected;
				connect.OnServerError -= OnServerError;
				
				Disconnect ();
			}
			connect = null;
			UXLog.Close ();
		}

		public bool InitAckSender(string title = "") {
			if (connect == null) {
				return false;
			}
			
			isSendAck = true;
			
			ackSender = new UXAckSender();
			
			if (title != "") {
				ackSender.ChangeCommandTitle (title);
			}
			ackSender.SetConnect(connect);
			
			return true;
		}
		
		public void StartAckSender() {
			if (ackSender == null) {
				return;
			}
//			ackSender.Start ();
		}
		
		public void StopAckSender() {
			if (ackSender == null) {
				return;
			}
			ackSender.Stop();
		}
		
		public void Run() {
			if (isSendAck == true && ackSender != null) {
				if (ackSender.CheckAckCount() == false) {
					if (OnAckFailed != null) {
						OnAckFailed();
					}
				}
			}
		}
		
		public UXRoomConnect GetRoomConnect() {
			return connect;
		} 
		
		/** Return formatted room number */
		public static string GetRoomNumberString() {
			if (launcherCode == -1) {
				return "";
			}
			
			return string.Format ("{0:D5}", launcherCode);
		}
		
		/** Return room number */
		public static int GetRoomNumber() {
			return launcherCode;
		}
		
		/** Set room number */
		public static void SetRoomNumber(int room) {
			launcherCode = room;
		}
		
		/** Set connection mode
			@warning It must be called before connection
			@see Mode
		*/
		public void SetMode(Mode mode) {
			connectMode = mode;
		}


		public void SetLanguage(Language lan){
			selectLanguage = lan;
		}


		public string GetLanguage(){
			return selectLanguage.ToString();
		}

		/** Connect to room server
			@brief connect, if socket is not connected
			@param hostIP Server IP Address
			@param port Server Port
		*/

		public void Connect() {

			string serverID = ROOM_SERVER_IP;


			if(connect == null)
			{
				connect = new UXRoomConnect();
				Init();
				SetMode(Mode.Client);
			}

			if (connect.IsConnected() == false) 
			{
				bool result = connect.SocketOpen(serverID, ROOM_SERVER_PORT);

			}
		}
		
		/** Close connection */
		public void Disconnect() {
			isJoined = false;
			connect.Disconnect ();
		}
		
		/** Return host connection state
			@param True if host is joined, false otherwise
		*/	
		public bool IsHostJoined() {
			return isHostJoined;
		}
		
		/** Return user index from user code
			@param code user code
			@return User index. if it can't be found then return -1
		*/		
		public int GetUserIndexFromCode(int code) {
			UXUserController userController = UXUserController.Instance;
			
			for (int i = 0; i < userController.GetCount(); i++) {
				UXUser user = (UXUser)userController.GetAt (i);
				if (code == user.GetCode ()) {
					return i;
				}
			}
			
			return -1;
		}
		
		public bool IsEventExist(Event handler) {
			if (handler == null) {
				return false;
			}
			
			return true;
		}
		
		/** Set debug mode
			@param debug mode
			@see UXRoomConnect.DebugMode
		*/			
		public void SetDebugMode(UXRoomConnect.DebugMode mode) {
			UXRoomConnect.debugMode = mode;
		}
		
		/** Return debug mode
			@return debug mode
			@see UXRoomConnect.DebugMode
		*/		
		public UXRoomConnect.DebugMode GetDebugMode() {
			return UXRoomConnect.debugMode;
		}
		
		/** Return current debug string
			@return dubug message
			@see UXRoomConnect.debugString
			@todo add all network error
		*/
		public static string GetDebugString() {
			return UXRoomConnect.debugString;
		}
		
		/** Return current connection mode
			@return connection mode
			@see Mode
		*/	
		public Mode GetConnectMode() {
			return connectMode;
		}
		
		/** Return connnection state  
			@return True if it was connected, false otherwize
		*/
		public bool IsConnected() {
			if (connect == null) {
				return false;
			}
			
			return connect.IsConnected();
		}
		
		/** Return join state
			@return True if user is joined, false otherwise
		*/
		public bool IsJoined() {
			return isJoined;
		}
		
		/** Send game start message
			@see OnGameStartHandler
		*/	
		public void SendStartGame() {
			string sendString = "{\"cmd\":\"start_game\",\"l_code\":\"" + launcherCode + "\"}" + DATA_DELIMITER; 
			Send (sendString);    
		}
		
		/** Send game restart message
			@see OnGameRestartHandler
		*/	
		public void SendRestartGame() {
			string sendString = "{\"cmd\":\"restart_game\",\"l_code\":\"" + launcherCode + "\"}" + DATA_DELIMITER; 
			Send (sendString);    
		}
		
		/** Send game result message
			@see OnGameResultHandler
		*/
		public void SendResultGame() {
			string sendString = "{\"cmd\":\"result_game\",\"l_code\":\"" + launcherCode + "\"}" + DATA_DELIMITER; 
			Send (sendString);    
		}
		
		/** Send game end message
			@see OnGameEndHandler
		*/
		public void SendEndGame() {
			string sendString = "{\"cmd\":\"end_game\",\"l_code\":\"" + launcherCode + "\"}" + DATA_DELIMITER; 
			Send (sendString);    
		}		
		
		/** Send game exit message
			@see OnGameExitHandler
		*/
		public void SendExit() {
			string type = "host";
			int userCode = -1;
			
			if (connectMode == Mode.Client) {
				type = "user";
				userCode = UXPlayerController.Instance.GetCode ();
			}
			
			string sendString = "{\"cmd\":\"exit\",\"type\":\"" + type + "\",\"l_code\":\"" + launcherCode + "\",\"u_code\":\"" + userCode + "\"}" + DATA_DELIMITER; 
			Send (sendString);
		}
		
		/** Send changed user index
			@param user code
			@param idx changed index
		*/
		public void SendUserIndex(int userCode, int idx) {
			if (connectMode == Mode.Client) {
				return;
			}
			
			string msg = "{\"cmd\":\"update_user_index\",\"u_code\":\""+ userCode + "\",\"l_code\":\"" + launcherCode + "\",\"index\":\""+ idx + "\"}" + DATA_DELIMITER;
			Send (msg);
		}
		
		/** Broadcast data
			@param msg sending data
		*/	
		public void SendData(string msg) {
			string sendString = "{\"cmd\":\"broadcast\",\"data\":";
			sendString += GetSendDataFormat (msg);
			sendString += "}" + DATA_DELIMITER;
			
			connect.Send (sendString);
		}
		
		/** Send data to specific user
			@param userIndex user index
			@param msg sending data
		*/	 
		public void SendDataTo(int userIndex, string msg) {
			UXUser user = (UXUser)UXUserController.Instance.GetAt (userIndex);
			
			if (user.IsConnected() == true) {
				SendDataToCode (user.GetCode (), msg);
			}
		}
		
		/** Send data to specific user
			@param user UXUser Type
			@param msg sending data
			@see UXUser
		*/	
		public void SendDataTo(UXUser user, string msg) {
			int userIndex = GetUserIndexFromCode(user.GetCode ());
			SendDataTo (userIndex, msg);
		}
		
		/** Send data to specific user
			@param target user Code
		*/	
		public void SendDataToCode(int target, string msg) {
			string sendString = "{\"cmd\":\"send_target\",\"target\":[\"" + target + "\"],\"data\":";
			sendString += GetSendDataFormat (msg);
			sendString += "}" + DATA_DELIMITER; 
			
			connect.Send (sendString);
		}
		
		/** Send data to host
			@param msg sending data
		*/
		public void SendDataToHost(string msg) {
			string sendString = "{\"cmd\":\"send_host\",\"data\":";
			sendString += GetSendDataFormat (msg);
			sendString += "}" + DATA_DELIMITER; 
			connect.Send (sendString);    
		}
		
		/** Send raw data to server
			@param msg
			@warning Use if you know the protocol
		*/	
		public void Send(string msg) {
			connect.Send (msg);           
		}	
		
		
		/** Test network speed and send the data to host
			@param count sending count
			@param sendResult True if want to broadcat the result, false otherwise
		*/
		public void NetworkTest(int count, bool sendResult=false) {
			UXPlayerController player = UXPlayerController.Instance;
			
			isSendNetWorkResult = sendResult;
			networkCheckCount = count;
			networkCheckValues = new long[count];
			
			for (int i = 0; i < count; i++) {
				string sendString = "{\"cmd\":\"check_network_state\",\"u_code\":\"" + player.GetCode () + "\",\"l_code\":\"" + launcherCode + "\",\"count\":\"" + (i+1) + "\",\"time\":\"" + DateTime.Now.Ticks + "\"}" + DATA_DELIMITER;
				connect.Send (sendString);
				
				System.Threading.Thread.Sleep (50);
			}
		}

		/** Request for user list
			@see OnUserListReceived
		*/
		public void RefreshUserListFromServer() {
			UXPlayerController player = UXPlayerController.Instance;
			
			string msg = "{\"cmd\":\"get_user_list\",\"l_code\":\"" + launcherCode + "\"}" + DATA_DELIMITER;
			Send (msg);
		}
		
		string GetSendDataFormat(string data) {
			bool isAddDoubleQuotationMarks = true;
			
			if (data.StartsWith ("{") || data.StartsWith("[")) {
				isAddDoubleQuotationMarks = false;
			}
			
			string sendData = "";
			
			if (isAddDoubleQuotationMarks == true) {
				sendData += "\"";
			}
			sendData += data;
			if (isAddDoubleQuotationMarks == true) {
				sendData += "\"";
			}
			
			return sendData;
		}
		
		void OnMessageReceived(string data) {
			if (string.IsNullOrEmpty(prevReceivedData) == false) {
				data = prevReceivedData + data;
			}
			string[] datas = data.Split(DATA_DELIMITER);
			lock(receiveQueue)
			{
				for (int i = 0; i < datas.Length-1; i++) {
					if(datas[i].Substring(0,1)=="{")
						receiveQueue.Add (datas[i]);
				}
			}
			if (string.IsNullOrEmpty(datas[datas.Length - 1]) == false) {
				prevReceivedData = datas[datas.Length - 1];
			} else {
				prevReceivedData = null;
			}
		}
		
		void OnServerConnected() {
			if(OnConnected != null) {
				OnConnected();
			}
		}
		
		void OnServerConnectFailed() {
			if(OnConnectFailed != null) {
				OnConnectFailed();
			}
		}
		
		void OnServerDisconnected() {
			if (OnDisconnected != null)
				OnDisconnected();
		}
		
		void OnServerError(int err, string msg) {
			if (OnError != null) {
				OnError(err, msg);
			}
		}

		protected List<UXUser> ParseUserList(JSONArray users) {
			List<UXUser> userList = new List<UXUser>();
			
			for (int i = 0; i < users.Count; i++) {
				string temp = users[i];
				
				string[] info = temp.Split ('.');
				UXUser userObj = new UXUser(info[1], Int32.Parse (info[0]));
				userObj.SetConnected(true);
				userList.Add (userObj);
			}	
			
			return userList;		
		}
		
		protected int ProcessConnectError(int result) {
			isJoined = false;
			int errCode = JE_FAIL;
				
			if (result == UXErrorCode.RS_ERROR_MAX_USER) {
				errCode = JE_MAX_USER;
			} else if (result == UXErrorCode.RS_ERROR_ALREADY_START) {
				errCode = JE_ALREADY_START;
			} 
			
			return errCode;
		}
		
		protected void ProcessReceivedMessage(string data) {
			if (string.IsNullOrEmpty(data) == true || data.Length <= 0) {
				Debug.Log ("Empty" );
				return;
			}

			Debug.Log ("data : " + data);

			var N = JSON.Parse(data);
			string command = N["cmd"];
			
			if (command == "ack_result") {
				if (isSendAck == true && ackSender != null) {
					ackSender.ReceiveResult();
				}
			} else if (command == "user_add") {
				Debug.Log ("UserAdd : " );
				int code = N["u_code"].AsInt;
				string name = N["name"];
				
				UXUser userObj = new UXUser(name, code);
				userObj.SetConnected(true);
				userObj.GetProfileFromServer();
				
				UXUserController userController = UXUserController.Instance;
				userController.Add ((UXObject)userObj);
				
				var array = N["user_list"];
				
				if (array != null) {
					List<UXUser> list = ParseUserList((JSONArray)array);
					
					if (userController.IsEqual(list) == false) {
						userController.CopyList(list);
					}
				}
				
				if (OnUserAdded != null) {
					int userIndex = GetUserIndexFromCode (code);
					OnUserAdded(userIndex, code);
				}
			} else if (command == "user_del") {
				int code = N["u_code"].AsInt;
				
				UXUserController userController = UXUserController.Instance;
				List<UXObject> userList = userController.GetList();
				
				for (int i = 0; i < userList.Count; i++) {
					UXUser user = (UXUser)userList[i];
					
					if (user.GetCode() == code) {
						if (isGameStarted == false) { // In lobby
							string name = user.GetName ();
							
							userController.RemoveByName(user.GetName ());

							UXPlayerController player = UXPlayerController.Instance;
							player.SetIndex(i);

							var array = N["user_list"];
							
							if (array != null) {
								List<UXUser> list = ParseUserList((JSONArray)array);
								
								if (userController.IsEqual(list) == false) {
									userController.CopyList(list);
								}
							}	
							
							if (OnUserRemoved != null) {
								OnUserRemoved(name, code);
							}	
						} else {
							user.SetConnected(false);
							
							if (OnUserLeaved != null) {
								OnUserLeaved(i);
							}
						}
						
						break;
					}
				}
			} else if (command == "update_user_index_result") {
				int index = N["index"].AsInt;
				
				UXPlayerController player = UXPlayerController.Instance;
				player.SetIndex (index);
				if (OnIndexChanged != null) {
					UXLog.SetLogMessage(" onindexchanged" );
					OnIndexChanged(index);
				}
			} else if (command == "send_error") {
			} else if (command == "exit_result") {
				if (OnExit != null) {
					OnExit();
				}
			} else if (command == "host_close") {
				
				if (OnHostDisconnected != null) {
					OnHostDisconnected();
				}
			} else if (command == "data") {
				string val = N["data"].Value.ToString();
				
				if (val == "") {
					val = N["data"].ToString();
				}
				
				int senderCode = N["sender"].AsInt;
				int userIndex = GetUserIndexFromCode (senderCode);
				
				if (OnReceived != null) {
					OnReceived(userIndex, val); 
				}
			}  else if (command == "check_network_state_result") {				
				int cur = N["count"].AsInt;
				string temp = N["time"];
				long stime = long.Parse (temp);
				
				networkCheckValues[cur - 1] = DateTime.Now.Ticks - stime;
				
				if (cur >= networkCheckCount) {
					float totalTime = 0;
					for (int i = 0; i < networkCheckCount; i++) {
						totalTime += networkCheckValues[i];
					}
					
					if (OnNetworkReported != null) {
						OnNetworkReported(networkCheckCount, totalTime);
					}
					
					UXPlayerController player = UXPlayerController.Instance;
					
					if (isSendNetWorkResult == true) {
						string sendString = "{\"cmd\":\"report_network_state\",\"u_code\":\"" + player.GetCode () + "\",\"l_code\":\"" + launcherCode + "\",\"count\":\"" + networkCheckCount + "\",\"time\":\"" + totalTime + "\"}&";
						Send (sendString);
					}
				}
			}else if (command == "start_game_result") {
				isGameStarted = true;
				
				if (OnGameStart != null) {
					OnGameStart();
				}
			} else if (command == "restart_game_result") {
				isGameStarted = true;
				
				if (OnGameRestart != null) {
					OnGameRestart();
				}
			} else if (command == "result_game_result") {
				if (OnGameResult != null) {
					OnGameResult();
				}
			} else if (command == "end_game_result") {
				isGameStarted = false;
				
				if (OnGameEnd != null) {
					OnGameEnd();
				}	
			} else if (command == "host_joined") {
				isHostJoined = true;
				
				if (OnHostJoined != null) {
					OnHostJoined();
				}
			} else if (command == "get_user_list_result") {

				UXUserController userController = UXUserController.Instance;
				List<UXUser> userList = ParseUserList((JSONArray)N["list"]);
				
				if (userController.IsEqual(userList) == false) {
					userController.CopyList(userList);

					
					if (OnUserListReceived != null) {
						OnUserListReceived(userList);
					}
				}
				
				if (connectMode == UXConnectController.Mode.Client) {
					UXPlayerController playerController = UXPlayerController.Instance;
					for (int i = 0; i < userController.GetCount(); i++) {
						UXUser user = (UXUser)userController.GetAt (i);
						if (playerController.GetCode () == user.GetCode ()) {
							if (i != playerController.GetIndex()) {
								if (OnIndexChanged != null) {
									OnIndexChanged(i);
								}
							
								playerController.SetIndex (i);
								Debug.Log ("SetInedx : " + i);
							}
							break;
						}
					}
				}
			} else if (command == "update_ready_count_result") {
				int ready = N["ready"].AsInt;
				int total = N["total"].AsInt;
				
				if (OnUpdateReadyCount != null) {
					OnUpdateReadyCount(ready, total);
				}
			}
		}		
	}
}
