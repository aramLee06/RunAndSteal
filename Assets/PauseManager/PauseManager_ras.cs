using UnityEngine;
using System.Collections;

public class PauseManager_ras : MonoBehaviour {
	
	public GameObject ResumeUI;
	public LobbyClient clientLobby;

	void Start()
	{
		try{
			clientLobby = GameObject.Find ("LobbyClient").GetComponent<LobbyClient> ();
		}catch{
			
		}
	}

	public void SendPause()
	{
		clientLobby.SendAll("Pause");
	}
	
	public void GetPause()
	{
		Time.timeScale = 0f;
		ResumeUI.SetActive (true);
	}
	
	public void SendResume()
	{
		clientLobby.SendAll("Resume");
	}
	
	public void GetResume()
	{
		Time.timeScale = 1f;
		ResumeUI.SetActive (false);
	}

}