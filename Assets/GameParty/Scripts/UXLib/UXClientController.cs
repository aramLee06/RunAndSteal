using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;

using UXLib.Util;
using UXLib;
using UXLib.Base;
using UXLib.User;
using UXLib.Connect;
using UXLib.User;
using SimpleJSON;

namespace UXLib {
	public class UXClientController : UXConnectController {

		/** Called when join was successful */
		public event OnJoinSucceededHandler OnJoinSucceeded;

		/** Called when join was failed */		
		public event OnJoinFailedHandler OnJoinFailed;
		
		private static UXClientController instance = null;
		
		/** Singletone */
		public static UXClientController Instance {
			get {
				if (instance == null) {
					instance = new UXClientController();
				}
				return instance;
			}
		}
		 
		private UXClientController() {
			Init ();
			SetMode (UXConnectController.Mode.Client);
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
					ProcessReceivedMessage(receiveQueue[i]);
				}
				receiveQueue.Clear();
			}
		}
		
		/** Send join message */
		public override void Join(string packageName) { //data는쓰레기 호출할때도 "none"
			Debug.Log ("Join");
			UXPlayerController playerController = UXPlayerController.Instance;
			
			int userCode = playerController.GetCode();
			string name = playerController.GetName ();
			
			string msg = "{\"cmd\":\"join\",\"type\":\"user\",\"l_code\":\"" + room.RoomNumber + "\",\"u_code\":\"" + userCode + "\",\"name\":\""+ name + "\",\"max_user\":\"0\", \"package_name\" : \"" + packageName + "\"}" + DATA_DELIMITER;

			Debug.Log (msg);
			Send(msg);//{"cmd":"join","type":"user","l_code":"launcherCode","u_code":"userCode","name":"name","max_user":"0"}232
		}

		/** Set player's lobby state 
			@param state lobby state
			@see UXUser.LobbyState
		*/
		public void SetPlayerState(UXUser.LobbyState state) {
			UXPlayerController player = UXPlayerController.Instance;
			player.SetLobbyState(state);

			string stateString = "";
			if (state == UXUser.LobbyState.Ready) {
				stateString = "ready";
			} else {
				stateString = "wait";	
			}
			
			string sendString = "{\"cmd\":\"change_lobby_state\",\"u_code\":\""+ player.GetCode () + "\",\"l_code\":\""+ room.RoomNumber + "\",\"state\":\""+ stateString + "\"}" + UXConnectController.DATA_DELIMITER; //<-?
			Send (sendString);//{"cmd":change_lobby_state","u_code":player.GetCode()","l_code":"GetRoomNumber()","state":stateString"}232
		}
		
		/** Set user code and room number 
			@param aUcode user code
			@param roomNumber room number
			@return True if codes are valid, false otherwise 
		*/
		public bool SetCode(int aUcode, int roomNumber) {
			UXPlayerController player = UXPlayerController.Instance;
			
			int lcode = GetRoomNumber();
			int ucode = -1;
			
			if (lcode != -1) {
				ucode = player.GetCode();
			} else {
				lcode = roomNumber;
				ucode = aUcode;
			}
			
			if (lcode == -1 || ucode == -1) {
				return false;
			}	
			
			launcherCode = lcode;
			
			player.SetCode (ucode);
						
			return true;
		}

		void ProcessReceivedMessage(string data) {
			
			if (string.IsNullOrEmpty(data) == true || data.Length <= 0){//data가 비어있따면
				return;
			}
			
			var N = JSON.Parse(data);
			string command = N["cmd"];
			
			bool isRunBaseProcess = false;
			
			if (command == "join_result") {
				
				int result = N["ack"].AsInt;
				if (result != UXRoomConnect.ACK_RESULT_OK) {
					int errCode = ProcessConnectError(result);
					
					if (OnJoinFailed != null) {
						OnJoinFailed(result);
					}
					return;
				
				}
				
				isJoined = true;
				
				string temp = N["user"];
				isHostJoined = N["is_host"].AsBool;
					
				string[] infos = temp.Split('.');
					
				UXPlayerController player = UXPlayerController.Instance;
					
				player.SetCode (Int32.Parse(infos[0]));
				player.SetName (infos[1]);
				
				if (OnJoinSucceeded != null) {
					OnJoinSucceeded(isHostJoined);
				}
			}  else { //cmd != join_result
				base.ProcessReceivedMessage(data);
				Debug.Log ("ProcessReceivedMessage : " + data);
			}
		}
	}
}
