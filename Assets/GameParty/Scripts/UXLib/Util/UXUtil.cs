using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UXLib.Base;

namespace UXLib.Util {
	public class UXUtil : UXObject {
	
		public UXUtil() : base("Util") {
		}
		
		public static int GetUnixtimestamp() {
			return (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
		}
	}
}
