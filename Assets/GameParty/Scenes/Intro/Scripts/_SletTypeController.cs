using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class _SletTypeController : MonoBehaviour {
	public GameObject Logo;
	public GameObject ScreenBtn;
	public GameObject ControllerBtn;
	public GameObject BackDrop;

	public Text portrait_screen_text;
	public Text portriat_controller_text;
	public Text landscape_guide_text;

	public Text CountText;

	private bool isFixed = false;

	// Use this for initialization
	void Start () {
		if (Screen.orientation == ScreenOrientation.Landscape || Screen.orientation == ScreenOrientation.LandscapeRight) {
			onLandscape ();
		} else if (Screen.orientation == ScreenOrientation.Portrait) {
			onPortrait ();
		}
	}
	// Update is called once per frame
	void Update () {
		if (Screen.orientation == ScreenOrientation.Portrait && !isFixed) {
			StopCoroutine ("SceneCountDown");
			onPortrait ();
		}
	}

	public void ScreenMode(){
	 	BackDrop.SetActive (true);
		SceneManager.LoadScene ("BS_LogoViewer");
	}

	public void ControllerMode(){
		SceneManager.LoadScene  ("1_Login");
	}

	void onPortrait(){		
		Screen.orientation = ScreenOrientation.Portrait;
		isFixed = true;

		CountText.gameObject.SetActive (false);
		landscape_guide_text.gameObject.SetActive (false);

		Logo.transform.localPosition = new Vector3 (0, 159, 0);
		Logo.transform.localScale = new Vector3 (1, 1, 0);

		portrait_screen_text.gameObject.SetActive (true);
		portrait_screen_text.transform.localPosition = new Vector3 (0, -17, 0);
		portrait_screen_text.transform.localScale = new Vector3 (1, 1, 0);

		portriat_controller_text.gameObject.SetActive (true);
		portriat_controller_text.transform.localPosition = new Vector3 (4, -306, 0);
		portriat_controller_text.transform.localScale = new Vector3 (1, 1, 0);

		ScreenBtn.transform.localPosition = new Vector3 (0,-121,0);
		ScreenBtn.transform.localScale = new Vector3 (1, 1, 0);

		ControllerBtn.transform.localPosition = new Vector3 (0,-410,0);
	}

	void onLandscape(){		
		portrait_screen_text.gameObject.SetActive (false);
		portriat_controller_text.gameObject.SetActive (false);

		Logo.transform.localPosition = new Vector3 (-190, -100, 0);
		Logo.transform.localScale = new Vector3 (0.8f, 0.8f, 0);

		ScreenBtn.transform.localPosition = new Vector3 (150, -10, 0);
		ScreenBtn.transform.localScale = new Vector3 (0.8f, 0.8f, 0);

		landscape_guide_text.gameObject.SetActive (true);
		landscape_guide_text.transform.localPosition = new Vector3 (200, -195, 0);

		CountText.gameObject.SetActive (true);

		StartCoroutine ("SceneCountDown");
	}

	IEnumerator SceneCountDown (){
		int count = 5;
		while (count >= 0) {
			CountText.text = "Enter lobby screen in "+count+" seconds...";
			yield return new WaitForSeconds (1.0f);
			count--;
		}
		ScreenMode ();
	}
}

