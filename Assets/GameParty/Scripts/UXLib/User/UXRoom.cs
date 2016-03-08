using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UXLib.Base;
using UXLib.Connect;
using SimpleJSON;

namespace UXLib.User {
	public class UXRoom
	{
		static string REQUEST_ROOM_NUMBER = "g r n";
		bool isGameStarted = false;
		public static char DATA_DELIMITER = (char)232;//여기서 자름
		private UXRoomConnect roomConnect;

		private static UXRoom instance = null;
		public static UXRoom Instance
		{
			get {
				if (instance == null) {
					instance = new UXRoom ();
				}
				return instance;
			}
		}


		private UXUserController userList; //현재 방에 접속한 유저 관리
		public UXUserController UserList 
		{
			get {
				return userList;
			}
		}


		private UXPlayerController player; //현재 접속한 유저에 대한(자기자신) 정보
		public UXPlayerController Player
		{
			get {
				return player;
			}	
		}
			
		public bool IsPremium 
		{
			get{
				return IsPremiumRoom();
			}
		} 


		private int maxUser = 2; //이 방에 접속할 수 있는 최대 인원 수 (초기값 2)
		public int MaxUser 
		{ 
			get{
				return maxUser;
			}
			set{
				this.maxUser = value;
				Debug.Log ("maxuser setting: "+this.maxUser);
				string sendString = "{\"cmd\":\"max_user_set\",\"max_client\":\"" + value + "\",\"l_code\":\"" + RoomNumber + "\",\"u_code\":\"" + player.GetCode() + "\"}" + DATA_DELIMITER;
				roomConnect.Send(sendString);   // {"cmd":"max_user_set","max_client":" maxUser","l_code":"launcherCode","u_code":"player.GetCode()"}232
			}
		} // 추후 get set 추가가능성 있음


		private string roomNumber; //이 방의 번호
		public string RoomNumber 
		{ 
			get{
				return roomNumber;
			}
			set{ 
				this.roomNumber = value;
			} 
		}

		private bool IsPremiumRoom(){
			UXUserController userController = UXUserController.Instance;
			List<UXObject> userList = userController.GetList();
			for (int i = 0; i < userList.Count; i++)
			{
				UXUser user = (UXUser)userList[i];
				if (user.IsPremium) {
					return true;
				}
			}
			return false;
		}

		public UXRoom ()
		{
			userList = UXUserController.Instance; 
			player = UXPlayerController.Instance; 
			roomConnect = UXRoomConnect.Instance;
		}


		/*** REST를 통해 방을 생성함. ***/ 
		public bool CreateRoom(string package, int maxClient){ 
			UXConnectController.SetMode (UXConnectController.Mode.Host);

			string recData = UXRestConnect.Request("room/create?package=" + package + "&max_client=" + maxClient, UXRestConnect.REST_METHOD_GET, ""); //room/create:방만드는애

			if (recData == null) {
				return false;
			}

			var N = JSON.Parse(recData);

			RoomNumber = N["roomNumber"];
			player.SetCode (N ["uuid"].AsInt);

			return true;
		}


		/*** UXUser 객체를 넘겨받아 유저를 추가함 ***/
		public void AddUser(UXUser user){
			// TODO : ConnectController 의 ProcessReceivedMessage -> cmd == 'user_add' 참조
			user.SetConnected(true);
			user.GetProfileFromServer(); //user name, image url저장

			UXUserController userController = UXUserController.Instance;
			List<UXObject> userList = userController.GetList();
			userController.Add((UXObject)user); //사람 넣기	
		}


		/*** 유저리스트 갱신 ***/
		public void UpdateUserList(List<UXUser> list){
			if (!userList.IsEqual(list))
			{
				userList.CopyList(list);
			}
		}


		/*** 이름을 넘겨받아 유저를 삭제 ***/
		public void RemoveUser(string name){ 
			UXUserController userController = UXUserController.Instance;
			List<UXObject> userList = userController.GetList();

			for (int i = 0; i < userList.Count; i++)
			{
				UXUser user = (UXUser)userList[i]; //누군지 찾는거
				if (user.GetName().Equals(name)) {
					if (!isGameStarted) { // In lobby
						userController.RemoveByName (name);
					}
					else
						user.SetConnected (false);
					break;
				}
			}
		}


		/*** UXUser 객체를 넘겨받아 유저를 삭제 ***/
		public void RemoveUser(UXUser user){ 
			UXUserController userController = UXUserController.Instance;
			List<UXObject> userList = userController.GetList();

			for (int i = 0; i < userList.Count; i++)
			{
				if (user.GetCode ().Equals(userList[i])) { ///
					if (!isGameStarted) { // In lobby
						userController.RemoveByName (user.GetName());
					}
					else
						user.SetConnected (false);
					break;
				}
			}
		}


		/*** index을 넘겨받아 유저를 삭제 ***/
		public void RemoveUser(int index){
			UXUserController userController = UXUserController.Instance;
			List<UXObject> userList = userController.GetList();

			for (int i = 0; i < userList.Count; i++)
			{
				UXUser user = (UXUser)userList[i]; //누군지 찾는거
				if (i==index) {
					if (!isGameStarted) { // In lobby
						userController.RemoveAt (index);
					}
					else
						user.SetConnected (false);
					break;	
				}
			}
		}

			
		/*** 현재 index를 최신상태로 업데이트 ***/
		public void RefreshIndex(){

		}



	}
}

