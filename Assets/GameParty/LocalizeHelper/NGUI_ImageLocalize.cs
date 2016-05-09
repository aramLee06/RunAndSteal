using UnityEngine;
using System.Collections;

public class NGUI_ImageLocalize : MonoBehaviour {
	[SerializeField]
	public string English;

	[SerializeField]
	public string Chinese;

	[SerializeField]
	public string Korean;

	private bool isWorkOK = true;

	private CommonLang commonLang;


	void Awake(){
		if (string.IsNullOrEmpty(English)) {
			isWorkOK = false;
			return;
		}

		if (string.IsNullOrEmpty(Chinese)) {
			Chinese = English;
		}

		if (string.IsNullOrEmpty(Korean)) {
			Korean = English;
		}

		commonLang = CommonLang.instance;
	}

	// Use this for initialization
	void Start () {
		commonLang.OnLanaugeChange += OnLanguageChange;
		OnLanguageChange ();
	}

	void OnDestroy(){
		commonLang.OnLanaugeChange -= OnLanguageChange;
	}
	
	private void OnLanguageChange(){
		if (isWorkOK) {
			string target = "";
			switch (commonLang.Language) {
			case LanguageType.CHI:
				target = Chinese;
				break;
			case LanguageType.KOR:
				target = Korean;
				break;
			case LanguageType.ENG:
				target = English;
				break;
			}

			gameObject.GetComponent<UISprite> ().spriteName = target;	
		}
	}
}
