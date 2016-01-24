using UnityEngine;
using System.Collections;

public class GameLoadingBS : MonoBehaviour
{
	void Start ()
	{
		StartCoroutine(GameLoading());
	}

	IEnumerator GameLoading()
	{
		AsyncOperation async = Application.LoadLevelAsync("InGameBS");

		yield return async;
	}
}
