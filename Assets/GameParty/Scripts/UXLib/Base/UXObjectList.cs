using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UXLib.Base {
	public class UXObjectList : UXObject {
	
		protected List<UXObject> objectList;
		
		public UXObjectList() : base("UXObjectList") {
			objectList = new List<UXObject>();
		}
		
		public List<UXObject> GetList() {
			return objectList;
		}

		public void Add(UXObject obj) {
			objectList.Add (obj);
		}
		
		public void RemoveByName(string name) {
			UXObject obj = GetObjectByName(name);
			if (obj == null) {
				return;
			}
			
			objectList.Remove (obj);
		}
		
		public void RemoveAt(int pos) {
			objectList.RemoveAt(pos);
		}
		
		public void Clear() {
			objectList.Clear();
		}
		
		public UXObject GetAt(int index) {
			return objectList[index];
		}
		
		public UXObject GetObjectByName(string name) {
			foreach (UXObject obj in objectList) {
				if (obj.CompareName(name)) {
					return obj;
				}
			}
			
			return null;
		} 
		
		public UXObject GetObjectByIndex(int idx) {
			return objectList[idx];
		} 
		
		public int GetCount() {
			return objectList.Count;
		}
	}
}
