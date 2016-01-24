using UnityEngine;
using System.Collections;

public class BS_Store : MonoBehaviour
{
	private ArrayList scoredTails = new ArrayList();
	public float bondDistance = 0.0f;
	public float bondDamping = 5.0f;

	private bool isActiveStore = false;

	public GameObject openBoard = null;
	private float openBoardOriginalHeight = 0;

	public GameObject openArrow = null;

	public GameObject[] scoringPosition = new GameObject[6];

	private int blinkStartFrame;
	private float arrowSpeed = 0;

	public GameObject gate = null;
	public GameObject respawnPosition = null;

	public GameObject bonusEffect = null;

	void OnTriggerEnter(Collider other)
	{
		if(isActiveStore == true)
		{
			if(other.gameObject.layer == LayerMask.NameToLayer("Character"))
			{
				int scoredPlayer = other.gameObject.GetComponent<BS_Character>().playerNumber;
				int scoredPlayerTailLength = this.transform.GetComponentInParent<BS_InGameManager>().character[scoredPlayer].GetComponent<BS_Character>().tails.Count;
				
				if(scoredPlayerTailLength > 0)
				{
					for(int i = 0; i < scoredPlayerTailLength; i++)
					{
						GameObject scoredTail = (GameObject)this.transform.GetComponentInParent<BS_InGameManager>().character[scoredPlayer].GetComponent<BS_Character>().tails[0];

						if(other.gameObject.GetComponent<BS_Character>().IsBonusItem() == true)
						{
							switch(scoredTail.gameObject.GetComponent<BS_Item>().GetItemType())
							{
							case ITEM_TYPE.ITEM_APPLE:
								scoredTail.GetComponent<BS_Item>().SetScore((10 + i) * 2);
								break;
							case ITEM_TYPE.ITEM_SILVER:
								scoredTail.GetComponent<BS_Item>().SetScore((20 + i) * 2);
								break;
							case ITEM_TYPE.ITEM_GOLD:
								scoredTail.GetComponent<BS_Item>().SetScore((30 + i) * 2);
								break;
							case ITEM_TYPE.ITEM_RING:
								scoredTail.GetComponent<BS_Item>().SetScore((40 + i) * 2);
								break;
							case ITEM_TYPE.ITEM_DIAMOND:
								scoredTail.GetComponent<BS_Item>().SetScore((50 + i) * 2);
								break;
							case ITEM_TYPE.ITEM_SPEED:
							case ITEM_TYPE.ITEM_MAGNET:
							case ITEM_TYPE.ITEM_BONUS:
								scoredTail.GetComponent<BS_Item>().SetScore((0 + i) * 2);
								break;
							}

							GameObject bonusEffectClone = Instantiate(bonusEffect) as GameObject;
							bonusEffectClone.transform.position = other.gameObject.transform.position;
							bonusEffectClone.transform.position += new Vector3(0, 1.0f, 0);
						}
						else
						{
							switch(scoredTail.gameObject.GetComponent<BS_Item>().GetItemType())
							{
							case ITEM_TYPE.ITEM_APPLE:
								scoredTail.GetComponent<BS_Item>().SetScore(10 + i);
								break;
							case ITEM_TYPE.ITEM_SILVER:
								scoredTail.GetComponent<BS_Item>().SetScore(20 + i);
								break;
							case ITEM_TYPE.ITEM_GOLD:
								scoredTail.GetComponent<BS_Item>().SetScore(30 + i);
								break;
							case ITEM_TYPE.ITEM_RING:
								scoredTail.GetComponent<BS_Item>().SetScore(40 + i);
								break;
							case ITEM_TYPE.ITEM_DIAMOND:
								scoredTail.GetComponent<BS_Item>().SetScore(50 + i);
								break;
							case ITEM_TYPE.ITEM_SPEED:
							case ITEM_TYPE.ITEM_MAGNET:
							case ITEM_TYPE.ITEM_BONUS:
								scoredTail.GetComponent<BS_Item>().SetScore(0 + i);
								break;
							}
						}

						this.transform.GetComponentInParent<BS_InGameManager>().character[scoredPlayer].GetComponent<BS_Character>().tails.RemoveAt(0);
						scoredTail.GetComponent<Rigidbody>().Sleep();

						scoredTail.layer = LayerMask.NameToLayer("ScoredItem");

						scoredTails.Add(scoredTail.gameObject);
					}

					other.gameObject.GetComponent<BS_Character>().SellingStart(respawnPosition);
				}
			}
		}

		if(other.gameObject.layer == LayerMask.NameToLayer("ScoredItem"))
		{
			scoredTails.RemoveAt(0);
			scoringPosition[other.gameObject.GetComponent<BS_Item>().GetHeadPlayer()].GetComponent<BS_ScoringPosition>().AddTail(other.gameObject);
		}
	}

	void Start ()
	{
		openBoard.transform.localScale = Vector3.zero;
		openBoardOriginalHeight = openBoard.transform.position.y;

		openArrow.SetActive(false);
	}

	void Update ()
	{
		TailsMove();

		arrowSpeed = GetComponentInParent<BS_StoreManager>().GetArrowSpeed();

		if(isActiveStore == true)
		{
			openBoard.transform.position = new Vector3(openBoard.transform.position.x, openBoardOriginalHeight + Mathf.Sin(Mathf.Rad2Deg * Time.frameCount / 500) * 0.3f ,openBoard.transform.position.z);
			openArrow.GetComponent<Animator>().speed = arrowSpeed;
		}
		else
		{
			blinkStartFrame = Time.frameCount;
		}
	}

	void TailsMove()
	{
		for(int i = 0; i < scoredTails.Count; i++)
		{
			GameObject preTail;
			GameObject thisTail;
			
			if(i == 0 || scoredTails[i - 1] == null)
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

	public void SetActiveStore(bool on)
	{
		isActiveStore = on;

		if(on == true)
		{
			iTween.ScaleTo(openBoard.gameObject, Vector3.one * 3.0f, 0.5f);
			openArrow.SetActive(true);

			gate.SetActive(false);
		}
		else
		{
			iTween.ScaleTo(openBoard.gameObject, Vector3.zero, 0.5f);
			openArrow.SetActive(false);
			arrowSpeed = 0f;

			gate.SetActive(true);
		}
	}
}
