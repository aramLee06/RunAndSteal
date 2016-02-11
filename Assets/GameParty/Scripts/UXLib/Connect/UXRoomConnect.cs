using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UXLib.Base;
using UXLib.Connect;
using SimpleJSON;
using UXLib.Util;

namespace UXLib.Connect {
	public class UXRoomConnect : UXObject {
		public static int ACK_RESULT_OK = 0; 
	
		public enum DebugMode {
			Not,
			UxOnly,
			All
		};

		private static UXRoomConnect instance = null;
		public static UXRoomConnect Instance{
			get {
				if (instance == null)
					instance = new UXRoomConnect ();
				return instance;
			}
		}

		public int clientIndex;
//		UXClientSocket roomSocket;
		UXClientSocketNew roomSocket;
		public static DebugMode debugMode;
		
		int connectCount;

		public static string CSP_TAG = "CSPLib";
		public static string debugString;
		
		public delegate void OnClientAddedHandler(string type, string host, string user, string cmd);
		public delegate void OnClientRemovedHandler(string type, string host, string user, string cmd);
		public delegate void OnServerConnectedHandler();
		public delegate void OnServerConnectFailedHandler();
		public delegate void OnServerDisconnectedHandler();
		public delegate void OnReceivedHandler(byte[] msg);
		public delegate void OnServerErrorHandler(int err, string msg);

		public event OnReceivedHandler OnReceived; 
		public event OnServerConnectedHandler OnServerConnected;
		public event OnServerConnectFailedHandler OnServerConnectFailed;
		public event OnServerDisconnectedHandler OnServerDisconnected;  
		public event OnServerErrorHandler OnServerError;
		
		public UXRoomConnect() : base("UXRoomConnect") {
			roomSocket = new UXClientSocketNew();
			roomSocket.OnDataReceived += OnMessageReceived;
			roomSocket.OnConnect += OnConnect; 
			roomSocket.OnConnectFailed += OnConnectFailed; 
			roomSocket.OnDisconnect += OnDisconnect;
			roomSocket.OnError += OnError;
		}
		
		public void Disconnect() {
			roomSocket.Disconnect ();
		}

		public void OnConnect() {
			if (OnServerConnected != null) {
				OnServerConnected();
			}
		}
		
		public void OnConnectFailed() {
			if (OnServerConnectFailed != null) {
				OnServerConnectFailed();
			}
		}
		
		void OnDisconnect () {
			if (OnServerDisconnected != null) {
				OnServerDisconnected();
			}
		}
		
		void OnError (int err, string msg) {
			if (OnServerError != null) {
				OnServerError(err, msg);
			}
		}
		
		public bool IsConnected() {
			if (roomSocket == null) {
				return false;
			}
			
			return roomSocket.IsConnected();
		}

		public bool SocketOpen(string ip, int port){
			Debug.Log ("SocketOpen");
			if(!IsConnected())
				return roomSocket.Open (ip, port);
			return true;
		}
		
		public void OnMessageReceived(byte[] msg) {
			if (OnReceived != null) {
				OnReceived(msg);
			}
		}

		public void Send (string msg, bool isAsync=false) {
			roomSocket.Write (msg, isAsync);
		}	
	}
}
