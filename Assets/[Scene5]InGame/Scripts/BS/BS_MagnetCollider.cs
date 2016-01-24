using UnityEngine;
using System.Collections;

public class BS_MagnetCollider : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Item") && other.gameObject.GetComponent<BS_Item>().IsTail() == false)
		{
			this.GetComponentInParent<BS_Character>().MagnetCollision(other);
		}
	}
}
