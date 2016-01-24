using UnityEngine;
using System.Collections;

public class BS_SelectCharacter : MonoBehaviour
{
	public GameObject[] character = new GameObject[(int)CHARACTER_TYPE.CHARACTER_MAX];
	
	void Start ()
	{
		this.gameObject.SetActive(false);
	}

	public void SetCharacter(int characterType)
	{
		this.gameObject.SetActive(true);
		character[characterType].SetActive(true);
		iTween.ScaleTo(this.gameObject, Vector3.one, 1.0f);
	}
}
