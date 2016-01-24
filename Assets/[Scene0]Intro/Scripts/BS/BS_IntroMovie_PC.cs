using UnityEngine;
using System.Collections;

public class BS_IntroMovie_PC : MonoBehaviour
{
	void Start ()
	{
#if UNITY_STANDALONE
		((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();
		AudioSource tutorialMovieSound = this.GetComponent<AudioSource>();
		tutorialMovieSound.Play();
#endif
		StartCoroutine(StartGameLoading());
	}

	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			StopAllCoroutines();
			Application.LoadLevel("LobbyHost");
		}
	}

	IEnumerator StartGameLoading()
	{
		yield return new WaitForSeconds(33.0f);

		Application.LoadLevel("LobbyHost");
	}
}
