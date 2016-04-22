using UnityEngine;
using System.Collections;

public class BS_CharacterSelectLobbyManager : MonoBehaviour
{
	private LobbyHost lobbyHost;

	public GameObject[] playerNumber = new GameObject[6];

	public GameObject[] character = new GameObject[6];

	public GameObject[] characterName = new GameObject[6];
	public Sprite[] characterNameSpirte= new Sprite[7];

	private bool isAllSelected = false;
	private bool isCountDown = false;

	public AudioClip characterSelectSound = null;

	public GameObject sceneChange = null;

	void Start ()
	{
		lobbyHost = GameObject.Find ("LobbyHost").GetComponent<LobbyHost>();

		for(int i = 0; i < playerNumber.Length; i++)
		{
			playerNumber[i].SetActive(false);
		}
		for(int i = 0; i < lobbyHost.GetPlayerCount(); i++)
		{
			playerNumber[i].SetActive(true);
		}

		for(int i = 0; i < lobbyHost.selectedPlayerCharacter.Length; i++)
		{
			lobbyHost.selectedPlayerCharacter[i] = (int)CHARACTER_TYPE.CHARACTER_NONE;
		}
	}

	void Update ()
	{
		isAllSelected = true;
		for(int i = 0; i < lobbyHost.GameUserList.Count; i++)
		{
			if(lobbyHost.selectedPlayerCharacter[i] == (int)CHARACTER_TYPE.CHARACTER_NONE)
			{
				isAllSelected = false;
				break;
			}
		}

		if(isAllSelected == true && isCountDown == false)
		{
			isCountDown = true;

			StartCoroutine(TutorialStart());
		}
	}

	public void SetSelectedCharacter(int player, int characterType) //player = idx
	{
		if(characterType > (int)CHARACTER_TYPE.CHARACTER_MAX)
		{
			return;
		}

		Camera.main.GetComponent<AudioSource>().PlayOneShot(characterSelectSound);

		Debug.Log ("selectedPlayerCharacter : " + lobbyHost.selectedPlayerCharacter.Length + ", " + player);
		if (lobbyHost.selectedPlayerCharacter.Length > player)
			lobbyHost.selectedPlayerCharacter[player] = (int)characterType;

		if (character.Length > player) {
			character [player].SetActive (true);
			character [player].GetComponent<BS_SelectCharacter> ().SetCharacter (characterType);
		}

		if (characterName.Length > player) 
			characterName[player].GetComponent<SpriteRenderer>().sprite = characterNameSpirte[(int)characterType];

		if (playerNumber.Length > player)
			iTween.RotateTo(playerNumber[player], iTween.Hash("rotation", new Vector3(0, -180.0f, 0), "time", 2.0f, "easetype", iTween.EaseType.easeOutElastic));
	}
	public void ClearCharacter (int userIndex)
	{
		if(lobbyHost.selectedPlayerCharacter[userIndex] == (int)CHARACTER_TYPE.CHARACTER_DISCONNECTED)//
		{
			playerNumber[userIndex].SetActive(false);
			character[userIndex].SetActive(false);
		}
	}

	public bool IsSoldOutCharacter(int characterType)
	{
		bool isSoldOut = false;

		for(int i = 0; i < lobbyHost.selectedPlayerCharacter.Length; i++)
		{
			if(lobbyHost.selectedPlayerCharacter[i] == characterType)
			{
				isSoldOut = true;
				break;
			}
		}

		return isSoldOut;
	}

	IEnumerator TutorialStart()
	{
		yield return new WaitForSeconds(3.0f);

		sceneChange.SetActive(true);

		yield return new WaitForSeconds(2.0f);

		lobbyHost.StartTutorial();
	}
}
 