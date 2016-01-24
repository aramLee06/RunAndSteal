using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Timers;

using UXLib;
using UXLib.Base;
using UXLib.Connect;
using UXLib.User;
using UXLib.Util;

using SimpleJSON;

namespace UXLib {
	public class UXPlayerLauncherController : UXConnectController {
		public static char DATA_DELIMITER = (char)232;
		public const int JOIN_ERROR_NOT_LOGIN = 1; /**< user is not joined */
		public const int JOIN_ERROR_SERVER = 2; /**< error occured from server */

//		public const string LAUNCHER_SERVER_IP = "52.76.86.12";  /**< Launcher server ip address In Mer*/
//		public const string LAUNCHER_SERVER_IP = "52.69.126.120";  /**< Launcher server ip address In japan*/
		public const string LAUNCHER_SERVER_IP = "112.124.65.169";  /**< Launcher server ip address In China*/
		public const int LAUNCHER_SERVER_PORT = 5000; /**< Launcher server port */

		bool isQueueRunning = false;
		bool isActive;
		bool isJoin;
		bool isAutoJoin;
		bool isPlay = false;
		string bigscreenState = null;

		int sendAckTime;
		public static int ackSendCount;
		
		public static string padPackage;
		public static string version;

		public delegate void OnUserLeftHandler();
		public delegate void OnFirstUserHandler();
		public delegate void OnChangeFirstUserHandler();
		public delegate void OnStartGamePadHandler(string padPackage);
		public delegate void OnReceivedHandler(string data);
		public delegate void OnStorePageStateHandler(string storePage);
		public delegate void OnBigscreenGameInfoHandler(string gameInfo);
		

		/** Called when user left */
		public event OnUserLeftHandler OnUserLeft;

//		/** Called when message was received */
//		public event OnReceivedHandler OnReceived;
		
		/** Called when user joined */
		public event OnJoinSucceededHandler OnJoinSucceeded;
		
		/** Called when user failed to join */
		public event OnJoinFailedHandler OnJoinFailed;
		
		/** Called when user be a first user */
		public event OnFirstUserHandler OnFirstUser;
		public event OnChangeFirstUserHandler OnChangeFirstUser;
		
		/** Called when game is started */
		public event OnStartGamePadHandler OnStartGamePad;

		//		public event OnUdateBigScreenHandler OnUpdateBigScreen;
		public event OnStorePageStateHandler OnStorePageState;
		public event OnBigscreenGameInfoHandler OnBigscreenGameInfo;

		
		private static UXPlayerLauncherController instance = null;
		
		/** Singletone 
			@reeturn Instance of UXPlayerLauncherController
		*/
		public static UXPlayerLauncherController Instance {
			get {
				if (instance == null) {
					instance = new UXPlayerLauncherController();
				}
				return instance;
			}
		}
		
		private UXPlayerLauncherController() {
			Init();
			SetMode (UXConnectController.Mode.None);
			//receiveQueue = new List<string>();
			isJoin = false;
			isAutoJoin = false;
		}

		/** Connect to server */
		public void Connect() {
			Debug.Log ("launcher Conn");
			base.Connect ();
		}

		public void Run() {

			lock(receiveQueue)
			{
				int count = receiveQueue.Count;
				if (count == 0) {
					return;
				}
				
				for (int i = 0; i < count; i++) {
					ProcessReceivedMessage(receiveQueue[i]);
				}
				receiveQueue.Clear();
			}
		}

		public bool PlayState(){
			return isPlay;
		}
		public string PadPackageName(){
			return padPackage;
		}

		/** Send message to server
			@param data left,right - Scroll direction, click - click index
			@param buttonType "ok", "up", "down", "left", "right", "back", "click"
		*/	
		public void SendData(string data, string buttonType) {
			UXPlayerController player = UXPlayerController.Instance;
			string sendString = "{\"cmd\":\"pad_button_touch\",\"u_code\":\"" + player.GetCode () + "\",\"l_code\":\""  + UXConnectController.GetRoomNumber() + "\",\"data\":\"" + data + "\",\"button_type\":\"" + buttonType + "\"}" + DATA_DELIMITER;
			
			Send (sendString);    
			
		}
		
		/** Send join message 
			@param roomNumber room number
		*/
		public override void Join(string data) {
			Debug.Log ("Join");
			UXPlayerController player = UXPlayerController.Instance;
			
			if (player.IsUserLogin() == false) {
				OnJoinFailed(JOIN_ERROR_NOT_LOGIN);
				return;
			}

			string sendString = "{\"cmd\":\"join_user\",\"u_code\":\"" + player.GetCode () + "\",\"u_name\":\"" + player.GetName () + "\",\"l_code\":\"" + launcherCode + "\"}" + DATA_DELIMITER;
			Debug.Log ("sen " + sendString);
			Send (sendString);
		}
		
		/** Send leave message */
		public void Leave() {
			UXPlayerController player = UXPlayerController.Instance;
			
			string sendString = "{\"cmd\":\"leave_user\",\"u_code\":\"" + player.GetCode () + "\",\"l_code\":\"" + UXConnectController.GetRoomNumber() + "\"}"+ DATA_DELIMITER;
			Send (sendString);
		}

		public void GetFirstUser() {

			string sendString = "{\"cmd\":\"first_user\",\"l_code\":\"" + UXConnectController.GetRoomNumber() + "\"}"+ DATA_DELIMITER;
			Send (sendString);
		}

		public string GetBigscreenState(){
			return bigscreenState;
		}


		void ProcessReceivedMessage(string data) {

			if (string.IsNullOrEmpty(data) == true || data.Length <= 0) {
				Debug.Log ("Empty" );
				return;
			}

			Debug.Log (" ProcessReceivedMessage Launchhhhhhhhhhhhh   " + data);
			var N = JSON.Parse(data);
			string command = N["cmd"];
			if (command == "join_user_result") {

				int rec = N["gp_ack"].AsInt;
				bool isFirst = N["is_first"].AsBool;
				bigscreenState = N["package_name"];
				Debug.Log("bigscreenState" + bigscreenState);
				bool result	= (rec == UXRoomConnect.ACK_RESULT_OK);
				
				if (result == true) {
					isJoin = true;
					isHostJoined = true;
					if (OnJoinSucceeded != null) {
						OnJoinSucceeded(isHostJoined);
					}
					
					if (isFirst == true) {
						if (OnFirstUser != null) {
							OnFirstUser();
						}
					}
				} else {
					isJoin = false;
					Debug.Log(" join fail " );

					if (OnJoinFailed != null) {
						OnJoinFailed(rec);
					}
				}
			} else if (command == "launcher_power_result") {
				int rec = N["gp_ack"].AsInt;
				bool result	= (rec == UXRoomConnect.ACK_RESULT_OK);
				
				if (result == true) {
					isActive = true;
				} else {
					isActive = false;
				}
			}else if (command == "pad_button_touch_result") {
				int rec = N["gp_ack"].AsInt;
				bool result	= (rec == UXRoomConnect.ACK_RESULT_OK);

				string buttonType = N["button_type"];

				if(buttonType == "click"){
					isPlay = true;
					string gameInfo = N["game_appinfo"];
					OnBigscreenGameInfo(gameInfo);
				}else if(buttonType == "back"){
					isPlay = false;
					string storePage = N["store_page"];
					if(OnStorePageState != null){
						OnStorePageState(storePage);
					}
				}else if (buttonType == "ok") {
					if (OnStartGamePad != null) {
						padPackage = N["pad_package"];

						Debug.Log("Lib   button type   ok   ");
						OnStartGamePad(padPackage);
					}
				}

				
			} else if (command == "leave_user_result") {
				UXLog.SetLogMessage("leave user result");
				Clear ();
				
				if (OnUserLeft != null) {
					UXLog.SetLogMessage(" OnUserLeft");
					OnUserLeft();
				}
			} else if (command == "first_user") {
				Debug.Log("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&  command == first_user !!!!!!!!!!!");
				if (OnChangeFirstUser != null) {
					OnChangeFirstUser();
				}
			} else {
				base.ProcessReceivedMessage(data);
			} 
		}	

	}
}