using UnityEngine;
using System.Collections;

public class BS_IntroMovie_Android : MonoBehaviour
{
	void Start ()
    {
#if UNITY_ANDROID
		Handheld.PlayFullScreenMovie("intro.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
#endif

		Application.LoadLevel("LobbyHost");
	}
}
