using UnityEngine;
using System.Collections;

namespace UXLib.Base {
	public class UXObject {
	
		protected string name;
		
		public UXObject(string aName) {
			name = aName;
		}
		
		public void SetName(string aName) {
			name = aName;
		}
		
		public string GetName() {
			return name;
		}
		
		public bool CompareName(string aName) {
			return (name == aName);
		}
	}
}
