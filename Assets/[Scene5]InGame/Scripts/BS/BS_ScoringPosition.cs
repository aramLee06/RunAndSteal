using UnityEngine;
using System.Collections;

public class BS_ScoringPosition : MonoBehaviour
{
	private LobbyHost lobbyHost;

	public float bondDistance = 0.0f;
	public float bondDamping = 5.0f;

	private ArrayList scoredTails = new ArrayList();

	public GameObject scoreBoard = null;
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("ScoredItem"))
		{
			if(other.gameObject.GetComponent<BS_Item>().IsScored() == true)
			{
				int headPlayer = other.gameObject.GetComponent<BS_Item>().GetHeadPlayer();
				BS_ScoreManager.Instance().PlusScore(headPlayer, other.gameObject.GetComponent<BS_Item>().GetScore());

				scoredTails.RemoveAt(0);
				Destroy(other.gameObject);

				int headPlayerScore = BS_ScoreManager.Instance().GetPlayerScore(headPlayer);

				iTween.PunchScale(scoreBoard.gameObject, Vector3.one * 1.05f, 0.05f);
				
				Color playerColor = Color.white;
				switch(headPlayer)
				{
				case 0:
					playerColor = new Color(1.0f, 0.08f, 0.08f, 1.0f); // Red
					break;
				case 1:
					playerColor = new Color(1.0f, 0.65f, 0.0f, 1.0f); // Orange
					break;
				case 2:
					playerColor = new Color(1.0f, 0.93f, 0.0f, 1.0f); // Yellow
					break;
				case 3:
					playerColor = new Color(0.27f, 0.95f, 1.0f, 1.0f); // Blue
					break;
				case 4:
					playerColor = new Color(0.85f, 0.19f, 1.0f, 1.0f); // Violet
					break;
				case 5:
					playerColor = new Color(0.0f, 0.0f, 0.0f, 1.0f); // Black
					break;
				}
				
				switch(other.gameObject.GetComponent<BS_Item>().GetItemType())
				{
				case ITEM_TYPE.ITEM_APPLE:
					lobbyHost.itemScore[headPlayer, (int)ITEM_TYPE.ITEM_APPLE]++;
					lobbyHost.SendToCode(lobbyHost.GameUserList[headPlayer], "SellSound," + headPlayerScore);
					break;
				case ITEM_TYPE.ITEM_SILVER:
					lobbyHost.itemScore[headPlayer, (int)ITEM_TYPE.ITEM_SILVER]++;
					lobbyHost.SendToCode(lobbyHost.GameUserList[headPlayer], "SellSound," + headPlayerScore);
					break;
				case ITEM_TYPE.ITEM_GOLD:
					lobbyHost.itemScore[headPlayer, (int)ITEM_TYPE.ITEM_GOLD]++;
					lobbyHost.SendToCode(lobbyHost.GameUserList[headPlayer], "SellSound," + headPlayerScore);
					break;
				case ITEM_TYPE.ITEM_RING:
					lobbyHost.itemScore[headPlayer, (int)ITEM_TYPE.ITEM_RING]++;
					lobbyHost.SendToCode(lobbyHost.GameUserList[headPlayer], "SellSound," + headPlayerScore);
					break;
				case ITEM_TYPE.ITEM_DIAMOND:
					lobbyHost.itemScore[headPlayer, (int)ITEM_TYPE.ITEM_DIAMOND]++;
					lobbyHost.SendToCode(lobbyHost.GameUserList[headPlayer], "SellSound," + headPlayerScore);
					break;
				case ITEM_TYPE.ITEM_SPEED:
				case ITEM_TYPE.ITEM_MAGNET:
				case ITEM_TYPE.ITEM_BONUS:
					lobbyHost.SendToCode(lobbyHost.GameUserList[headPlayer], "SpecialSellSound," + headPlayerScore);
					break;
				}
			}
		}
	}

	void Start ()
	{
		lobbyHost = GameObject.Find ("LobbyHost").GetComponent<LobbyHost>();
	}

	void Update()
	{
		TailsMove();
	}

	public void AddTail(GameObject tail)
	{
		scoredTails.Add(tail.gameObject);
	}

	void TailsMove()
	{
		for(int i = 0; i < scoredTails.Count; i++)
		{
			GameObject preTail;
			GameObject thisTail;
			
			if(i == 0)
			{
				preTail = this.gameObject;
			}
			else
			{
				preTail = (GameObject)scoredTails[i - 1];
				if(preTail == null)
				{
					preTail = this.gameObject;
				}
			}
			
			thisTail = (GameObject)scoredTails[i];
			
			Vector3 wantedPosition = preTail.transform.TransformPoint(0, 0, -bondDistance); 
			thisTail.transform.position = Vector3.Lerp (thisTail.transform.position, wantedPosition, Time.deltaTime * bondDamping); 
			thisTail.GetComponent<Rigidbody>().velocity = Vector3.zero;
			
			Quaternion wantedRotation = Quaternion.LookRotation(preTail.transform.position - thisTail.transform.position, preTail.transform.up);
			thisTail.transform.rotation = Quaternion.Slerp (thisTail.transform.rotation, wantedRotation, Time.deltaTime * bondDamping);
			thisTail.transform.eulerAngles = new Vector3(0, thisTail.transform.eulerAngles.y, thisTail.transform.eulerAngles.z);
		}
	}
}
