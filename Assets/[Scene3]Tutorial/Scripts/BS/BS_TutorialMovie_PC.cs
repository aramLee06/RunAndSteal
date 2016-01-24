using UnityEngine;
using System.Collections;

public class BS_TutorialMovie_PC : MonoBehaviour
{
	private LobbyHost lobbyHost;

	void Start ()
	{
#if UNITY_STANDALONE || UNITY_EDITOR
        lobbyHost.StartInGame();
        return;

		((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();
		AudioSource tutorialMovieSound = this.GetComponent<AudioSource>();
		tutorialMovieSound.Play();
#endif
		StartCoroutine(StartGameLoading());

		lobbyHost = GameObject.Find ("LobbyHost").GetComponent<LobbyHost>();
	}

	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			StopAllCoroutines();
			lobbyHost.StartInGame();
		}
	}

	IEnumerator StartGameLoading()
	{
		yield return new WaitForSeconds(28.0f);

		lobbyHost.StartInGame();
	}
}
