using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Main : MonoBehaviour {
	[SerializeField]
	LimitTimer timer;

	[SerializeField]
	Text text;

	// Use this for initialization
	void Start () {
		timer.OnLimitTimeOut += TimeOut;
		timer.OnTimeUpdate += TimeUpdate;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void TimeOut(){
		text.text = "End!";
	}

	public void TimeUpdate(int leftTime){
		text.text = "Remain " + leftTime + " sec";
	}
}
