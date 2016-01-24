using UnityEngine;
using System.Collections;

public class BS_StoreManager : MonoBehaviour
{
	public float activeStoreOnTime = 10.0f;
	public float activeStoreOffTime = 2.0f;

	private int activeStore1 = -1;
	private int activeStore2 = -1;

	public GameObject[] store = new GameObject[4];

	public GameObject carManager = null;

	public AudioClip storeOpenSound = null;

	private float arrowSpeed = 1.0f;

	void Start ()
	{
		activeStore1 = -1;
		activeStore2 = -1;

		StartCoroutine(ActiveZoneOn());
	}

	void Update ()
	{
		if(activeStore1 == -1)
		{
			for(int i = 0; i < store.Length; i++)
			{
				store[i].GetComponent<BS_Store>().SetActiveStore(false);
			}
		}
		else
		{
			store[activeStore1].GetComponent<BS_Store>().SetActiveStore(true);
		}

		if(activeStore2 == -1)
		{
			for(int i = 0; i < store.Length; i++)
			{
				store[i].GetComponent<BS_Store>().SetActiveStore(false);
			}
		}
		else
		{
			store[activeStore2].GetComponent<BS_Store>().SetActiveStore(true);
		}
	}

	IEnumerator ActiveZoneOn()
	{
		ActiveZoneSet();

		yield return new WaitForSeconds(activeStoreOnTime);

		StartCoroutine(ActiveZoneOff());
	}

	IEnumerator ActiveZoneOff()
	{
		activeStore1 = -1;
		activeStore2 = -1;

		yield return new WaitForSeconds(activeStoreOffTime);

		StartCoroutine(ActiveZoneOn());
	}

	void ActiveZoneSet()
	{
		activeStore1 = Random.Range(0, 4);
		activeStore2 = Random.Range(0, 4);

		while(activeStore1 == activeStore2)
		{
			activeStore2 = Random.Range(0, 4);
		}

		Camera.main.GetComponent<AudioSource>().PlayOneShot(storeOpenSound);

		carManager.GetComponent<BS_CarManager>().CarStart((DIRECTION)activeStore1, (DIRECTION)activeStore2);

		arrowSpeed = 0;
		iTween.ValueTo (this.gameObject, iTween.Hash ("from", 0,
		                                              "to", 2.5f,
		                                              "time", activeStoreOnTime,
		                                              "easetype", iTween.EaseType.easeInQuad,
		                                              "onupdate", "UpdateArrowSpeed"));
	}

	void UpdateArrowSpeed(float size)
	{
		arrowSpeed = size;
	}

	public float GetArrowSpeed()
	{
		return arrowSpeed;
	}
}
