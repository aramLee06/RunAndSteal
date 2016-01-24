using UnityEngine;
using System.Collections;

public class PS_LobbyExit : MonoBehaviour
{
	void Update ()
	{
		if (Input.GetKey(KeyCode.Escape))
		{
			//Application.Quit();
		}
	}
}
