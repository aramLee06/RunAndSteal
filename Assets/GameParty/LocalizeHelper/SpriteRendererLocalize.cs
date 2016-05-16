using UnityEngine;
using System.Collections;

public class SpriteRendererLocalize : MonoBehaviour {
	[SerializeField]
	Sprite English;

	[SerializeField]
	Sprite Korean;

	[SerializeField]
	Sprite Chinese;

	[SerializeField]


	private CommonLang commonLang;

	void Awake(){
		commonLang = CommonLang.instance;
	}
	// Use this for initialization
	void Start () {
		if (Korean == null)
			Korean = English;
		if (Chinese == null)
			Chinese = English;
		commonLang.OnLanaugeChange += OnLanguageChange;
		OnLanguageChange ();
	}

	void OnDestroy(){
		commonLang.OnLanaugeChange -= OnLanguageChange;
	}

	private void OnLanguageChange(){
		Sprite target = null;
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

		gameObject.GetComponent<SpriteRenderer> ().sprite = target;
	}
}
