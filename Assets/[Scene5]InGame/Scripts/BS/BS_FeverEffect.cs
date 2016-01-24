using UnityEngine;
using System.Collections;

public class BS_FeverEffect : MonoBehaviour
{
	public GameObject[] feverEffect = new GameObject[10];

	void Start ()
	{
		for(int i = 0; i < feverEffect.Length; i++)
		{
			feverEffect[i].GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-5.0f, 5.0f), Random.Range(2.0f, 5.0f));
		}
	}
}
