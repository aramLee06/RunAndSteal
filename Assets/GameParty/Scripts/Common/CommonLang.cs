using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

public enum LanguageType {
	KOR, ENG, CHI
}

public class CommonLang : MonoBehaviour{
	public static CommonLang instance {
		get {
			return _instance;
		}
	}
	private static CommonLang _instance = null;

	private LanguageType language;
	public LanguageType Language { 
		get {
			return language;
		}
		set{
			this.language = value;
			if (OnLanaugeChange != null)
				OnLanaugeChange ();
		}
	}

	[SerializeField]
	public language lang;

	public delegate void OnLanguageChangeEvent();
	public event OnLanguageChangeEvent OnLanaugeChange;

	void Awake () {
		if (_instance == null) {
			_instance = this; 
		} else {
			Destroy (gameObject);
		}
	}

	public string GetWord(int index){
		return GetWord (index, this.Language);
	}

	public string GetWord(int index, LanguageType type){
		string word = "";
		try {
			switch (this.Language) {
			case LanguageType.CHI :
				word = lang.dataArray[index].Chinese;
				break;
			case LanguageType.KOR :
				word = lang.dataArray[index].Korean;
				break;
			case LanguageType.ENG :	
				word = lang.dataArray[index].English;
				break;
			}
		} catch(Exception e){
			word = "No Data";
		}

		return word;
	}
}
