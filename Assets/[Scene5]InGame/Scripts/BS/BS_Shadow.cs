using UnityEngine;
using System.Collections;

public class BS_Shadow : MonoBehaviour
{
	private Vector3 shadowAngle = Vector3.zero;

	void Start ()
	{
		shadowAngle = this.transform.eulerAngles;
	}

	void Update ()
	{
		this.transform.eulerAngles = shadowAngle;
	}
}
