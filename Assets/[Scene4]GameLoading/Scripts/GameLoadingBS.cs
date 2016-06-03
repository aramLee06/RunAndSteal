using UnityEngine;
using System.Collections;

public class GameLoadingBS : MonoBehaviour
{
	void Start ()
	{
		StartCoroutine(GameLoading());
		LimitTimer.instance.TimerStart ();
	}

	IEnumerator GameLoading()
	{
		AsyncOperation async = Application.LoadLevelAsync("InGameBS");

		yield return async;
	}
}
