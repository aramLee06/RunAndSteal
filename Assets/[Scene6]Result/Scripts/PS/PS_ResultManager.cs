using UnityEngine;
using System.Collections;

public class PS_ResultManager : MonoBehaviour
{
	private LobbyClient lobbyClient;

	public GameObject player1UI = null;

	public GameObject medal = null;

	public GameObject characterFace = null;
	public GameObject characterName = null;
	public GameObject rankNumber = null;

	public Sprite[] characterFaceSprite = new Sprite[(int)CHARACTER_TYPE.CHARACTER_MAX];
	public Sprite[] characterNameSprite = new Sprite[(int)CHARACTER_TYPE.CHARACTER_MAX];
	public Sprite[] rankNumberSprite = new Sprite[6];

	public UILabel scoreLabel = null;

	private int myScore = 0;
	private int myRank = 0;
	private int myCharacter = (int)CHARACTER_TYPE.CHARACTER_NONE;

	void Start ()
	{
		lobbyClient = GameObject.Find ("LobbyClient").GetComponent<LobbyClient>();

		myCharacter = lobbyClient.myCharacter;

		if(lobbyClient.IsRoomMaster() == false)
		{
			player1UI.SetActive(false);
		}
		UnityAds.instance.ShowAdPlacement ();
	}

	void Update ()
	{
		myScore = lobbyClient.myScore;
		myRank = lobbyClient.myRank;

		characterFace.GetComponent<SpriteRenderer>().sprite = characterFaceSprite[(int)myCharacter];
		characterName.GetComponent<SpriteRenderer>().sprite = characterNameSprite[(int)myCharacter];

		rankNumber.GetComponent<SpriteRenderer>().sprite = rankNumberSprite[myRank];

		medal.transform.Rotate(Vector3.up * 3.0f);

		scoreLabel.text = "" + myScore;
	}

	public void Player1ReplayButton()
	{
		//lobbyClient.Replay();
		Application.LoadLevel("LobbyClient");
		lobbyClient.SendAll("Replay");
	}

	public void Player1ExitButton()
	{
		Application.LoadLevel("LobbyClient");
		lobbyClient.SendAll("Replay");
        PopupManager_RaS.Instance.CloseGame();
	}
}
