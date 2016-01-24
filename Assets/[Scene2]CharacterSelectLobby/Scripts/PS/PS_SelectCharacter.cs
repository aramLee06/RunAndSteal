using UnityEngine;
using System.Collections;

public class PS_SelectCharacter : MonoBehaviour
{
	public GameObject characterNameTag = null;
	public GameObject soldOutStamp = null;
	public GameObject grayCharacter = null;

	private Vector2 colliderSize;

	private bool isSoldOut = false;

	void Start ()
	{
		colliderSize = this.GetComponent<BoxCollider>().size;
		soldOutStamp.SetActive(false);
	}

	void Update ()
	{
		this.transform.localScale = Vector2.one - ((Vector2.one * Mathf.Abs(this.transform.position.x)) * 0.8f);
		this.GetComponent<BoxCollider>().size = colliderSize / this.transform.localScale.x;

		characterNameTag.transform.localScale = Vector2.one - ((Vector2.one * Mathf.Abs(this.transform.position.x)) * 2.0f);
	
		if(isSoldOut == true)
		{
			grayCharacter.SetActive(true);
			soldOutStamp.SetActive(true);
		}
		else
		{
			grayCharacter.SetActive(false);
			soldOutStamp.SetActive(false);
		}
	}

	public void SetSoldOut()
	{
		isSoldOut = true;
	}

	public bool IsSoldOut()
	{
		return isSoldOut;
	}
}
