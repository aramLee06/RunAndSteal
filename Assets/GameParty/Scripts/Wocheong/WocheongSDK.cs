using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WocheongSDK : MonoBehaviour {
	private static WocheongSDK mInput = null;
	private AndroidJavaObject mJavaObject = null;

	public static WocheongSDK getInstance() {
		return mInput;
	}

	void Awake(){
		if (mInput == null) {
			mInput = this;
			mInput.mJavaObject = Current ();
			DontDestroyOnLoad (gameObject);
		} else {
			Destroy (gameObject);
		}
	}
		

	public static AndroidJavaObject Current() {
		if (Application.platform == RuntimePlatform.Android)  
			return new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"); 
		else  
			return null;
	}

	public bool buyItem() {
		if (PlayerPrefs.GetInt ("TimeLimit") == 0) {
			if (mJavaObject != null) {
				mJavaObject.Call("buyItem");
				return true;
			}	
			return false;
		} else {
			showToast ("Already purchase time limit");
			return false;	
		}

	}

	public bool showToast(string txt) {
		if (mJavaObject != null) {
			mJavaObject.Call ("showToast", txt);
			return true;
		}
		return false;
	}

	void OnPaySuccess(string arg) {
		showToast ("Payment Success! Time Limit deactive");
		StartCoroutine(AutoClosePopup(POPUP_TYPE_RaS.TLP_SUCCESS));
		PlayerPrefs.SetInt ("TimeLimit", 1);
	}

	void OnPayCanceled(string arg){
		showToast ("Payment was canceled");
		StartCoroutine(AutoClosePopup(POPUP_TYPE_RaS.TLP_CANCELED));
	}

	void OnPayFailed(string arg){
		showToast("Payment was failed. " + arg);
		StartCoroutine(AutoClosePopup(POPUP_TYPE_RaS.TLP_FAILED));
	}

	IEnumerator AutoClosePopup(POPUP_TYPE_RaS type){
		Debug.Log (type.ToString ());
		PopupManager_RaS.Instance.OpenPopup (type);
		yield return new WaitForSeconds (2.0f);
		PopupManager_RaS.Instance.ClosePopup ();
	}
}