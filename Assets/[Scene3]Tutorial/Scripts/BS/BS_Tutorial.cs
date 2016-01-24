using UnityEngine;
using System.Collections;

public class BS_Tutorial : MonoBehaviour
{
	public GameObject tutorialMoviePC = null;
	public GameObject tutorialMovieAndroid = null;
	
	void Start ()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		tutorialMovieAndroid.SetActive(true);
#else
        tutorialMoviePC.SetActive(true);
#endif
	}
}
