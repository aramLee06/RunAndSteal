using UnityEngine;
using System.Collections;

public class BS_Car : MonoBehaviour
{
	void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Character"))
		{
			other.gameObject.GetComponent<BS_Character>().CarCrash();
		}
	}

	void Update()
	{
		this.transform.eulerAngles = new Vector3(0, this.transform.eulerAngles.y, 0);
	}

	public void MoveCar(float moveTime, string path)
	{
		iTween.MoveTo(this.gameObject, iTween.Hash("path", iTweenPath.GetPath(path),
		                                           "orienttopath", true,
		                                           "lookTime", 0.2f,
		                                           "easetype", iTween.EaseType.linear,
		                                           "time", moveTime));
	}
}
