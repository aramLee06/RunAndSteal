using UnityEngine;
using System.Collections;

public class BS_ScoreManager : MonoBehaviour
{
	public static BS_ScoreManager instance = null;

	private int[] playerScore = new int[6];

	public UILabel[] playerScoreLabel = new UILabel[6];

	void Start ()
	{
		if(instance == null)
		{
			instance = this;
		}

		for(int i = 0; i < playerScore.Length; i++)
		{
			playerScore[i] = 0;
		}
	}

	void Update ()
	{
		for(int i = 0; i < playerScore.Length; i++)
		{
			playerScoreLabel[i].text = playerScore[i] + "";
		}
	}

	public static BS_ScoreManager Instance()
	{
		return instance;
	}

	public void PlusScore(int player, int score)
	{
		playerScore[player] += score;
	}

	public int GetPlayerScore(int player)
	{
		return playerScore[player];
	}
}
