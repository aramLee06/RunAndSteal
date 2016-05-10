using UnityEngine;
using System.Collections;

public class ChangeLanguageController : MonoBehaviour {
	private CommonLang commonLang;

	// Use this for initialization
	void Start () {
		commonLang = CommonLang.instance;
	}

	public void OnKoreanButton(){
		commonLang.Language = LanguageType.KOR;
	}

	public void OnEnglishButton(){
		commonLang.Language = LanguageType.ENG;
	}

	public void OnChineseButton(){
		commonLang.Language = LanguageType.CHI;
	}

}
