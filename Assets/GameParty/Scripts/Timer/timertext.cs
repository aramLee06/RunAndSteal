using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class timertext : MonoBehaviour {
	private LimitTimer timer;

	void Awake(){
		timer = LimitTimer.instance;
		if (timer == null) {
			Destroy (this.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		timer.OnLimitTimeOut += TimeOut;
		timer.OnTimeUpdate += TimeUpdate;

		if (!timer.active) {
			Destroy (gameObject);
		}
	}
		
	void OnDestroy ()
	{
		timer.OnLimitTimeOut -= TimeOut;
		timer.OnTimeUpdate -= TimeUpdate;
	}
		
	public void TimeOut(){
		gameObject.GetComponent<Text> ().text = "byebye";
	}

	public void TimeUpdate(int leftTime){
		gameObject.GetComponent<Text> ().text = leftTime.ToString ();
	}
}
