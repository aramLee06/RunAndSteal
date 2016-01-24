/**
	@class UXLib.UXAndroidManager
	@brief Class for using android native function 
	@section intro Usage
 	  - Add GameObject
 	  - Renamee it 'AndroidManager'
      - Add UXAndroidManager script to AndroidManager object
      - Call InitAndroid method before use
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UXLib.Base;
using System.Threading;

namespace UXLib {
	public class UXAndroidManager : MonoBehaviour {
		
		#if UNITY_ANDROID
		public const int MTYPE_GYRO = 1;		/**< 자이로 센서 */
		public const int MTYPE_GRAVITY = 2;		/**< 중력 센서 */
		
		int mouseMode;
		AndroidJavaClass javaClass;
		AndroidJavaObject activity;
		
		bool isMouseOn, isWifiOn, isP2pOn, isNfcOn;
		
		public delegate void OnAnd_MouseChangedHandler(int x, int y);
		public delegate void OnAnd_WifiStateChangedHandler(bool connected, string ssid);
		public delegate void OnAnd_NfcDataReceivedHandler(string data);
		
		public delegate void OnAnd_RESTSucceededHandler(string data);
		public delegate void OnAnd_RESTFailedHandler();
		
		public delegate void OnAnd_P2PConnectedHandler(int idx);
		public delegate void OnAnd_P2PPeersChangedHandler(List<string>p2pList);
		public delegate void OnAnd_P2PMessageReceivedHandler(int idx, string message);
		public delegate void OnAnd_ProfileImageChangedHandler(string filePath);
		
		/** P2P 호스트 연결시 호출 된다.
			@param index 인덱스
		*/ 
		public event OnAnd_P2PConnectedHandler OnAnd_P2PConnected;
		
		/** P2P 클라이언트 연결 변동시 호출 된다. 
			@param p2pList 연결 목록
		*/
		public event OnAnd_P2PPeersChangedHandler OnAnd_P2PPeersChanged;

		/** P2P 데이터 수신시 호출 된다. 
			@param idx 보낸 사용자 인덱스
			@param message 수신된 데이터
		*/	
		public event OnAnd_P2PMessageReceivedHandler OnAnd_P2PMessageReceived;
		
		/** 마우스 좌표 이동시 호출 된다.
			@param x x좌표
			@param y y좌표
		*/	 
		public event OnAnd_MouseChangedHandler OnAnd_MouseChanged;
		
		/** 와이파이 연결 상태 변동시 호출 된다.
			@param connected wifi가 연결 되어 있으면 true 아니면 false
			@param ssid 공유기 SSID
		*/
		public event OnAnd_WifiStateChangedHandler OnAnd_WifiStateChanged;

		/** NFC 데이터 수신시 호출 된다.
			@param data 수신된 데이터 
		*/
		public event OnAnd_NfcDataReceivedHandler OnAnd_NfcDataReceived;

		/** REST 결과가 성공 했을 경우에 호출 된다.
			@param data 수신된 데이터
		*/
		public event OnAnd_RESTSucceededHandler OnAnd_RESTSucceeded;
		
		/** REST 결과가 실패 했을 경우에 호출 된다.
		*/
		public event OnAnd_RESTFailedHandler OnAnd_RESTFailed;

		/** 갤러리나 사진에서 이미지가 선택되었을 경우에 호출된다.
		*/
		public event OnAnd_ProfileImageChangedHandler OnAnd_ProfileImageChanged;
		
		/** 초기화 - 사용 전에 호출되어야 한다.
		*/
		public void InitAndroid() {
			javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			activity = javaClass.GetStatic<AndroidJavaObject>("currentActivity");
			
			isMouseOn = false;
			isWifiOn = false;
			isP2pOn = false;
			isNfcOn = false;
		}
		
		public void TTT(string msg) {
		
		}
		
		
		/** 에어마우스 입력시 사용한다.
			@param width 스크린 너비
			@param height 스크린 높이
			@param mode 사용 센서 (MTYPE_GYRO, MTYPE_GRAVITY);	
		*/	
		public void StartMouseService(int width, int height, int mode) {
			activity.Call("StartMouseService", width, height, mode);
			mouseMode = mode;
			isMouseOn = true;
		}
		public void KillProcess(){
			activity.Call("killProcess");
		}
		public void StartRESTService(string url) {
			activity.Call ("StartHttpConnect", url);
		}
		
		public void SyncREST(string endPoint, string method, string json) {
			activity.Call ("SyncRequest", endPoint, method, json);
		}
		
		public void StartWifiService() {
			activity.Call ("StartWifiService");
			isWifiOn = true;
		}
		
		public void StartP2PServer() {
			activity.Call ("StartP2PServer");
		}
		
		public void StartP2PClient() {
			activity.Call ("StartP2PClient");
		}
		
		public void StartNfcService() {
			activity.Call ("StartNfcService");
			isNfcOn = true;
		}
		
		/** 절전모드에서 화면이 꺼지지 않도록 한다. */
		public void SetScreenOn() {
			activity.Call ("SetScreenOn");
		}
		
		/** */	
		public void StartCamera(int userCode) {
			activity.Call ("StartCamera", userCode);
		}
		
		public void StartGallery(int userCode) {
			activity.Call ("StartGallery", userCode);
		}

		public void ExecPackage(string packageID, int userCode, int launcherCode,int countryCode) {
			activity.Call ("ExecPackage", packageID, userCode, launcherCode, countryCode);
		}
		
		public void Alert(string title, string message, string buttonName) {
			activity.Call ("Alert", title, message, buttonName);
		}
			
		/** */	
		public void Vibrate(long msec) {
			activity.Call ("Vibrate", msec);
		}
		
		public void PatternVibrate(long[] pattern, int rep) {
			activity.Call ("PatternVibrate", pattern, rep);
		}
		
		public void SetMouseRevision(float val) {
			activity.Call ("SetMouseRevision", val);
		}
		
		public void SetMouseSensitivity(float val) {
			activity.Call ("SetMouseSensitivity", val);
		}
		
		public void SetMouseMode(int m) {
			mouseMode = m;
			activity.Call ("SetMouseMode", m);
		}
		
		/** 마우스 센스 모드를 확인한다.
		    @return true/false
		*/
		public int GetMouseMode() {
			return mouseMode;
		}
		
		/** NFC 메시지를 설정한다.
		    @rparam msg 보낼 메시지
		*/
		public void SetNfcMessage(string msg) {
			if (isNfcOn == false) {
				return;
			}
			activity.Call ("SetNfcMessage");
		}
		
		/** 와이파이를 락한다.
		*/
		public void LockWifi() {
			if (isWifiOn == false) {
				return;
			}
			activity.Call ("LockWifi");
		}
		
		/** 와이파이를 언락한다.
		*/
		public void UnlockWifi() {
			if (isWifiOn == false) {
				return;
			}
			activity.Call ("UnlockWifi");
		}
		
		/** 와이파이가 락되어 있는지 확인한다.
		    @return true/false
		*/
		public bool IsWifiLocked() {
			int rst = activity.Call<int>("IsWifiLocked");
			if (rst == 0) {
				return false;
			}
			
			return true;
		}
		
		/** 게임 실행시 넘겨 받은 사용자 코드를 얻어 온다.
		    @return 사용자 코드
		*/
		public int GetUserCode() {
			return activity.Call<int>("GetUserCode");
		}

		public int GetCountryCode() {
			return activity.Call<int>("GetCountryCode");
		}

		/** 게임 실행시 넘겨 받은 런처 코드를 얻어 온다.
		    @return 런처 코드
		*/
		public int GetLauncherCode() {
			return activity.Call<int>("GetLauncherCode");
		}

		public string GetVersionName(string packName){
			return activity.Call<string>("getVersionName", packName);
		}

		/*  
			디바이스에 해당게임이 인스톨되어있는지 체크
			게임실행가능 상태이면 play 반환
			게임이 인스톨이 필요하면 install 반환
			게임이 업데이트가 필요하면 update 반환
		*/
		public string GetPackageInfo(string packName,string versionName){
			return activity.Call<string>("getPackageInfo", packName, versionName);
		}

		/** 사용자 코드를 설정한다.
		    @param filePath 이미지 파일 패스
		*/		
		public void SetCode(int code) {
			activity.Call("SetUserCode", code);
		}
		
		/** 에어마우스 영점을 조절한다.
		*/
		public void CalibrateMouse() {
			if (isMouseOn == false) {
				return;
			}
			activity.Call ("CalibrateMouse");
		}

		/** 카메라나 갤러리로 부터 이미지 파일 선택시 호출된다.
		    @param filePath 이미지 파일 패스
		*/
		public void And_ProfileImageChanged(string filePath) {
			if (OnAnd_ProfileImageChanged != null) {
				OnAnd_ProfileImageChanged(filePath);
			}
		}
		
		/** REST 쿼리 후 성공시 호출된다. 
		    @param data 수신된 데이터
		*/	
		public void And_RESTSucceeded(string data) {
			OnAnd_RESTSucceeded(data);
		}
		
		/** REST 쿼리 후 실패시 호출된다. 
		*/	
		public void And_RESTFailed() {
			OnAnd_RESTFailed();
		}
		
		/** 마우스 커서 변경시 호출된다.
		    @param data 좌표 데이터
		*/    
		public void And_MouseChanged(string data) {
			//Lobby.logString = "R:" + data;
			string[] pos = data.Split (',');
			int x = int.Parse (pos[0]);
			int y = int.Parse (pos[1]);
			
			OnAnd_MouseChanged(x, y);
		}
		
		/** 와이파이 상태 변경시 호출된다.
		    @param data 결과값 (1이면 연결)
		*/    
		public void And_WifiStateChanged(string data) {
			string ssid = "";
			
			bool connected = false;
			if (data.Substring(0, 1) == "1") {
				ssid = data.Substring(1);
				connected = true;
			} 
			
			OnAnd_WifiStateChanged(connected, ssid);
		}

		/** NFC 데이터 수신시 호출된다.
		    @param data 수신값
		*/ 
		public void And_NfcDataReceived(string data) {
			OnAnd_NfcDataReceived(data);
		}
		
		/** P2P 연결시 호출된다.
		    @param data 클라이언트 인덱스
		*/
		public void And_P2PConnected(string data) {
			int idx = int.Parse (data);
		}
		
		/** P2P 연결 목록에 변경이 있을 경우 호출된다.
		    @param data 연결 목록
		*/
		public void And_P2PPeersChanged(string data) {
			string[] items = data.Split ('*');
			List<string> list = new List<string>();
			
			for (int i = 0; i < items.Length; i++) {
				list.Add(items[i]);	
			} 
			
			OnAnd_P2PPeersChanged(list);
		}
		
		/** P2P 메시지 수신시 호출된다.
		    @param data 수신된 값
		*/
		public void And_P2PMessageReceived(string data) {
			int idx = int.Parse (data.Substring (0, 1));
			string msg = data.Substring(1);
			
			OnAnd_P2PMessageReceived(idx, msg);
		}
		
		public void And_SetLog(string data) {
			//Lobby.logString = data;
		}
		
		#endif
	}

}

