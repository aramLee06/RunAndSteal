using UnityEngine;
using System.Collections;
using System.Timers;

using UXLib;
using UXLib.Base;
using UXLib.Util;
using UXLib.Connect;

namespace UXLib.Connect {
	public class UXAckSender : UXObject {
	
		int sendTime;
		int sendCount;
		
		string commandTitle;
		
		UXRoomConnect roomConnect;
		
		System.Timers.Timer ackTimer;
	
		public UXAckSender(int interval = 3000) : base("AckSender") {
			ackTimer = new System.Timers.Timer();
			ackTimer.Interval = interval;
			ackTimer.Elapsed += new ElapsedEventHandler(SendAckTimer);
			
			commandTitle = "cmd";
		}	
		
		public void SetConnect(UXRoomConnect connect) {
			roomConnect = connect;
			
		}
		
		public void ChangeCommandTitle(string title) {
			commandTitle = title;
		}
		
		public void Start() {
			sendCount = 0;
			ackTimer.Start();
		}
		
		public void Stop() {
			sendCount = 0;
			ackTimer.Stop ();
		}
		
		public void ReceiveResult() {
			if (sendCount > 0) {
				sendCount--;
			}
		}
		
		public bool CheckAckCount() {
			if (sendCount == 0) {
				return true;
			}

			int currentTime = UXUtil.GetUnixtimestamp();			
			int gab = currentTime - sendTime;
				
			if (gab < ackTimer.Interval/1000) {
				return true;
			}
				
			sendCount = 0;
				
			return false;
		}
			
		void SendAckTimer(object sender, ElapsedEventArgs e) {
			
			if (sendCount != 0) {
				return;
			}
	
			if (roomConnect == null ) {
				return;
			}
				
			sendTime = UXUtil.GetUnixtimestamp();
			sendCount++;
				
			string sendString = "{\""+ commandTitle + "\":\"ack\",\"time\":" + sendTime.ToString ();
			sendString += "}" + UXConnectController.DATA_DELIMITER; 
			roomConnect.Send (sendString);    
		}
	}
}
