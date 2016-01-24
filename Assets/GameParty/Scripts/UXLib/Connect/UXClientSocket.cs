using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Net.Sockets;
using System.Net;
using UXLib.Base;
using UXLib.Util;
using System.Text;
using System.Threading;



namespace UXLib.Connect {

	public class StateObject {
		// Client  socket.
		public Socket workSocket = null;
		// Size of receive buffer.
		public const int BufferSize = 1024;
		// Receive buffer.
		public byte[] buffer = new byte[BufferSize];
		// Received data string.
		public StringBuilder sb = new StringBuilder();  
	}




	public class UXClientSocket : UXObject {
		const int RBUFFER_SIZE = (1024 * 100);
		
		public static int SOCK_ERROR_SEND = 1;
		public static int SOCK_ERROR_SEND_FILE = 2;
		public static int SOCK_ERROR_READ = 3;
	
		string hostIP;
		int hostPort;
		Socket socket;
		
		FileStream fileStream = null;
		BinaryWriter binaryWriter = null;
		bool isFileRecieved;
		
		private static byte[] recieveBuffer = new byte[RBUFFER_SIZE];
		
		public delegate void OnConnectHandler(); 
		public delegate void OnConnectFailedHandler(); 
		public delegate void OnDisConnectHandler();
		public delegate void OnDataReceivedHandler(string msg);
		public delegate void OnErrorHandler(int err, string msg);
		public delegate void OnSendEndedHandler();
		public delegate void OnFileSendEndedHandler();
		
		public event OnConnectHandler OnConnect; 
		public event OnConnectFailedHandler OnConnectFailed;
		public event OnDisConnectHandler OnDisconnect;
		public event OnDataReceivedHandler OnDataReceived;
		public event OnErrorHandler OnError;
		public event OnSendEndedHandler OnSendEnded;
		public event OnFileSendEndedHandler OnFileSendEnded;
		
		public UXClientSocket(string name="ClientSocket") : base(name) {
			isFileRecieved = false;
		}
		
		public bool Open(string host, int port) { 
			hostIP = host;
			hostPort = port;

			try {
				IPAddress serverIp = IPAddress.Parse(hostIP);
				IPEndPoint ipep = new IPEndPoint(serverIp, port);
				
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				socket.SendTimeout = 3000;
				socket.NoDelay = true;		
						                                 
				socket.BeginConnect(ipep, new AsyncCallback(OnConnected), socket);	
				
			} catch (Exception e) {
				if (OnConnectFailed != null) {
					OnConnectFailed();
				}
				
				return false;
			}
			
			return true;
		}
		
		public void Disconnect() {
			if (socket != null) {
	//			if (socket.Connected == true) {
				socket.Disconnect(false);
				socket.Close ();
				socket = null;
	//			}
			}
			
			socket = null;
			
			if (OnDisconnect != null) {
				OnDisconnect();
			}
		}
		
		public void RecieveFile(string fileName) {
			fileStream = File.Create (fileName);
			binaryWriter = null;
		}
		
		public bool IsConnected() {
			if (socket == null) {
				return false;
			}
			
			return socket.Connected;
		}
		
		private void OnConnected (IAsyncResult ar) {
		
			if (socket.Connected == false) {
				if (OnConnectFailed != null) {
					OnConnectFailed();
				}
				
				return;
			}
			
			socket.SendTimeout = 3000;
			socket.NoDelay = true;		
			
			socket.BeginReceive(recieveBuffer, 0, recieveBuffer.Length, SocketFlags.None, new AsyncCallback(OnMessaged), socket);
			
			if (OnConnect != null) {
				OnConnect();
			}
		}
		
		public void SendFile(string filePath) {
			if (IsConnected() == false) {
				return;
			}
			
			socket.BeginSendFile (filePath, new AsyncCallback(OnFileSended), socket);
		}
		
		public void OnFileSended(IAsyncResult ar) {
		
			Thread.Sleep(1000); 

			if (OnFileSendEnded != null) {
				OnFileSendEnded();
			}
		}
		
		public void Write(string msg, bool isAsync=false) {
			
			if (IsConnected() == false) {
				if (OnError != null)
					OnError(SOCK_ERROR_SEND, "Socket was closed");
					
				return;
			}
			byte[] data = Encoding.UTF8.GetBytes(msg);
			try {
				if (isAsync == true) {

					socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(OnSened), socket);
				} else {
				//	socket.Blocking = false;
					int len = socket.Send (data);
					Debug.Log ("socket : " + msg);
					
					if (OnSendEnded != null)
						OnSendEnded();
				}
				
			} catch (Exception e) {
				if (OnError != null)
					OnError(SOCK_ERROR_SEND, "Send error: " + e.Message);
					
				return;
			}
		}

		private void OnMessaged (IAsyncResult ar) { 
			try {
				Socket aSocket = (Socket)ar.AsyncState;
				
				int recieved = aSocket.EndReceive(ar);
				byte[] dataBuf = new byte[recieved];
				Array.Copy(recieveBuffer, dataBuf, recieved);
				string text = UTF8Encoding.UTF8.GetString(dataBuf);

				Debug.Log ("text : " + text);
				if (OnDataReceived != null) {
					OnDataReceived(text);
				}
				
				try {
					socket.BeginReceive(recieveBuffer, 0, recieveBuffer.Length, SocketFlags.None, new AsyncCallback(OnMessaged), aSocket);
				} catch (Exception e) {
					if (OnError != null) {
						OnError(SOCK_ERROR_READ, "Received error: " + e.Message);
					}	
				}
			} catch (Exception e) {
				if (OnError != null) {
					OnError(SOCK_ERROR_READ, "Received error: " + e.Message);
				}	
			}
		}
		
		private void OnSened (IAsyncResult ar) {
			if (OnSendEnded != null)
				OnSendEnded();
		}
	}
}
