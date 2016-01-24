using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using UXLib;
using UXLib.Util;

public class QRCodeScanner : MonoBehaviour {

	public Renderer PlaneRender;


	void Start () {

		// Initialize EasyCodeScanner
		EasyCodeScanner.Initialize();

		//Register on Actions
		EasyCodeScanner.OnScannerMessage += onScannerMessage;
		EasyCodeScanner.OnScannerEvent += onScannerEvent;
		EasyCodeScanner.OnDecoderMessage += onDecoderMessage;



		EasyCodeScanner.launchScanner( true, "", -1, true);
		//Screen.orientation = ScreenOrientation.LandscapeLeft;
	}
	
	void OnDestroy() {
		
		//Unregister
		EasyCodeScanner.OnScannerMessage -= onScannerMessage;
		EasyCodeScanner.OnScannerEvent -= onScannerEvent;
		EasyCodeScanner.OnDecoderMessage -= onDecoderMessage;

	}
	
	public void Update() {
	}

	//Callback when returns from the scanner
	void onScannerMessage(string data){
		Debug.Log("EasyCodeScannerExample - onScannerMessage data=:"+data);
		


		Debug.Log("dataStr.Length    " + data.Length);
		if(string.IsNullOrEmpty(data) == true || data == ""){

			RoomNumberWindow.qrCodeIsNull = false;
			Application.LoadLevel("2_RoomNumber");
			return;
		}
		if(data.Length > 5) {

			RoomNumberWindow.qrCodeIsNull = false;
			Application.LoadLevel("2_RoomNumber");
			return;
		}
		RoomNumberWindow.qrString = data;
		Application.LoadLevel("2_RoomNumber");

	}
	
	//Callback which notifies an event
	//param : "EVENT_OPENED", "EVENT_CLOSED"
	void onScannerEvent(string eventStr){
		Debug.Log("EasyCodeScannerExample - onScannerEvent:"+eventStr);
		if(eventStr == "EVENT_CLOSED"){
			RoomNumberWindow.qrString = null;
			Application.LoadLevel("2_RoomNumber");
			return;
		}
	}
	
	//Callback when decodeImage has decoded the image/texture 
	void onDecoderMessage(string data){

		Debug.Log("EasyCodeScannerExample - onDecoderMessage data:"+data);
		UXLog.SetLogMessage("EasyCodeScannerExample - onDecoderMessage data:"+data);
		if(string.IsNullOrEmpty(data) == true || data == ""){
			
			RoomNumberWindow.qrCodeIsNull = false;
			Application.LoadLevel("2_RoomNumber");
			return;
		}
		if(data.Length > 5) {
			
			RoomNumberWindow.qrCodeIsNull = false;
			Application.LoadLevel("2_RoomNumber");
			return;
		}
		RoomNumberWindow.qrString = data;
		Application.LoadLevel("2_RoomNumber");

	}
	

}