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
		
		UXUser.LobbyState lobbyState;
		string lastReceivedData;
		
		bool isLauncherLogin;
		bool isUserLogin;

		
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
			JSONClass json = new JSONClass();
			json.Add ("mac", deviceNumber);
			
			string recData = UXRestConnect.Request("users/token", UXRestConnect.REST_METHOD_POST, json.ToString()); 
			if (recData == null) {
				return UXRestConnect.RESULT_ERROR;
			}
			
			lastReceivedData = recData;
			
			var N = JSON.Parse(recData);
			int rec = N["gp_ack"].AsInt;
			
			bool result	= (rec == UXRoomConnect.ACK_RESULT_OK);
			
			if (result == true) { 

				code = N["u_code"].AsInt;
				token = N["token"];
				
				isUserLogin = true;
				return UXRestConnect.RESULT_TRUE;
			} 
			
			isUserLogin = false;
			
			return UXRestConnect.RESULT_FALSE;
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
