using UnityEngine;
using System.Collections;

public class BS_Intro : MonoBehaviour
{
	public GameObject introMoviePC = null;
	public GameObject introMovieAndroid = null;
	
	void Start ()
    {
#if UNITY_ANDROID || UNITY_IOS
		introMovieAndroid.SetActive(true);
#else
		introMoviePC.SetActive(true);
#endif
	}
}
