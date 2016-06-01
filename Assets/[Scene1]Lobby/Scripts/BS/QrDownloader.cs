using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QrDownloader : MonoBehaviour {

	public string url = "http://cdn.playgameparty.com/code.png";

	IEnumerator Start () {
		
		WWW www = new WWW(url);
		yield return www;
		gameObject.GetComponent<RawImage> ().texture = www.texture;
	}

}


