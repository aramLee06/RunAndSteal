#define WRITE_LOG

using UnityEngine;
using System.Collections;
using System.IO;
using System;
using UXLib.Base;

namespace UXLib.Util {
	public class UXLog {
		
		static string logMessage;
		static string fileName;
		static FileStream file;
		static StreamWriter writeStream;
		
		public static void Init() {
			#if WRITE_LOG		
			int timeStamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			fileName = GetWritableFilePath("" + timeStamp + ".log");
			
			Debug.Log ("Log file path:" + fileName);
			
			if (file == null || file.CanWrite == false) {
				file = new FileStream (fileName, FileMode.Create, FileAccess.Write);	
				writeStream = new StreamWriter(file);
			}
			#endif
		}
		
		public static void Close() {
			#if WRITE_LOG
			if (writeStream != null) {
				writeStream.Close ();
			}
			
			if (file != null) {
				file.Close ();
				file = null;
			}
			#endif			
		}
		
		public static string GetLogMessage() {
			return logMessage;
		}
		
		public static void SetLogMessage(string msg) {
			logMessage = msg;
		}
		
		public static void Write(string log) {
			#if WRITE_LOG	
			DateTime time = DateTime.Now;
			
			string line = "[" + DateTime.Now.ToLongTimeString() + "]" + log;
			writeStream.WriteLine(line);
			writeStream.Flush();
			#endif			
		}
		
		public static string GetWritableFilePath(string filename) {
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				string path = Application.dataPath.Substring( 0, Application.dataPath.Length - 5);
				path = path.Substring(0, path.LastIndexOf( '/' ));
				return Path.Combine(Path.Combine( path, "Documents" ), filename);
			} else if(Application.platform == RuntimePlatform.Android) {
				string path = Application.persistentDataPath; 
				path = path.Substring(0, path.LastIndexOf( '/' )); 
				return Path.Combine(path, filename);
			} else {
				string path = Application.dataPath; 
				path = path.Substring(0, path.LastIndexOf( '/' ) );
				return Path.Combine(path, filename);
			}
		}
	}
}
