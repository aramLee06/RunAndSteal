using UnityEngine;
using System.Collections;

public class BS_Map : MonoBehaviour
{
	public Texture[] nightTexture = new Texture[6];

	public Light mapLight = null;

	public void ChangeNightTexture()
	{
		for(int i = 0; i < this.GetComponent<Renderer>().materials.Length; i++)
		{
			this.GetComponent<Renderer>().materials[i].SetTexture(0, nightTexture[i]);
		}

		mapLight.intensity = 0.5f;
	}
}
