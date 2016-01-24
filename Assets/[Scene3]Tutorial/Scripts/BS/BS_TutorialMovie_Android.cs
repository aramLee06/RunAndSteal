using UnityEngine;
using System.Collections;

public class BS_TutorialMovie_Android : MonoBehaviour
{
	private LobbyHost lobbyHost;

	void Start ()
	{
#if UNITY_ANDROID || UNITY_IOS
		Handheld.PlayFullScreenMovie("tutorial.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
#endif

		lobbyHost = GameObject.Find ("LobbyHost").GetComponent<LobbyHost>();
		lobbyHost.StartInGame();
	}
}
