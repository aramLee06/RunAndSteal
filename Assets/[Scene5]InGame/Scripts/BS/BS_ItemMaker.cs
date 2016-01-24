using UnityEngine;
using System.Collections;

public class BS_ItemMaker : MonoBehaviour
{
	public int maxItems = 10;
	public float itemMakeTerm = 1.0f;

	public int maxItemsFever = 20;
	public float itemMakeTermFever = 0.5f;

	public GameObject[] item = new GameObject[5];

	private int itemCount = 0;

	public GameObject inGameManager = null;

	void Start ()
	{
		StartCoroutine(MakeItem());
	}

	void Update ()
	{
		if(inGameManager.GetComponent<BS_InGameManager>().IsFeverTime() == true)
		{
			maxItems = maxItemsFever;
			itemMakeTerm = itemMakeTermFever;
		}

		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
		itemCount = 0;
		foreach (GameObject item in items)
		{
			if(item.GetComponent<BS_Item>().IsTail() == false)
			{
				itemCount++;
			}
		}
	}

	IEnumerator MakeItem()
	{
		yield return new WaitForSeconds(itemMakeTerm);

		if (itemCount < maxItems)
		{
			GameObject itemClone = null;

			ITEM_TYPE itemType = ITEM_TYPE.ITEM_NONE;
			switch(Random.Range(0, 15))
			{
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
				itemType = ITEM_TYPE.ITEM_APPLE;
				break;
			case 5:
			case 6:
			case 7:
			case 8:
				itemType = ITEM_TYPE.ITEM_SILVER;
				break;
			case 9:
			case 10:
			case 11:
				itemType = ITEM_TYPE.ITEM_GOLD;
				break;
			case 12:
			case 13:
				itemType = ITEM_TYPE.ITEM_RING;
				break;
			case 14:
				itemType = ITEM_TYPE.ITEM_DIAMOND;
				break;
			}

			itemClone = Instantiate(item[(int)itemType]) as GameObject;
			itemClone.transform.parent = this.transform.parent;
			itemClone.transform.localPosition = new Vector3(Random.Range(-17.0f, 17.0f), 1.5f, Random.Range(-26.0f, 26.0f));
		}

		StartCoroutine(MakeItem());
	}
}
