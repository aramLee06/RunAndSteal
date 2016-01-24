using UnityEngine;
using System.Collections;

using System.Collections.Generic;
public class CommonLang : MonoBehaviour {
	public static CommonLang instance = null;

	public List<string> langList = new List<string>();

	public language lang;

	void Start () {
		if(instance == null)
		{
			instance = this; 
		}
	}

	public void SeleteLanguage(string language) {

		Debug.Log("language   :  " + language);
		if(language == "chi"){
			for(int i = 0; i < lang.dataArray.Length; i++) {
				langList.Add(lang.dataArray[i].Chinese);	
			}
		}else {
			for(int i = 0; i < lang.dataArray.Length; i++) {
				langList.Add(lang.dataArray[i].English);	
			}
		}

	}

	
	void Update () {
	}
}
