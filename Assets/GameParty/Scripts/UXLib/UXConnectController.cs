using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Timers;

using UXLib.Base;
using UXLib.Connect;
using UXLib.Connect.Protocol;
using UXLib.Util;
using UXLib.User;

using System.Threading;
using SimpleJSON;

namespace UXLib
{
    public abstract class UXConnectController : UXObject
    {

        //public static string ROOM_SERVER_IP = "192.168.0.81";
		public static string ROOM_SERVER_IP = "112.74.40.64";

        public const int ROOM_SERVER_PORT = 4000;// 7000;

        //public static string BASE_REST_URL = "http://192.168.0.81:3000";
		public static string BASE_REST_URL = "http://112.74.40.64:3000"; //중국 ^0^


        /**<join event*/
        public static int JE_FAIL = 1; /**< Fail to join */
        public static int JE_MAX_USER = 2; /**< Exceed max user */
        public static int JE_ALREADY_START = 3; /**< Game is already started */

        public static char DATA_DELIMITER = (char)232;//여기서 짤라

        protected static int launcherCode = -1;

        static string prevReceivedData;

        /// Connection mode
        public enum Mode
        {
            Host,	/**< Host */
            Client, /**< Client */
            None /**< None */
        };


        public enum Language
        {
            China, English
        }



        public static List<string> receiveQueue;
        protected bool isQueueRunning = false;
        protected bool isJoined = false;

        protected bool isSendAck = false;//안씀
        protected UXAckSender ackSender;//안씀

        UXRoomConnect connect;

		public static UXRoom room; //static으로 바꿈

		public UXPlayerController player;

		protected static Mode connectMode;  //static으로 바꿈
		 
        protected Language selectLanguage;

        protected int networkCheckCount;//안씀
        protected long[] networkCheckValues;//안씀

        protected bool isSendNetWorkResult;//안씀
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
        public event OnUserRemovedHandler OnUserRemoved;//로비에서 나감

        /** Called when user leave the game
            @param index user index
        */
        public event OnUserLeavedHandler OnUserLeaved;//게임중에 나감 (다른사람이)

        /** Called when user index was changed
            @param idx new index
        */
        public event OnIndexChangedHandler OnIndexChanged;

        /** Called when host received network test results
            @count test count
            @time 
        */
        public event OnNetworkReportedHandler OnNetworkReported;//안쓰일듯

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
        public event OnAckFailedHandler OnAckFailed; //x

        /** */
        public event OnHostDisconnectedHandler OnHostDisconnected;

        public UXConnectController()
            : base("UXConnectController")
        {
            receiveQueue = new List<string>();
            systemUID = SystemInfo.deviceUniqueIdentifier;
			connect = UXRoomConnect.Instance;
			room = UXRoom.Instance;
			player = UXPlayerController.Instance;
            PlayerPrefs.SetInt("ServerList", 0); //china
            ServerCheck((ServerList)PlayerPrefs.GetInt("ServerList"));
        }


        public static void ServerCheck(ServerList str)
        {//서버체크(중국or싱가폴)
            Debug.Log("ROOM_SERVER_PORT:" + ROOM_SERVER_PORT);

            switch (str)
            {
                case ServerList.CN:
                    {
                        //ROOM_SERVER_IP = "192.168.0.81";//"112.74.40.64";
                        //BASE_REST_URL = "http://192.168.0.81:3000";// 6000";
						ROOM_SERVER_IP = "112.74.40.64";//"112.74.40.64";
						BASE_REST_URL = "http://112.74.40.64:3000";// 6000";
                    }
                    break;
                case ServerList.SG:
                    {
                        ROOM_SERVER_IP = "52.76.82.48";
                        BASE_REST_URL = "http://52.76.82.48:3000";
                    }
                    break;
            }
            Debug.Log("BASE_REST_URL:" + BASE_REST_URL);
        }

        /** Inìtialization 
            @warning This method msub be called before using UXConnectController
        */
        public void Init()
        {
            if (connect != null)
            {
                connect.OnReceived += OnMessageReceived;
                connect.OnServerConnected += OnServerConnected;
                connect.OnServerConnectFailed += OnServerConnectFailed;
                connect.OnServerDisconnected += OnServerDisconnected;
                connect.OnServerError += OnServerError;
            }
        }

        /** Release event handler and disconnect socket	*/
        public void Clear()//연결끊을때 사용
        {

            if (connect != null)
            {
                connect.OnReceived -= OnMessageReceived;
                connect.OnServerConnected -= OnServerConnected;
                connect.OnServerConnectFailed -= OnServerConnectFailed;
                connect.OnServerDisconnected -= OnServerDisconnected;
                connect.OnServerError -= OnServerError;

                Disconnect();
            }
            connect = null;
            UXLog.Close();
        }

        public bool InitAckSender(string title = "")
        {//안쓰나봐
            if (connect == null)
            {
                return false;
            }

            isSendAck = true;

            ackSender = new UXAckSender();

            if (title != "")
            {
                ackSender.ChangeCommandTitle(title);
            }
            ackSender.SetConnect(connect);

            return true;
        }

        public void StartAckSender()
        {//안쓰는듯
            if (ackSender == null)
            {
                return;
            }
            //			ackSender.Start ();
        }

        public void StopAckSender()
        { //LogoutPopup-OnYesButtonUp()에서 씀. //로그아웃을 안해
            if (ackSender == null)
            {
                return;
            }
            ackSender.Stop();
        }

        public void Run()
        {
            if (isSendAck == true && ackSender != null)
            {
                if (ackSender.CheckAckCount() == false)
                {
                    if (OnAckFailed != null)
                    {
                        OnAckFailed();
                    }
                }
            }
        }

        public UXRoomConnect GetRoomConnect()
        {
            return connect;
        }

        /** Return formatted room number */
        public static string GetRoomNumberString()//string형 roomNumber반환
        { 
          /*  if (launcherCode == -1)
            {
                return "";
            }

          	return string.Format("{0:D5}", launcherCode); // 25 ->"00025"*/
			return room.RoomNumber;
        }

        /** Return room number */
        public static int GetRoomNumber()
        {//int형 roomNumber반환
            return launcherCode;
        }

        /** Set room number */
        public static void SetRoomNumber(int room)
        {
            launcherCode = room;
        }

        /** Set connection mode
            @warning It must be called before connection
            @see Mode
        */
        public static void SetMode(Mode mode) //static으로 바꿈
        {
            connectMode = mode;
        }

		public static Mode GetMode() //새로만듦
		{
			return connectMode;
		}

        public void SetLanguage(Language lan)
        {
            selectLanguage = lan;
        }


        public string GetLanguage()
        {
            return selectLanguage.ToString();
        }

        /** Connect to room server
            @brief connect, if socket is not connected
            @param hostIP Server IP Address
            @param port Server Port
        */

        public void Connect(int serverPort = ROOM_SERVER_PORT)//서버와 연결
        {

            string serverID = ROOM_SERVER_IP;


            // RoomConnect 객체가 생성 되었는지 확인, 생성되지 않았으면 새로 생성함
            if (connect == null)
            {
                connect = new UXRoomConnect();
                Init();
                SetMode(Mode.Client);
            }

            // 현재 서버와 연결되어있는지 확인, 연결되어 있지않으면 Room Server와 연결함  //연결요청
            if (connect.IsConnected() == false)
            {
                bool result = connect.SocketOpen(serverID, serverPort);
            }
        }

        /** Close connection */
        public void Disconnect()
        {
            isJoined = false;
            connect.Disconnect();
        }

        /** Return host connection state
            @param True if host is joined, false otherwise
        */
        public bool IsHostJoined()
        {
            return isHostJoined;
        }

        /** Return user index from user code
            @param code user code
            @return User index. if it can't be found then return -1
        */
        public int GetUserIndexFromCode(int code)
        {
            UXUserController userController = UXUserController.Instance;

            for (int i = 0; i < userController.GetCount(); i++)
            {
                UXUser user = (UXUser)userController.GetAt(i);
                if (code == user.GetCode())
                {
                    return i;//user index로 사용됨 index = GetUserIndexFromCode()이케//index : 0,1,2,3 +1 = 1,2,3,4P
                }
            }

            return -1;
        }

        public bool IsEventExist(Event handler)
        { //안쓰ㄴ다
            if (handler == null)
            {
                return false;
            }

            return true;
        }

        /** Set debug mode
            @param debug mode
            @see UXRoomConnect.DebugMode
        */
        public void SetDebugMode(UXRoomConnect.DebugMode mode)
        {//안써
            UXRoomConnect.debugMode = mode;
        }

        /** Return debug mode
            @return debug mode
            @see UXRoomConnect.DebugMode
        */
        public UXRoomConnect.DebugMode GetDebugMode()
        {//안사용
            return UXRoomConnect.debugMode;
        }

        /** Return current debug string
            @return dubug message
            @see UXRoomConnect.debugString
            @todo add all network error
        */
        public static string GetDebugString()
        {//안씀
            return UXRoomConnect.debugString; 
        }

        /** Return current connection mode
            @return connection mode
            @see Mode
        */
        public Mode GetConnectMode()
        {//안써
            return connectMode;
        }

        /** Return connnection state  
            @return True if it was connected, false otherwize
        */
        public bool IsConnected()
        {
            if (connect == null)
            {
                return false;
            }

            return connect.IsConnected();
        }

        /** Return join state
            @return True if user is joined, false otherwise
        */
        public bool IsJoined()
        {
            return isJoined;
        }

        /** Send game start message
            @see OnGameStartHandler
        */
        public void SendStartGame()
        {
            string sendString = "{\"cmd\":\"start_game\",\"l_code\":\"" + launcherCode + "\"}" + DATA_DELIMITER;
            Send(sendString);    //{"cmd:"start_game",l_code":"launcherCode"}232
        }

        /** Send game restart message
            @see OnGameRestartHandler
        */
        public void SendRestartGame()
        {
            string sendString = "{\"cmd\":\"restart_game\",\"l_code\":\"" + launcherCode + "\"}" + DATA_DELIMITER;
            Send(sendString);    //{"cmd":"restart_game","l_code":"launcherCode"}232
        }

        /** Send game result message
            @see OnGameResultHandler
        */
        public void SendResultGame()
        {//안쓴다
            string sendString = "{\"cmd\":\"result_game\",\"l_code\":\"" + launcherCode + "\"}" + DATA_DELIMITER;
            Send(sendString);    //{"cmd":"result_game","l_code":"launcherCode"}232
        }

        /** Send game end message
            @see OnGameEndHandler
        */
        public void SendEndGame()
        {
            string sendString = "{\"cmd\":\"end_game\",\"l_code\":\"" + launcherCode + "\"}" + DATA_DELIMITER;
            Send(sendString);    //{"cmd":"end_game","l_code":"launcherCode"}232
        }

        /** Send game exit message
            @see OnGameExitHandler
        */
        public void SendExit()
        {
            string type = "host";
            int userCode = -1;

            if (connectMode == Mode.Client)
            {
                type = "user";
                userCode = UXPlayerController.Instance.GetCode();
            }

            string sendString = "{\"cmd\":\"exit\",\"type\":\"" + type + "\",\"l_code\":\"" + launcherCode + "\",\"u_code\":\"" + userCode + "\"}" + DATA_DELIMITER;
            Send(sendString);  //{"cmd":"exit","type":"type","l_code":launcherCode","u_code":userCode",}232
        }

        /** Send changed user index
            @param user code
            @param idx changed index
        */
        public void SendUserIndex(int userCode, int idx)
        { //안쓰이는듯
            if (connectMode == Mode.Client)
            {
                return;
            }

            string msg = "{\"cmd\":\"update_user_index\",\"u_code\":\"" + userCode + "\",\"l_code\":\"" + launcherCode + "\",\"index\":\"" + idx + "\"}" + DATA_DELIMITER;
            Send(msg); //{"cmd":"update_user_index","u_code":"userCode","l_code":"launcherCode","index":"idx"}232
        }

        /** Broadcast data
            @param msg sending data
        */
        public void SendData(string msg)
        {
            string sendString = "{\"cmd\":\"broadcast\",\"data\":";
            sendString += GetSendDataFormat(msg);
            sendString += "}" + DATA_DELIMITER;

            connect.Send(sendString);//{"cmd":"broadcast","data":GetSendDataFormat(msg)}232
        }

        /** Send data to specific user
            @param userIndex user index
            @param msg sending data
        */
        public void SendDataTo(int userIndex, string msg)
        {
            UXUser user = (UXUser)UXUserController.Instance.GetAt(userIndex);

            if (user.IsConnected() == true)
            {
                SendDataToCode(user.GetCode(), msg);
            }
        }

        /** Send data to specific user
            @param user UXUser Type
            @param msg sending data
            @see UXUser
        */
        public void SendDataTo(UXUser user, string msg)
        {//sendDataToUser
            int userIndex = GetUserIndexFromCode(user.GetCode());
            SendDataTo(userIndex, msg);
        }

        /** Send data to specific user
            @param target user Code
        */
        public void SendDataToCode(int target, string msg)
        {//sendDataToUser2
            //string sendString = "{\"cmd\":\"send_target\",\"target\":[\"" + target + "\"],\"data\":";
			string sendString = "{\"cmd\":\"send_target\",\"target\":[" + target + "],\"data\":";
            sendString += GetSendDataFormat(msg);
            sendString += "}" + DATA_DELIMITER;
            //{"cmd":"send_target","target":["target"],"data":GetSendDataFormat(msg)}232
            connect.Send(sendString);
        }

        /** Send data to host
            @param msg sending data
        */
        public void SendDataToHost(string msg)
        {
            string sendString = "{\"cmd\":\"send_host\",\"data\":";
            sendString += GetSendDataFormat(msg);
            sendString += "}" + DATA_DELIMITER;
            connect.Send(sendString);    //{"cmd":"send_host","data":GetSendDataFormat(msg)"}
        }


		public void SendPremium(bool isPremium){
			player.IsPremium = isPremium;
		}



        /** Send raw data to server
            @param msg
            @warning Use if you know the protocol
        */
        public void Send(string msg)
        {
            connect.Send(msg);
        }


        /** Test network speed and send the data to host
            @param count sending count
            @param sendResult True if want to broadcat the result, false otherwise
        */
        public void NetworkTest(int count, bool sendResult = false)
        {//안쓰는듯
            UXPlayerController player = UXPlayerController.Instance;

            isSendNetWorkResult = sendResult;
            networkCheckCount = count;
            networkCheckValues = new long[count];

            for (int i = 0; i < count; i++)
            {
                string sendString = "{\"cmd\":\"check_network_state\",\"u_code\":\"" + player.GetCode() + "\",\"l_code\":\"" + launcherCode + "\",\"count\":\"" + (i + 1) + "\",\"time\":\"" + DateTime.Now.Ticks + "\"}" + DATA_DELIMITER;
                connect.Send(sendString);
                //{"cmd":"check_network_state","u_code":"player.GetCode()","l_code":"launcherCode","count":"(i+1)","time":"DataTime.Now.Ticks"}232

                System.Threading.Thread.Sleep(50);
            }
        }

        /** Request for user list
            @see OnUserListReceived
        */
        public void RefreshUserListFromServer()
        {
           UXPlayerController player = UXPlayerController.Instance;

            string msg = "{\"cmd\":\"get_user_list\",\"l_code\":\"" + launcherCode + "\"}" + DATA_DELIMITER;
            Send(msg);//{"cmd":"get_user_list","l_code":"launcherCode"}232
			Debug.Log("RefreshUserListFromServer!!");
        }

        /** Max user set //^0^
            @param max user
        */
        public void SendMaxUser(int maxUser) //이거 여기서 안쓰고 host에서 쓰느?????
        {
           /* UXPlayerController player = UXPlayerController.Instance;

            string sendString = "{\"cmd\":\"max_user_set\",\"max_client\":\"" + maxUser + "\",\"l_code\":\"" + launcherCode + "\",\"u_code\":\"" + player.GetCode() + "\"}" + DATA_DELIMITER;
            Send(sendString);   // {"cmd":"max_user_set","max_client":" maxUser","l_code":"launcherCode","u_code":"player.GetCode()"}232
            */
			room.MaxUser = maxUser;
        }


        //data처음끝에 "를 붙여준다. data->"data"
        string GetSendDataFormat(string data)
        {
            bool isAddDoubleQuotationMarks = true;

            if (data.StartsWith("{") || data.StartsWith("["))
            {
                isAddDoubleQuotationMarks = false;
            }

            string sendData = "";

            if (isAddDoubleQuotationMarks == true)
            {
                sendData += "\"";
            }
            sendData += data;
            if (isAddDoubleQuotationMarks == true)
            {
                sendData += "\"";
            }

            return sendData;
        }


        void OnMessageReceived(byte[] data) //데이터가 오면 발생하는 이벤트
        {
            //TODO : UXProtocolParser 사용해야 할 곳
            
            /*if (string.IsNullOrEmpty(prevReceivedData) == false)
            { // 이전에 받은 데이터가 있으면
                data = prevReceivedData + data;
            }
            string[] datas = data.Split(DATA_DELIMITER);
            lock (receiveQueue)
            {
                for (int i = 0; i < datas.Length - 1; i++)
                {
                    if (datas[i].Substring(0, 1) == "{")
                        receiveQueue.Add(datas[i]);//자른 data를 큐에 넣어준다
                }
            }
            if (string.IsNullOrEmpty(datas[datas.Length - 1]) == false)
            { //data[마지막]에 데이터가 있으면
                prevReceivedData = datas[datas.Length - 1];               //prevReceivedData에 넣어줌.
            }
            else
            {
                prevReceivedData = null;
            }
            */

            lock (receiveQueue)
            {
                int offset = 0;
				int ackBlockCount = 0;
                while (offset < data.Length)
                {
					//Debug.Log (offset + ", " + data.Length + ", " + data [offset]);
                    byte command = data[offset++];
                    byte len = data[offset++];

					if (command == 0) {
						if (ackBlockCount > 0)
							break;
						ackBlockCount++;
					}

                    List<byte> msg = new List<byte>();

                    //Debug.Log(command);

                    msg.Add(command);
                    msg.Add(len);

                    for (int i = 0; i < len; i++)
                    {
                        msg.Add(data[offset++]);
                    }

                    var N = UXProtocol.Instance.ParserFactory(command).Parse(msg.ToArray());
                    //Debug.Log(N.ToString());
                    receiveQueue.Add(N.ToString());
                }
            }
            
        }

        void OnServerConnected()
        {
            if (OnConnected != null)
            {
                OnConnected();
            }
        }

        void OnServerConnectFailed() //소켓에서 연결 실패
        {
            if (OnConnectFailed != null)
            {
                OnConnectFailed();
            }
        }

        void OnServerDisconnected() //끊을때
        {
            if (OnDisconnected != null)
                OnDisconnected();
        }

        void OnServerError(int err, string msg)
        {
            if (OnError != null)
            {
                OnError(err, msg);
            }
        }

		protected string ParseUser(string user)
		{
			string[] fixedUser = user.Split ('.');

			return fixedUser[fixedUser.Length-1];
		}
			
        protected List<UXUser> ParseUserList(JSONArray users)
        {
            List<UXUser> userList = new List<UXUser>();

            for (int i = 0; i < users.Count; i++)
            {
                string temp = users[i]; //123.name //6.Player 1

                string[] info = temp.Split('.'); //[o]=123;[1]=name;
                UXUser userObj = new UXUser(info[1], Int32.Parse(info[0])); //(name,code)
                userObj.SetConnected(true);
                userList.Add(userObj);
            }

            return userList;
        }
        //join실패 이유 (join event 중에서)반환
        protected int ProcessConnectError(int result) //ProcessConnectError(UXErrorCode.JE_FAIL);
        {
            isJoined = false;
            int errCode = JE_FAIL;

            if (result == UXErrorCode.RS_ERROR_MAX_USER)
            {
                errCode = JE_MAX_USER;
            }
            else if (result == UXErrorCode.RS_ERROR_ALREADY_START)
            {
                errCode = JE_ALREADY_START;
            }

            return errCode;
        }

        protected void ProcessReceivedMessage(string data)
        {
            if (string.IsNullOrEmpty(data) == true || data.Length <= 0)
            {
                Debug.Log("Empty"); //data 비어있떠
                return;
            }

			Debug.Log("UXConnectController ProcessReceivedMessage data : " + data);

            var N = JSON.Parse(data);
            string command = N["cmd"];

			if (command == "ack_result") {
				if (isSendAck == true && ackSender != null) { //x
					ackSender.ReceiveResult ();
				}
			} else if (command == "user_add") {
				int code = N ["u_code"].AsInt;
				string name = ParseUser(N ["name"]);
				UXUser userObj = new UXUser (name, code);

				Debug.Log ("UserAdd : " + name);
				  
				room.AddUser (userObj);
             
				var array = N ["user_list"];

				if (array != null) {
					List<UXUser> list = ParseUserList ((JSONArray)array); //리스트 갱신

					room.UpdateUserList (list);
				}

				if (OnUserAdded != null) {
					int userIndex = GetUserIndexFromCode (code);
					OnUserAdded (userIndex, code); //playerCount 갱신, index 증가 -> index는 접속할 유저에게 할당할 index  +이벤트 하나더 (로비매니저)

				}
			} else if (command == "user_del") {
				Debug.Log ("UserDel: ");
				int code = N ["u_code"].AsInt;
				room.RemoveUser (GetUserIndexFromCode (code));

				var array = N ["user_list"];

				if (array != null) {
					List<UXUser> list = ParseUserList ((JSONArray)array);

					room.UpdateUserList (list);
				}

				if (OnUserRemoved != null) {
					int userIndex = GetUserIndexFromCode (code);
					OnUserRemoved (name, code);
				}

			} else if (command == "update_user_index_result") { // 사용되고 있음
				int index = N ["index"].AsInt;

				UXPlayerController player = UXPlayerController.Instance;
				player.SetIndex (index);
				if (OnIndexChanged != null) {
					UXLog.SetLogMessage (" onindexchanged");
					OnIndexChanged (index);
				}
			} else if (command == "send_error") {
			} else if (command == "exit_result") {
				if (OnExit != null) {
					OnExit ();
				}
			} else if (command == "host_close") {

				if (OnHostDisconnected != null) {
					OnHostDisconnected ();
				}
			} else if (command == "data") {
				string val = N ["data"].Value.ToString ();

				if (val == "") {
					val = N ["data"].ToString ();
				}

				int senderCode = N ["sender"].AsInt;
				int userIndex = GetUserIndexFromCode (senderCode);

				if (OnReceived != null) {
					OnReceived (userIndex, val);
				}
			} else if (command == "check_network_state_result") {	//이거 안올듯. 위에서 cmd:check_network_state를안보내
				int cur = N ["count"].AsInt;
				string temp = N ["time"];
				long stime = long.Parse (temp);

				networkCheckValues [cur - 1] = DateTime.Now.Ticks - stime;

				if (cur >= networkCheckCount) {
					float totalTime = 0;
					for (int i = 0; i < networkCheckCount; i++) {
						totalTime += networkCheckValues [i];
					}

					if (OnNetworkReported != null) {
						OnNetworkReported (networkCheckCount, totalTime);
					}

					UXPlayerController player = UXPlayerController.Instance;

					if (isSendNetWorkResult == true) {
						string sendString = "{\"cmd\":\"report_network_state\",\"u_code\":\"" + player.GetCode () + "\",\"l_code\":\"" + launcherCode + "\",\"count\":\"" + networkCheckCount + "\",\"time\":\"" + totalTime + "\"}&";
						Send (sendString);
					}
				}
			} else if (command == "start_game_result") {
				isGameStarted = true;

				if (OnGameStart != null) {
					OnGameStart ();
				}
			} else if (command == "restart_game_result") {
				isGameStarted = true;

				if (OnGameRestart != null) {
					OnGameRestart ();
				}
			} else if (command == "result_game_result") {//안쓰일듯
				if (OnGameResult != null) {
					OnGameResult ();
				}
			} else if (command == "end_game_result") {
				isGameStarted = false;

				if (OnGameEnd != null) {
					OnGameEnd ();
				}
			} else if (command == "host_joined") {
				isHostJoined = true;

				if (OnHostJoined != null) {
					OnHostJoined ();
				}
			} else if (command == "get_user_list_result") {
				Debug.Log ("UserList: ");
				List<UXUser> userList = ParseUserList ((JSONArray)N ["user_list"]);
				UXUserController userController = UXUserController.Instance;
               

				if (userController.IsEqual (userList) == false) {
					userController.CopyList (userList);

					if (OnUserListReceived != null) {
						OnUserListReceived (userList); //log찍는듯
					}
				}
			
				if (connectMode == UXConnectController.Mode.Client) { //PAD면
					UXPlayerController playerController = UXPlayerController.Instance;
					for (int i = 0; i < userController.GetCount (); i++) {
						UXUser user = (UXUser)userController.GetAt (i); //유저마다
						if (playerController.GetCode () == user.GetCode ()) { //코드를 비교 해서 나를 찾는거
							if (i != playerController.GetIndex ()) {
								if (OnIndexChanged != null) {
									OnIndexChanged (i); 
								}

								playerController.SetIndex (i); 
								Debug.Log ("SetInedx : " + i);
							}
							break;
						}
					}
				}

			} else if (command == "update_ready_count_result") {
				int ready = N ["ready"].AsInt;
				int total = N ["total"].AsInt;

				if (OnUpdateReadyCount != null) {
					OnUpdateReadyCount (ready, total);
				}
			}

        }
    }
}
