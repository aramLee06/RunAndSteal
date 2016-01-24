using UnityEngine;
using System.Collections;

public enum ITEM_TYPE
{
	ITEM_NONE = -1,

	ITEM_APPLE = 0,
	ITEM_SILVER,
	ITEM_GOLD,
	ITEM_RING,
	ITEM_DIAMOND,

	ITEM_SPEED = 10,
	ITEM_MAGNET,
	ITEM_BONUS,

	ITEM_MAX
}

public class BS_Item : MonoBehaviour
{
	public ITEM_TYPE itemType = ITEM_TYPE.ITEM_NONE;
	private bool isTail = false;
	private int headPlayer = -1;
	private int tailIndex = -1;
	private int tailLength = 0;

	private bool isScored = false;
	private int itemScore = 0;

	public GameObject itemShadow = null;

	void Start ()
	{
		iTween.PunchScale(this.gameObject, new Vector3(1.5f, 1.5f, 1.5f), 1.5f);
	}

	void Update ()
	{
		itemShadow.transform.position = new Vector3(this.transform.position.x, 0 ,this.transform.position.z);

		if(isTail == true)
		{
			if(tailIndex > tailLength)
			{
				isTail = false;
				tailLength = tailIndex - 1;
			}
		}
		else
		{
			this.transform.Rotate(Vector3.up * 180.0f * Time.deltaTime);
		}
	}

	public void SetTail(int player, int index, int length)
	{
		isTail = true;
		headPlayer = player;
		tailIndex = index;
		tailLength = length;
	}

	public void CutTail()
	{
		isTail = false;
		headPlayer = -1;
		tailIndex = -1;
	}

	public bool IsTail()
	{
		return isTail;
	}

	public int GetHeadPlayer()
	{
		return headPlayer;
	}

	public int GetTailIndex()
	{
		return tailIndex;
	}

	public void SetScore(int score)
	{
		isScored = true;
		itemScore = score;

		itemShadow.SetActive(false);
	}

	public bool IsScored()
	{
		return isScored;
	}

	public int GetScore()
	{
		return itemScore;
	}

	public ITEM_TYPE GetItemType()
	{
		return itemType;
	}
}