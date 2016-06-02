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
	}

	public void TimerStart(){
		active = true;
		timeLeft = LimitTime;
		StartCoroutine (updateCoroutine ());
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
			LimitTime -= Time.deltaTime;

			if (LimitTime < 0) {
				active = false;
				if (OnLimitTimeOut != null) {
					OnLimitTimeOut ();
				}
			}
		}
	}

	private IEnumerator updateCoroutine(){
		while(active){
			if (OnTimeUpdate != null) {
				OnTimeUpdate (Mathf.RoundToInt (LimitTime));
			}
			yield return new WaitForSeconds(0.5f);
		}
	}
}
