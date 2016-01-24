using UnityEngine;
using System.Collections;
using System;

public class BS_ResultManager : MonoBehaviour
{
	private LobbyHost lobbyHost;

	private string[] rankSpriteName =
	{
		"(result-b)_rank_1_v1",
		"(result-b)_rank_2_v1",
		"(result-b)_rank_3_v1",
		"(result-b)_rank_4_v1",
		"(result-b)_rank_5_v1",
		"(result-b)_rank_6_v1",
	};

	private string[] nameSpriteName =
	{
		"(result-b)_character_andy",
		"(result-b)_character_bunnee",
		"(result-b)_character_frank",
		"(result-b)_character_jeanpaul",
		"(result-b)_character_johnny",
		"(result-b)_character_johnson",
		"(result-b)_character_roony"
	};

	private int[] sortedTotalScore = new int[6];
	private int[] playerRank = new int[6]; // Rank[Player]
	private int[] playerRealRank = new int[6]; // Player[Rank]

	public GameObject[] playerTable = new GameObject[6];

	public UILabel[] player1ItemScoreLabel = new UILabel[5];
	public UILabel[] player2ItemScoreLabel = new UILabel[5];
	public UILabel[] player3ItemScoreLabel = new UILabel[5];
	public UILabel[] player4ItemScoreLabel = new UILabel[5];
	public UILabel[] player5ItemScoreLabel = new UILabel[5];
	public UILabel[] player6ItemScoreLabel = new UILabel[5];

	public GameObject[] playerRankNumber = new GameObject[6];
	public GameObject[] playerName = new GameObject[6];
	public GameObject[] playerTotalScore = new GameObject[6];

	private int[,] itemScoreCurrent = new int[6, 5];

	private bool isRankOpened = false;

	public AudioClip scoreCountingSound = null;
	public AudioClip finalScoreSound = null;
	public AudioClip celebrationSound = null;

	void Start ()
	{
		lobbyHost = GameObject.Find ("LobbyHost").GetComponent<LobbyHost>();

		/*
		// Test Data
		lobby.GetPlayerCount() = 3;

		lobby.selectedPlayerCharacter[0] = (int)CHARACTER_TYPE.CHARACTER_JOHNNY;
		lobby.selectedPlayerCharacter[1] = (int)CHARACTER_TYPE.CHARACTER_JEAN;
		lobby.selectedPlayerCharacter[2] = (int)CHARACTER_TYPE.CHARACTER_BUNNEE;

		lobby.itemScore[0, 0] = 20;
		lobby.itemScore[0, 1] = 30;
		lobby.itemScore[0, 2] = 30;
		lobby.itemScore[0, 3] = 20;
		lobby.itemScore[0, 4] = 10;
		lobby.itemScore[1, 0] = 12;
		lobby.itemScore[1, 1] = 3;
		lobby.itemScore[1, 2] = 12;
		lobby.itemScore[1, 3] = 32;
		lobby.itemScore[1, 4] = 12;
		lobby.itemScore[2, 0] = 35;
		lobby.itemScore[2, 1] = 32;
		lobby.itemScore[2, 2] = 12;
		lobby.itemScore[2, 3] = 17;
		lobby.itemScore[2, 4] = 7;

		lobby.totalScore[0] = 500;
		lobby.totalScore[1] = 2000;
		lobby.totalScore[2] = 1000;
		*/

		for(int i = 0; i < playerTable.Length; i++)
		{
			if(i < lobbyHost.GetPlayerCount())
			{
				if(lobbyHost.selectedPlayerCharacter[i] == (int)CHARACTER_TYPE.CHARACTER_DISCONNECTED)
				{
					playerTable[i].SetActive(false);
				}
				else
				{
					playerTable[i].SetActive(true);
					playerName[i].GetComponent<UISprite>().spriteName = nameSpriteName[(int)lobbyHost.selectedPlayerCharacter[i]];
				}
			}
			else
			{
				playerTable[i].SetActive(false);
			}
		}

		CalculateRank();

		for(int i = 0; i < player1ItemScoreLabel.Length; i++)
		{
			player1ItemScoreLabel[i].text = "0";
		}
		for(int i = 0; i < player2ItemScoreLabel.Length; i++)
		{
			player2ItemScoreLabel[i].text = "0";
		}
		for(int i = 0; i < player3ItemScoreLabel.Length; i++)
		{
			player3ItemScoreLabel[i].text = "0";
		}
		for(int i = 0; i < player4ItemScoreLabel.Length; i++)
		{
			player4ItemScoreLabel[i].text = "0";
		}
		for(int i = 0; i < player5ItemScoreLabel.Length; i++)
		{
			player5ItemScoreLabel[i].text = "0";
		}
		for(int i = 0; i < player6ItemScoreLabel.Length; i++)
		{
			player6ItemScoreLabel[i].text = "0";
		}

		for(int i = 0; i < playerTotalScore.Length; i++)
		{
			playerTotalScore[i].GetComponent<UILabel>().text = "" + lobbyHost.totalScore[i];
		}
	}

	void Update ()
	{
		bool isAllScoreCurrent = false;
		if(Time.frameCount % 6 == 0 && isAllScoreCurrent == false)
		{
			isAllScoreCurrent = true;
			for(int i = 0; i < lobbyHost.itemScore.GetLength(0); i++)
			{
				for(int j = 0; j < lobbyHost.itemScore.GetLength(1); j++)
				{
					if(itemScoreCurrent[i, j] < lobbyHost.itemScore[i, j])
					{
						itemScoreCurrent[i, j]++;
						isAllScoreCurrent = false;
					}
				}
			}

			for(int i = 0; i < itemScoreCurrent.GetLength(0); i++)
			{
				for(int j = 0; j < itemScoreCurrent.GetLength(1); j++)
				{
					switch(i)
					{
					case 0:
						player1ItemScoreLabel[j].text = "" + itemScoreCurrent[i, j];
						break;
					case 1:
						player2ItemScoreLabel[j].text = "" + itemScoreCurrent[i, j];
						break;
					case 2:
						player3ItemScoreLabel[j].text = "" + itemScoreCurrent[i, j];
						break;
					case 3:
						player4ItemScoreLabel[j].text = "" + itemScoreCurrent[i, j];
						break;
					case 4:
						player5ItemScoreLabel[j].text = "" + itemScoreCurrent[i, j];
						break;
					case 5:
						player6ItemScoreLabel[j].text = "" + itemScoreCurrent[i, j];
						break;
					}
				}
			}

			if(isAllScoreCurrent == false)
			{
				Camera.main.GetComponent<AudioSource>().PlayOneShot(scoreCountingSound);
			}
		}

		if(isAllScoreCurrent == true)
		{
			if(isRankOpened == false)
			{
				isRankOpened = true;
				StartCoroutine(RankOpen());
			}
		}
	}

	void CalculateRank()
	{
		Array.Copy(lobbyHost.totalScore, sortedTotalScore, lobbyHost.totalScore.Length);
		Array.Sort(sortedTotalScore);
		Array.Reverse(sortedTotalScore);

		for(int i = 0; i < lobbyHost.totalScore.Length; i++)
		{
			for(int j = 0; j < sortedTotalScore.Length; j++)
			{
				if(lobbyHost.totalScore[i] == sortedTotalScore[j])
				{
					playerRank[i] = j;
					break;
				}		
			}
		}
		
		Array.Copy (playerRank, playerRealRank, playerRank.Length);
		for(int i = 0; i < playerRealRank.Length; i++)
		{
			if(i != 0)
			{
				for(int j = 0; j < i; j++)
				{
					if(playerRealRank[i] == playerRealRank[j] && i != j)
					{
						playerRealRank[i] = playerRealRank[j] + 1;
					}
				}
			}
		}


		for(int i = 0; i < playerRealRank.Length; i++)
		{
			switch(playerRealRank[i])
			{
			case 0:
				playerTable[i].transform.localPosition = new Vector2(0, 153);
				break;
			case 1:
				playerTable[i].transform.localPosition = new Vector2(0, 51);
				break;
			case 2:
				playerTable[i].transform.localPosition = new Vector2(0, -51);
				break;
			case 3:
				playerTable[i].transform.localPosition = new Vector2(0, -151);
				break;
			case 4:
				playerTable[i].transform.localPosition = new Vector2(0, -254);
				break;
			case 5:
				playerTable[i].transform.localPosition = new Vector2(0, -356);
				break;
			}
		}

		for(int i = 0; i < playerRankNumber.Length; i++)
		{
			playerRankNumber[i].GetComponent<UISprite>().spriteName = rankSpriteName[playerRank[i]];
		}

		for(int i = 0; i < lobbyHost.GetPlayerCount(); i++)
		{
			lobbyHost.SendTo(i, "MyRank," + playerRank[i]);
		}
	}

	IEnumerator RankOpen()
	{
		for(int i = lobbyHost.GetPlayerCount() - 1; i >= 0 ; i--)
		{
			for(int j = playerRealRank.Length - 1; j >= 0 ; j--)
			{
				if(lobbyHost.selectedPlayerCharacter[j] == (int)CHARACTER_TYPE.CHARACTER_DISCONNECTED)
				{
					continue;
				}

				if(i == playerRealRank[j])
				{
					playerRankNumber[j].SetActive(true);
					playerName[j].SetActive(true);
					playerTotalScore[j].SetActive(true);

					Camera.main.GetComponent<AudioSource>().PlayOneShot(finalScoreSound);

					yield return new WaitForSeconds(0.5f);
					break;
				}
			}
		}

		Camera.main.GetComponent<AudioSource>().PlayOneShot(celebrationSound);
	}
}
