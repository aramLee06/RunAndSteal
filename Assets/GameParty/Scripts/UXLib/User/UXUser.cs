using UnityEngine;
using System.Collections;
using UXLib.Base;
using UXLib.Connect;
using SimpleJSON;

namespace UXLib.User {
	public class UXUser : UXObject {
		public enum LobbyState {
			Invalid = -1,
			Wait = 0,
			Ready = 1,
		};
		
		string name;
		int code;
		
		string imageURL;
		float networkSpeed;
		bool isConnected;
		UXUserController userList;
		LobbyState lobbyState;
		
		public UXUser(string name, int ucode) : base(name) {
			this.name = name;
			this.code = ucode;
			this.lobbyState = LobbyState.Wait;
		}
		
		public void SetConnected(bool connect) {
			isConnected = connect;
		}
		
		public bool IsConnected() {
			return isConnected;
		}

		private bool isPremium = false;
		public bool IsPremium 
		{
			get{
				return isPremium;
			}
			set{
				this.isPremium = value;
			}
		} 

		public string GetImageURL() { return imageURL; }
		public LobbyState GetLobbyState() { return lobbyState; }
		public void SetLobbyState(LobbyState state) {
			lobbyState = state;
		}
		
		public void SetNetworkSpeed(float value) {
			networkSpeed = value;
		}
		
		public float GetNetworkSpeed() {
			return networkSpeed;
		}
		
		public bool GetProfileFromServer() {
			/**
			if (code == -1) { //host면
				return false;
			}
			//pad면
			string recData = UXRestConnect.Request("users/?ucode=" + code, UXRestConnect.REST_METHOD_GET, null);
			
			var N = JSON.Parse(recData);
			int rec = N["gp_ack"].AsInt;
			bool result	= (rec == UXRoomConnect.ACK_RESULT_OK);
			
			if (result == true) {
				name = N["name"];
				imageURL = N["profile_pic_name"];
			} else {
				return false;
			}
			**/
			
			return true;
		}
		
		public int GetCode() { return code; }
	}
}
