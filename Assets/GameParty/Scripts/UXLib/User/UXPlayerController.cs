using UnityEngine;
using System.Collections;
using System.IO;
using System;

using UXLib.Base;
using UXLib.Connect;
using SimpleJSON;
using UXLib.Util;

namespace UXLib.User {
	
	public class UXPlayerController : UXObject {

		int index;
		int code;

		string token;
		string imageURL;
		UXRoomConnect roomConnect;
		UXUser.LobbyState lobbyState;
		string lastReceivedData;
		
		bool isLauncherLogin;
		bool isUserLogin;
		public static char DATA_DELIMITER = (char)232;//여기서 자름

		private bool isPremium = false; //현재 플레이어가 프리미엄 버전인지에 대한 여부 (초기값 false)
		public bool IsPremium
		{
			get{
				return isPremium;
			}
			set{
				this.isPremium = value;
				if (isPremium) {
					string sendString = "{\"cmd\":\"premium_user\",\"u_code\":\"" + GetCode () + "\",\"l_code\":\"" + UXConnectController.room.RoomNumber+ "\"}" + DATA_DELIMITER;
					roomConnect.Send (sendString);
				}
			}
		} 

		private static UXPlayerController instance = null;
		public static UXPlayerController Instance {
			get {
				if (instance == null) {
					instance = new UXPlayerController();
				}
				return instance;
			}
		}
		
		private UXPlayerController() : base("player") {
			isLauncherLogin = false;
			isUserLogin = false;
			roomConnect = UXRoomConnect.Instance;

			name = "";
			code = -1;
		}

		public int GetCode() { return code; }
		public string GetToken() { return token; }
		public int GetIndex() { return index; }

		public void SetCode(int val) { code = val; }
		public void SetToken(string val) { token = val; }
		public void SetIndex(int idx) { index = idx; }

		public void SetTestMode() {
			if (code != -1) {
				return;
			}
			
			code = -2;
		}


		/** Login
			@param lid login id
			@param passwd login password 
			@return True if login was successful, false otherwise
		*/ 

		string deviceNumber = SystemInfo.deviceUniqueIdentifier;

		public int GetUserCodeFromServer() {
			string recData = UXRestConnect.Request("user/uuid", UXRestConnect.REST_METHOD_GET, ""); 
			Debug.Log (recData);

			var N = JSON.Parse(recData);
			code = N["uuid"].AsInt;
			
			isUserLogin = true;
			
			return UXRestConnect.RESULT_TRUE;
		}

		/** Not used */
		public bool IsLauncherLogin() {
			return isLauncherLogin;
		}
		
		/** Return login state
			@return True if user is loging in, false otherwise
		*/
		public bool IsUserLogin() {
			return isUserLogin;
		}

		/** Change user's lobby state
			@param state lobby state
			@see UXUser.LobbyState 
		*/	
		public void SetLobbyState(UXUser.LobbyState state) {
			lobbyState = state;
		}
		
		/** Get user's lobby state
			@reutrn 상태
			@see UXUser.LobbyState 
		*/
		public UXUser.LobbyState GetLobbyState() {
			return lobbyState;
		}
		
		/** Get last received data
			@reutrn last received data
		*/
		public string GetLastReceivedData() {
			return lastReceivedData;
		}
		

	}
}	
