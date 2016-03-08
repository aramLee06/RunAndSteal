using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UXLib.Base;
using UXLib.User;

namespace UXLib.User {
	public class UXUserController : UXObjectList {
	
		private static UXUserController instance = null;
		public static UXUserController Instance {
			get {
				if (instance == null) {
					instance = new UXUserController();
				}
				return instance;
			}
		}
		
		public UXUser GetUserByCode(int code) {
			foreach (UXUser obj in objectList) {
				if (obj.GetCode () == code) {
					return obj;
				}
			}
			return null;
		} 
		
		public bool IsConnectedUser(int index) {
			if (index < 0 || index >= objectList.Count) {
				return false;
			}
			
			UXUser user = (UXUser)GetAt (index);
			return user.IsConnected();
		}
		
		public bool IsEqual(List<UXUser> list) {
			if (objectList.Count != list.Count) {
				return false;
			}
			
			for (int i = 0; i < objectList.Count; i++) {
				UXUser user1 = (UXUser)objectList[i];
				UXUser user2 = list[i];
				
				if (user1.GetCode () != user2.GetCode () || user1.GetName () != user2.GetName ()) {
					return false;
				}
			}
			
			return true;
		}
		
		public void CopyList(List<UXUser> list) {

			// isPremium 정보 일시 저장
			int preListCnt = objectList.Count;

			List<UXObject> preObjList = new List<UXObject> (objectList);
			objectList.Clear ();

			foreach (UXUser user in list) {
				user.IsPremium = false;
				foreach (UXObject preObj in preObjList){				
					UXUser preUser = (UXUser)preObj;
					if (user.GetCode () == preUser.GetCode ()) {
						user.IsPremium = preUser.IsPremium;
					}
				}

				objectList.Add (user);
			}
		}

		
	}
}
