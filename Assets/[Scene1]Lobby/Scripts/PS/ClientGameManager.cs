using UnityEngine;
using UnityEngine.UI;
using UXLib.User;

public class ClientGameManager : MonoBehaviour
{
	public LobbyClient cspLobby;

	public GameObject playerNumber = null;
	public Sprite[] playerNumberSprite = new Sprite[6];

	public PremiumVersionStore inapp;

	void Awake()
	{
		cspLobby = GameObject.Find("LobbyClient").GetComponent<LobbyClient>();

		cspLobby.PlayerIndexChanged += PlayerIndexChanged;
		cspLobby.AfterJoin += AfterJoin;
	}

	void AfterJoin ()
	{
		if (inapp.IsPremiumVersion())
		{
			cspLobby.SendToHost("PREMIUM,");
		}
		inapp.OnPurchaseSuccess += OnPurchaseSuccess;
	}

	void OnPurchaseSuccess ()
	{
		cspLobby.SendToHost("PREMIUM,");
	}

	void PlayerIndexChanged (int index)
	{
		int exIndex = cspLobby.i_PlayerID;
		
		Debug.Log("PlayerIndexChanged : " + index + " player. " + exIndex);

		if(index >= 0)
		{
			playerNumber.GetComponent<SpriteRenderer>().sprite = playerNumberSprite[index];
		}
	}

	void Start() {}

	void Update() {}

}