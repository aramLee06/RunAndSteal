using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Timers;
using UXLib.Base;
using UXLib.Connect;
using UXLib.User;
using UXLib.Util;
using SimpleJSON;
namespace UXLib
{
	public class UXStoreLauncherController : UXConnectController
	{
		/** 조인시 오류 정의 */
		public const int JOIN_ERROR_NOT_LOGIN = 1; /**< 사용자가 로그인을 안했을 경우 */
		public const int JOIN_ERROR_SERVER = 2; /**< 서버에서 오류 처리되었을 경우 */
		/// REST 서버 URL
		
		static string REQUEST_ROOM_NUMBER = "g r n";
		//		public const string LAUNCHER_SERVER_IP = "52.69.126.120"; /**< Launcher server ip address In japan*/
		public const string LAUNCHER_SERVER_IP = "112.124.65.169";  /**< Launcher server ip address In China*/
		public const int LAUNCHER_SERVER_PORT = 5000; /**< Launcher server port */
		int userCode;
		int launcherPlatform;
		string launcherToken;
		int launcherTotalGame;
		List<int> gameCode;
		public delegate void OnButtonClickHandler(string data);
		public delegate void OnLauncherExitHandler();
		public delegate void OnUserEmptyHandler();
		public event OnUserEmptyHandler OnUserEmpty;
		public event OnLauncherExitHandler OnLauncherExit;
		public event OnButtonClickHandler OnButtonClicked;
		/** 서버 연결시 호출된다 */
		//public event OnConnectFailedHandler OnConnectFailed;
		/** 런처에 조인했을 경우에 호출 된다. */
		public event OnJoinSucceededHandler OnJoinSucceeded;
		/** 런처 조인에 실패했을 경우에 호출된다. */
		public event OnJoinFailedHandler OnJoinFailed;
		/** 싱글턴 */
		private static UXStoreLauncherController instance = null;
		public static UXStoreLauncherController Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new UXStoreLauncherController();
				}
				return instance;
			}
		}
		private UXStoreLauncherController()
		{
			Init();
			SetMode(UXConnectController.Mode.Host);
			gameCode = new List<int>();
		}
		/** 서버에 연결 한다 */
		public void Connect()
		{
			base.Connect();
		}
		/** 메시지큐 처리. 항시 불려져야 한다 */
		public void Run()
		{
			base.Run();
			lock (receiveQueue)
			{
				int count = receiveQueue.Count;
				for (int i = 0; i < count; i++)
					ProcessReceivedMessage(receiveQueue[i]);
				receiveQueue.Clear();
			}
		}
		public bool CreateRoom()
		{
			JSONClass json = new JSONClass();
			json.Add("id", REQUEST_ROOM_NUMBER);
			json.Add("pw", "");
			string recData = UXRestConnect.Request("launchers/token", UXRestConnect.REST_METHOD_POST, json.ToString());
			if (recData == null)
			{
				return false;
			}
			var N = JSON.Parse(recData);
			int rec = N["gp_ack"].AsInt;
			bool result = (rec == UXRoomConnect.ACK_RESULT_OK);
			if (result == true)
			{
				launcherCode = N["l_code"].AsInt;
			}
			else
			{
				return false;
			}
			return result;
		}
		public string getGameList()
		{
			JSONClass json = new JSONClass();
			string recData = UXRestConnect.Request("/games?type=download&launcher_uid=" + launcherCode + "&start_idx=0&offset=100", UXRestConnect.REST_METHOD_GET, json.ToString());
			return recData;
		}
		public string getGameInfo(int gameCode)
		{
			JSONClass json = new JSONClass();
			string recData = UXRestConnect.Request("/games/desc/?game_uid=" + gameCode + "&launcher_uid=" + launcherCode, UXRestConnect.REST_METHOD_GET, json.ToString());
			return recData;
		}
		public int GetLauncherCode()
		{
			return launcherCode;
		}
		public void SetLauncherCode(int code)
		{
			launcherCode = code;
		}
		public int GetUserCode()
		{
			return userCode;
		}
		public int GetlauncherTotalGame()
		{
			return launcherTotalGame;
		}
		public void SetlauncherTotalGame(int cnt)
		{
			//launcherTotalGame = cnt+1;
			launcherTotalGame = cnt;
		}
		public void setGameCode(int result)
		{
			gameCode.Add(result);
		}
		public List<int> getGameCode()
		{
			return gameCode;
		}
		public override void Join(string data)
		{
			string sendString = "{\"cmd\":\"join_launcher\",\"l_code\":\"" + launcherCode + "\",\"package_name\":\"" + data + "\"}" + DATA_DELIMITER;
			UXConnectController.SetRoomNumber(launcherCode);
			UXLog.SetLogMessage("sen" + sendString);
			Send(sendString);
		}
		/** Send leave message */
		public void Leave()
		{
			string sendString = "{\"cmd\":\"leave_launcher\",\"l_code\":\"" + launcherCode + "\"}" + DATA_DELIMITER;
			Send(sendString);
		}
		public void sendBackBtnResult(string page)
		{
			string sendString = "{\"cmd\":\"pad_button_touch_result\",\"gp_ack\":\"0\",\"u_code\":\"" + userCode + "\",\"l_code\":\"" + launcherCode + "\",\"button_type\":\"back\",\"store_page\":\"" + page + "\"}" + DATA_DELIMITER;
			Send(sendString);
		}
		public void sendClickBtnResult(string info, string gameName, string padPackageName, string padPackageVersion)
		{
			string sendString = "{\"cmd\":\"pad_button_touch_result\",\"gp_ack\":\"0\",\"g_name\":\"" + gameName + "\",\"u_code\":\"" + userCode + "\",\"l_code\":\"" + launcherCode + "\",\"button_type\":\"click\",\"game_appinfo\":\"" + info + "\",\"pad_package\":\"" + padPackageName + "\",\"pad_version\":\"" + padPackageVersion + "\"}" + DATA_DELIMITER;
			Send(sendString);
		}
		public void sendOkBtnResult(string padPackageName)
		{
			string sendString = "{\"cmd\":\"pad_button_touch_result\",\"gp_ack\":\"0\",\"u_code\":\"" + userCode + "\",\"l_code\":\"" + launcherCode + "\",\"button_type\":\"ok\",\"pad_package\":\"" + padPackageName + "\"}" + DATA_DELIMITER;
			Send(sendString);
		}
		public void sendExitResult()
		{
			string sendString = "{\"cmd\":\"exit_launcher_result\",\"gp_ack\":\"0\",\"l_code\":\"" + launcherCode + "\"}" + DATA_DELIMITER;
			Send(sendString);
		}
		List<UXUser> ParseUserList(JSONArray users) {
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
		void ProcessReceivedMessage(string data)
		{
			if (string.IsNullOrEmpty(data) == true)
			{
				return;
			}
			var N = JSON.Parse(data);
			string command = N["cmd"];
			bool isRunBaseProcess = false;
			if (command == "join_launcher_result")
			{
				int rec = N["gp_ack"].AsInt;
				bool result = (rec == UXRoomConnect.ACK_RESULT_OK);
				if (result == true)
				{
					isHostJoined = true;
					if (OnJoinSucceeded != null)
					{
						OnJoinSucceeded(isHostJoined);
					}
				}
				else
				{
					isHostJoined = false;
					if (OnJoinFailed != null)
					{
						OnJoinFailed(JOIN_ERROR_SERVER);
					}
				}
			}
			else if (command == "pad_button_touch")
			{
				int rec = N["gp_ack"].AsInt;
				bool result = (rec == UXRoomConnect.ACK_RESULT_OK);
				if (result == true)
				{
					int user = N["u_code"].AsInt;
					userCode = user;
					string str = N["button_type"];
					if (str == "right")
					{
						OnButtonClicked("right");
					}
					else if (str == "left")
					{
						OnButtonClicked("left");
					}
					else if (str == "up")
					{
						OnButtonClicked("up");
					}
					else if (str == "down")
					{
						OnButtonClicked("down");
					}
					else if (str == "click")
					{
						OnButtonClicked("click");
					}
					else if (str == "back")
					{
						OnButtonClicked("back");
					}
					else if (str == "ok")
					{
						OnButtonClicked("ok");
					}
					else if (str == "market")
					{
						OnButtonClicked("market");
					}
					else if (str == "qr")
					{
						OnButtonClicked("qr");
					}
				}
				else
				{
				}
			}
			else if (command == "exit_launcher")
			{
				int rec = N["gp_ack"].AsInt;
				bool result = (rec == UXRoomConnect.ACK_RESULT_OK);
				if (result == true)
				{
					OnLauncherExit();
				}
			}
			else if (command == "get_user_list_result") {
				
				List<UXUser> userList = ParseUserList((JSONArray)N["list"]);
				
				if(userList.Count == 0){
					OnUserEmpty();
				}
			}
			else
			{
				isRunBaseProcess = true;
			}
			if (isRunBaseProcess == true)
			{
				base.ProcessReceivedMessage(data);
			}
		}
	}
}