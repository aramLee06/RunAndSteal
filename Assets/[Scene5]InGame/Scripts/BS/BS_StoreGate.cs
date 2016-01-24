using UnityEngine;
using System.Collections;

public class BS_StoreGate : MonoBehaviour
{
	public GameObject respawnPosition = null;

	void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Character"))
		{
			other.transform.position = respawnPosition.transform.position;
		}
	}
}
