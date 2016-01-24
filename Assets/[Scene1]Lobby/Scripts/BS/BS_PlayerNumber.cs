using UnityEngine;
using System.Collections;

public class BS_PlayerNumber : MonoBehaviour
{
	void Start ()
	{
		iTween.PunchScale(this.gameObject, Vector3.one, 1.5f);
	}
}
