using UnityEngine;
using System.Collections;

public class BS_InGameManager : MonoBehaviour
{
	private LobbyHost lobbyHost;

	private string[] boardSpriteName =
	{
		"(ingame-b)_scorebar-andy_v1",
		"(ingame-b)_scorebar-bunnee_v1",
		"(ingame-b)_scorebar-frank_v1",
		"(ingame-b)_scorebar-jeanpaul_v1",
		"(ingame-b)_scorebar-johnny_v1",
		"(ingame-b)_scorebar-johnson_v1",
		"(ingame-b)_scorebar-roony_v1"
	};
	public GameObject[] scoreBoard = new GameObject[6];
	public GameObject[] character = new GameObject[6];

	public GameObject storeManager = null;
	public GameObject itemMaker = null;

	private bool isGameStart = false;
	private bool isGameSet = false;
	private bool isFeverTime = false;
	private float gameTime = 120.0f;
	private float feverTime = 31.0f;
	public UILabel timeLabel = null;

	public GameObject notification = null;

	public AudioClip mainBGM = null;
	public AudioClip feverBGM = null;
	public AudioClip feverSound = null;
	public AudioClip gameSetSound = null;
	public AudioClip countDownSound = null;

	private Vector3 cameraStartPosition = new Vector3(0, 50.0f, -122.0f);
	private float cameraStartSize = 0.1f;

	private Vector3 cameraFinalPosition = new Vector3(0, 50.0f, -110.0f);
	private float cameraFinalSize = 23.0f;

	private float cameraStartTime = 5.0f;

	private bool isCountDown = false;

	public GameObject map = null;

	void OnGUI()
	{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
		/*
		if(GUI.Button(new Rect(0, 300, 300, 100), "Reset"))
		{
			Application.LoadLevel("InGame");
		} */
#endif
	}
	
	void Start ()
	{
		lobbyHost = GameObject.Find ("LobbyHost").GetComponent<LobbyHost>();

		lobbyHost.SendAll("StartInGame");

		for(int i = 0; i < lobbyHost.totalScore.Length; i++)
		{
			lobbyHost.totalScore[i] = 0;
		}

		for(int i = 0; i < lobbyHost.itemScore.GetLength(0); i++)
		{
			for(int j = 0; j < lobbyHost.itemScore.GetLength(1); j++)
			{
				lobbyHost.itemScore[i, j] = 0;
			}
		}

		for(int i = 0; i < lobbyHost.GetPlayerCount(); i++)
		{
			if(lobbyHost.selectedPlayerCharacter[i] == (int)CHARACTER_TYPE.CHARACTER_DISCONNECTED)
			{
				continue;
			}

			scoreBoard[i].SetActive(true);
			scoreBoard[i].GetComponent<UISprite>().spriteName = boardSpriteName[(int)lobbyHost.selectedPlayerCharacter[i]];
		}

		switch(lobbyHost.GetPlayerCount())
		{
		case 1:
			character[0].transform.position = new Vector3(0, character[0].transform.position.y, 0);
			character[1].SetActive(false);
			character[2].SetActive(false);
			character[3].SetActive(false);
			character[4].SetActive(false);
			character[5].SetActive(false);
			break;
		case 2:
			character[0].transform.position = new Vector3(0, character[0].transform.position.y, 10);
			character[1].transform.position = new Vector3(0, character[1].transform.position.y, -10);
			character[2].SetActive(false);
			character[3].SetActive(false);
			character[4].SetActive(false);
			character[5].SetActive(false);
			break;
		case 3:
			character[0].transform.position = new Vector3(0, character[0].transform.position.y, 10);
			character[1].transform.position = new Vector3(-10, character[1].transform.position.y, -5);
			character[2].transform.position = new Vector3(10, character[2].transform.position.y, -5);
			character[3].SetActive(false);
			character[4].SetActive(false);
			character[5].SetActive(false);
			break;
		case 4:
			character[0].transform.position = new Vector3(-10, character[0].transform.position.y, 5);
			character[1].transform.position = new Vector3(10, character[1].transform.position.y, 5);
			character[2].transform.position = new Vector3(-10, character[2].transform.position.y, -5);
			character[3].transform.position = new Vector3(10, character[3].transform.position.y, -5);
			character[4].SetActive(false);
			character[5].SetActive(false);
			break;
		case 5:
			character[0].transform.position = new Vector3(-10, character[0].transform.position.y, 5);
			character[1].transform.position = new Vector3(0, character[1].transform.position.y, 10);
			character[2].transform.position = new Vector3(10, character[2].transform.position.y, 5);
			character[3].transform.position = new Vector3(-7, character[3].transform.position.y, -5);
			character[4].transform.position = new Vector3(7, character[4].transform.position.y, -5);
			character[5].SetActive(false);
			break;
		case 6:
			character[0].transform.position = new Vector3(-10, character[0].transform.position.y, 5);
			character[1].transform.position = new Vector3(0, character[1].transform.position.y, 10);
			character[2].transform.position = new Vector3(10, character[2].transform.position.y, 5);
			character[3].transform.position = new Vector3(-10, character[3].transform.position.y, -5);
			character[4].transform.position = new Vector3(0, character[4].transform.position.y, -10);
			character[5].transform.position = new Vector3(10, character[5].transform.position.y, -5);
			break;
		}

		timeLabel.text = "02:00";

		storeManager.SetActive(false);
		itemMaker.SetActive(false);

		Camera.main.transform.position = cameraStartPosition;
		Camera.main.orthographicSize = cameraStartSize;

		iTween.MoveTo(Camera.main.gameObject, iTween.Hash("position", cameraFinalPosition,
		                                                  "easetype", iTween.EaseType.easeOutExpo,
		                                                  "time", cameraStartTime,
		                                                  "delay", 3.0f));
		iTween.ValueTo (this.gameObject, iTween.Hash ("from", cameraStartSize,
		                                                     "to", cameraFinalSize,
		                                                     "time", cameraStartTime,
		                                                     "easetype", iTween.EaseType.easeOutExpo,
		                                                     "onupdate", "UpdateOrthographicCameraSize",
		                                             		 "delay", 3.0f));
	}
	
	void UpdateOrthographicCameraSize(float size)
	{
		Camera.main.orthographicSize = size;
	}

	void Update ()
	{
		for(int i = 0; i < lobbyHost.GetPlayerCount(); i++)
		{
			if(lobbyHost.selectedPlayerCharacter[i] == (int)CHARACTER_TYPE.CHARACTER_DISCONNECTED)
			{
				scoreBoard[i].SetActive(false);
				character[i].SetActive(false);
				character[i].gameObject.GetComponent<BS_Character> ().LeavedTail();//
			}
		}
		if(isGameStart == true && isGameSet == false)
		{
			gameTime -= Time.deltaTime;
		}

		string segmentTimeStirng;
		if(gameTime >= 0)
		{
			segmentTimeStirng = "0";

			if((int)gameTime / 60 == 1)
			{
				segmentTimeStirng += " ";
			}
			segmentTimeStirng += ((int)gameTime / 60);

			segmentTimeStirng += ":";

			if(((int)gameTime % 60) / 10 == 1)
			{
				segmentTimeStirng += " ";
			}
			segmentTimeStirng += ((int)gameTime % 60) / 10;

			if(((int)gameTime % 60 % 10) == 1)
			{
				segmentTimeStirng += " ";
			}
			segmentTimeStirng += ((int)gameTime % 60) % 10;
		}
		else if(gameTime >= -3)
		{
			if(Time.frameCount % 30 > 15)
			{
				segmentTimeStirng = "00:00";
			}
			else
			{
				segmentTimeStirng = " ";
			}

		}
		else
		{
			segmentTimeStirng = "  E:ND";
		}
		timeLabel.text = segmentTimeStirng;

		if(isFeverTime == true)
		{
			timeLabel.GetComponent<UILabel>().color = Color.red;
		}
		else
		{
			timeLabel.GetComponent<UILabel>().color = Color.yellow;
		}

		if(gameTime <= feverTime && isFeverTime == false)
		{
			isFeverTime = true;
			notification.GetComponent<BS_Notification>().HurryUp();

			Camera.main.GetComponent<AudioSource>().Stop();
			Camera.main.GetComponent<AudioSource>().PlayOneShot(feverSound);

			map.GetComponent<BS_Map>().ChangeNightTexture();
		}

		if(gameTime <= 10 && isCountDown == false)
		{
			isCountDown = true;
			StartCoroutine(PlayCountDownSound());
		}

		if(gameTime <= 0.0f && isGameSet == false)
		{
			isGameSet = true;
			notification.GetComponent<BS_Notification>().GameSet();

			for(int i = 0; i < character.Length; i++)
			{
				character[i].GetComponent<BS_Character>().StopCharacter();
			}

			for(int i = 0; i < lobbyHost.totalScore.Length; i++)
			{
				if(lobbyHost.selectedPlayerCharacter[i] == (int)CHARACTER_TYPE.CHARACTER_DISCONNECTED)
				{
					continue;
				}

				lobbyHost.totalScore[i] = BS_ScoreManager.Instance().GetPlayerScore(i);
			}

			for(int i = 0; i < lobbyHost.GetPlayerCount(); i++) //i = userindex
			{
				//lobbyHost.SendTo(i, "MyScore," + lobbyHost.totalScore[i]);
				lobbyHost.SendToCode(lobbyHost.GameUserList[i], "MyScore," + lobbyHost.totalScore[i]);
			}

			StartCoroutine(GameSet());
		}
	}

	public void JoystickDown(int player, int touchAngle)
	{
		if(isGameStart == true && isGameSet == false)
		{
			character[player].GetComponent<BS_Character>().MoveCharacter(touchAngle, true);
		}
	}

	public void JoystickUp(int player)
	{
		character[player].GetComponent<BS_Character>().StopCharacter();
	}

	public void GameStart()
	{
		isGameStart = true;

		storeManager.SetActive(true);
		itemMaker.SetActive(true);

		Camera.main.GetComponent<AudioSource>().clip = mainBGM;
		Camera.main.GetComponent<AudioSource>().loop = true;
		Camera.main.GetComponent<AudioSource>().Play();
	}

	public bool IsFeverTime()
	{
		return isFeverTime;
	}

	IEnumerator GameSet()
	{
		yield return new WaitForSeconds(5.0f);

		lobbyHost.StartResult();
	}

	public void StartFeverBGM()
	{
		Camera.main.GetComponent<AudioSource>().clip = feverBGM;
		Camera.main.GetComponent<AudioSource>().loop = true;
		Camera.main.GetComponent<AudioSource>().Play();
	}

	public void PlayGameSetSound()
	{
		Camera.main.GetComponent<AudioSource>().Stop();
		Camera.main.GetComponent<AudioSource>().PlayOneShot(gameSetSound);
	}

	IEnumerator PlayCountDownSound()
	{
		for(int i = 0; i < 10; i++)
		{
			Camera.main.GetComponent<AudioSource>().PlayOneShot(countDownSound);
			yield return new WaitForSeconds(1.0f);
		}
	}
}
