using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UXLib;
using UXLib.Util;

public class YesOrNoPopUp : MonoBehaviour {
	public static int popUpType = -1;
	
	public const int APPLICATION_QUIT = 0;
	public const int POPUP_DESTROY = 1;
	public const int GAME_START = 2;

	public Text message;

	public Button YESButton;
	public Button NOButton;

	public Image messageBg;

	void Start() {
		ReSize();
	}

	public void OnYesButtonUp(){

		if(popUpType == POPUP_DESTROY){
			message.text = "";
			Destroy(gameObject);
			
		}else if(popUpType == APPLICATION_QUIT){
			Application.Quit();
		
		}else if(popUpType == GAME_START){
			message.text = "";
			Destroy(gameObject);
		}
	}

	public void OnNoButtonUp(){
		message.text = "";
		Destroy(gameObject);
	}

	public void ReSize(){
		if(message != null) {
			RectTransform TextRT = message.GetComponent (typeof (RectTransform)) as RectTransform;
			TextRT.sizeDelta = new Vector2 ((int)520, message.preferredHeight);

			RectTransform messageBgRT = messageBg.GetComponent (typeof (RectTransform)) as RectTransform;
			messageBgRT.sizeDelta = new Vector2 ((int)620, message.preferredHeight + 200);

			
			RectTransform yesButtonRT = YESButton.GetComponent (typeof (RectTransform)) as RectTransform;
			yesButtonRT.localPosition = new Vector2 (-130,150-(message.preferredHeight+30));
			
			RectTransform noButtonRT = NOButton.GetComponent (typeof (RectTransform)) as RectTransform;
			noButtonRT.localPosition = new Vector2 (130,150-(message.preferredHeight+30));
		}
	}
}	

