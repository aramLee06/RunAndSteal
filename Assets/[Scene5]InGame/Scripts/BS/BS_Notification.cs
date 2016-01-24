using UnityEngine;
using System.Collections;

public class BS_Notification : MonoBehaviour
{
	public GameObject readyBack = null;
	public GameObject ready = null;
	public GameObject go = null;

	public GameObject hurryUp = null;

	public GameObject feverEffect = null;

	public GameObject gameSetBack = null;
	public GameObject gameSet = null;

	public GameObject inGameManager = null;

	public AudioClip readySound = null;
	public AudioClip goSound = null;

	void Start ()
	{
		readyBack.SetActive(true);

		ready.SetActive(true);
		go.SetActive(false);

		hurryUp.SetActive(false);

		gameSetBack.SetActive(false);
		gameSet.SetActive(false);

		StartCoroutine(PlayReadySound());
	}

	public void Ready()
	{
		ready.SetActive(true);
	}

	public void Go()
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(goSound);

		ready.SetActive(false);
		readyBack.GetComponent<TweenAlpha>().enabled = true;
		go.SetActive(true);

		inGameManager.GetComponent<BS_InGameManager>().GameStart();
	}

	public void HurryUp()
	{
		hurryUp.SetActive(true);
	}

	public void FeverEffect()
	{
		feverEffect.SetActive(true);
	}

	public void HurryUpEnd()
	{
		inGameManager.GetComponent<BS_InGameManager>().StartFeverBGM();
	}

	public void GameSet()
	{
		gameSetBack.SetActive(true);
		gameSet.SetActive(true);
	}

	IEnumerator PlayReadySound()
	{
		yield return new WaitForSeconds(2.0f);
		Camera.main.GetComponent<AudioSource>().PlayOneShot(readySound);
		yield return new WaitForSeconds(1.0f);
		Camera.main.GetComponent<AudioSource>().PlayOneShot(readySound);
		yield return new WaitForSeconds(1.0f);
		Camera.main.GetComponent<AudioSource>().PlayOneShot(readySound);
	}
}
