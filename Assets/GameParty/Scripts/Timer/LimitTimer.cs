using UnityEngine;
using System.Collections;

public class LimitTimer : MonoBehaviour {
	[SerializeField]
	public float LimitTime;

	[SerializeField]
	public bool active;

	private float timeLeft;

	public static LimitTimer instance = null;

	public delegate void LimitTimeOutHandler();
	public event LimitTimeOutHandler OnLimitTimeOut;

	public delegate void TimeUpdateHandler(int leftTime);
	public event TimeUpdateHandler OnTimeUpdate;

	void Awake(){
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad (this.gameObject);
		} else {
			Destroy (this.gameObject);
		}
		timeLeft = LimitTime;
	}

	public void TimerStart(){
		active = true;
		StartCoroutine (updateCoroutine ());
	}

	public void TimerReset(){
		timeLeft = LimitTime;
	}

	public void TimerStop(){
		active = false;
	}

	// Use this for initialization
	void Start () {
		if (active) {
			TimerStart ();	
		}
	}

	// Update is called once per frame
	void Update () {
		if (active) {
			timeLeft -= Time.deltaTime;

			if (timeLeft < 0) {
				active = false;
				if(OnLimitTimeOut != null)
					OnLimitTimeOut ();
			}
		}
	}

	private IEnumerator updateCoroutine(){
		while(active){
			if(OnTimeUpdate != null)
				OnTimeUpdate(Mathf.RoundToInt(timeLeft));
			yield return new WaitForSeconds(0.5f);
		}
	}
}