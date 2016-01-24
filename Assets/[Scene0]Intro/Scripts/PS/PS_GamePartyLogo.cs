using UnityEngine;
using System.Collections;

public class PS_GamePartyLogo : MonoBehaviour
{
	void Start ()
	{
		StartCoroutine(GamePartyLogo());
	}

	IEnumerator GamePartyLogo()
	{
		yield return new WaitForSeconds(3.0f);

		Application.LoadLevel("LobbyClient");
	}
}
