using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextLocalize : MonoBehaviour {
	[SerializeField]
	public int index;

	private CommonLang commonLang;

	void Awake(){
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
		gameObject.GetComponent<Text> ().text = commonLang.GetWord (index);
	}
}
