using UnityEngine;
using System.Collections;

public class PS_CharacterSelectLobbyManager : MonoBehaviour
{
	private LobbyClient lobbyClient;

	public GameObject[] character = new GameObject[(int)CHARACTER_TYPE.CHARACTER_MAX];
	public GameObject selectButton = null;
	public GameObject characterList = null;

	private int selectedCharacter = (int)CHARACTER_TYPE.CHARACTER_NONE;

	public AudioClip selectSound = null;
	public AudioClip selectUnableSound = null;

	void Start ()
	{
		lobbyClient = GameObject.Find ("LobbyClient").GetComponent<LobbyClient>();
	}

	public void SelectButton()
	{
		for(int i = 0; i < character.Length; i++)
		{
			if(character[i].transform.position.x > -0.5f && character[i].transform.position.x < 0.5f)
			{
				if(character[i].GetComponent<PS_SelectCharacter>().IsSoldOut() == true)
				{
					selectButton.GetComponent<UIButton>().isEnabled = true;
					characterList.GetComponent<UIScrollView>().enabled = true;
					selectedCharacter = (int)CHARACTER_TYPE.CHARACTER_NONE;
					lobbyClient.myCharacter = selectedCharacter;

					Camera.main.GetComponent<AudioSource>().PlayOneShot(selectUnableSound);
				}
				else
				{
					selectButton.GetComponent<UIButton>().isEnabled = false;
					characterList.GetComponent<UIScrollView>().enabled = false;
					selectedCharacter = i;
					lobbyClient.myCharacter = (int)selectedCharacter;
					Debug.Log ("SelectBtn :: " + selectedCharacter);
					lobbyClient.SendToHost("CharacterSelect," + selectedCharacter);

					Camera.main.GetComponent<AudioSource>().PlayOneShot(selectSound);
				}
			}
		}
	}

	public void SetCharacterSoldOut(int characterType)
	{
		character[(int)characterType].GetComponent<PS_SelectCharacter>().SetSoldOut(true);
		
		if(selectedCharacter == characterType)
		{	
			selectButton.GetComponent<UIButton>().isEnabled = true;
			characterList.GetComponent<UIScrollView>().enabled = true;
			selectedCharacter = (int)CHARACTER_TYPE.CHARACTER_NONE;
			lobbyClient.myCharacter = selectedCharacter;
			
			Camera.main.GetComponent<AudioSource>().PlayOneShot(selectUnableSound);
		}
	}

	public void CancelCharacterSoldOut(int chartype){
		character[(int)chartype].GetComponent<PS_SelectCharacter>().SetSoldOut(false);
	}

}
