using UnityEngine;
using System.Collections;

public class BS_ItemBox : MonoBehaviour
{
	public GameObject[] item = new GameObject[5];
	public GameObject[] specialItem = new GameObject[3];

	public GameObject boxShadow = null;

	private float boxMakeTime = 2.0f;
	private float boxBombTime = 7.0f;

	public GameObject explosionSmoke = null;

	public AudioClip boxMakeSound = null;
	public AudioClip boxExplosionSound = null;

	void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Floor") && this.transform.localScale.magnitude > 0 && this.gameObject.activeSelf == true)
		{
			Camera.main.GetComponent<AudioSource>().PlayOneShot(boxMakeSound);
		}
	}

	void Start ()
	{
		boxShadow.SetActive(false);
		this.gameObject.SetActive(false);
	}

	void Update ()
	{
		boxShadow.transform.position = new Vector3(this.transform.position.x, 0.1f, this.transform.position.z);
	}

	public void MakeItemBox()
	{
		this.gameObject.SetActive(true);
		this.gameObject.transform.localScale = Vector3.zero;

		StartCoroutine(BoxProcess());
	}

	IEnumerator BoxProcess()
	{
		yield return new WaitForSeconds(boxMakeTime);

		this.gameObject.transform.localScale = Vector3.one;
		iTween.PunchScale(this.gameObject, new Vector3(1.5f, 1.5f, 1.5f), 1.5f);

		boxShadow.SetActive(true);

		yield return new WaitForSeconds(boxBombTime);

		this.gameObject.SetActive(false);

		GameObject itemClone = null;
		GameObject explosionSmokeClone = null;

		switch(Random.Range(0, 10))
		{
		case 0:
			itemClone = Instantiate(item[(int)ITEM_TYPE.ITEM_APPLE]) as GameObject;
			break;
		case 1:
			itemClone = Instantiate(item[(int)ITEM_TYPE.ITEM_SILVER]) as GameObject;
			break;
		case 2:
			itemClone = Instantiate(item[(int)ITEM_TYPE.ITEM_GOLD]) as GameObject;
			break;
		case 3:
			itemClone = Instantiate(item[(int)ITEM_TYPE.ITEM_RING]) as GameObject;
			break;
		case 4:
			itemClone = Instantiate(item[(int)ITEM_TYPE.ITEM_DIAMOND]) as GameObject;
			break;
		case 5:
			itemClone = Instantiate(specialItem[(int)ITEM_TYPE.ITEM_SPEED - 10]) as GameObject;
			break;
		case 6:
			itemClone = Instantiate(specialItem[(int)ITEM_TYPE.ITEM_MAGNET - 10]) as GameObject;
			break;
		case 7:
			itemClone = Instantiate(specialItem[(int)ITEM_TYPE.ITEM_BONUS - 10]) as GameObject;
			break;
		case 8:
		case 9:
			break;
		}

		if(itemClone != null)
		{
			itemClone.transform.position = this.transform.position;
			itemClone.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-5.0f, 5.0f), 10.0f, Random.Range(-5.0f, 5.0f));
		}

		if(QualitySettings.GetQualityLevel() != (int)QualityLevel.Fastest)
		{
			explosionSmokeClone = Instantiate(explosionSmoke) as GameObject;
			explosionSmokeClone.transform.position = this.transform.position;
		}

		Camera.main.GetComponent<AudioSource>().PlayOneShot(boxExplosionSound);

		boxShadow.SetActive(false);
	}
}
