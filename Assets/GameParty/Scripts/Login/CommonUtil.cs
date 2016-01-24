using UnityEngine;
using System.Collections;
using UXLib.User;
using UnityEngine.UI;

public class CommonUtil : MonoBehaviour {

	public static Rect ResizeGUI(Rect resizeRect){
		float FilScreenWidth = resizeRect.width / 720;
		float rectWidth = FilScreenWidth * Screen.width;
		float FilScreenHeight = resizeRect.height / 1280;
		float rectHeight = FilScreenHeight * Screen.height;
		float rectX = (resizeRect.x / 720) * Screen.width;
		float rectY = (resizeRect.y / 1280) * Screen.height;
		
		return new Rect(rectX, rectY, rectWidth, rectHeight);
	}

	public static void ScreenSettingsPortrait() {
		Screen.SetResolution (720, 1280, true);
		Screen.orientation = ScreenOrientation.Portrait;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	public static void ScreenSettingsLandscape() {
		Screen.SetResolution (1280, 720, true);
		Screen.orientation = ScreenOrientation.Landscape;
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	public static void InstantiateOKPopUp(string message){
		GameObject go = GameObject.Find("Canvas/OKPopUp(Clone)");
		if(go != null){
			return;
		}
		GameObject okPopUp = Instantiate (Resources.Load ("Prefabs/OKPopUp")) as GameObject; 
		okPopUp.transform.SetParent(GameObject.Find("Canvas").transform, false);
		okPopUp.transform.localScale = new Vector3(1,1,1);

		Text errMessage = okPopUp.transform.FindChild("ErrorMessage").GetComponent<Text>();
		errMessage.text = message;

		YesOrNoPopUp popup = new YesOrNoPopUp();	
		popup.ReSize();

	}

	public static void InstantiateYesNoPopUP(string message){
		GameObject go = GameObject.Find("Canvas/YesNoPopUp(Clone)");
		if(go != null){
			return;
		}

		GameObject yesNoPopUp = Instantiate (Resources.Load ("Prefabs/YesNoPopUp")) as GameObject; 
		yesNoPopUp.transform.SetParent(GameObject.Find("Canvas").transform, false);
		yesNoPopUp.transform.localScale = new Vector3(1,1,1);

		Text errMessage = yesNoPopUp.transform.FindChild("ErrMessage").GetComponent<Text>();
		errMessage.text = message;

	}	
}