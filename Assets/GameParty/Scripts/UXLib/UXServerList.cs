using UnityEngine;
using System.Collections;
using UXLib;


public class UXServerList : MonoBehaviour {
		
	private int 				count;

	public GameObject			list;
	float 						_time;

	public string				FirstSceneName;


	void Start () {
		#if GOOGLE
		this.gameObject.SetActive (true);
		#else
		this.gameObject.SetActive (false);
		#endif
		list.SetActive(false);
	}

	public void SaveSeverList(int server){
		PlayerPrefs.SetInt("ServerList",server);
		UXConnectController.ServerCheck((ServerList)server);
		GameObject[] obj = FindObjectsOfType(typeof(GameObject)) as GameObject[]; 
		for(int i = 0 ; i < obj.Length; i++){
			Destroy(obj[i]);
		}
		System.GC.Collect();
		Application.LoadLevel(FirstSceneName);
	}

	void Update(){

		if(Input.GetKey(KeyCode.Escape) || Input.GetMouseButton(0))
		{
			_time += Time.deltaTime;

			if(_time >= 7.0f)
			{
				list.SetActive(true);
			}
		}
		else if(Input.GetKeyUp(KeyCode.Escape) || Input.GetMouseButtonUp(0))
		{
			_time = 0;
		}
	}


}

public enum ServerList
{
	CN = 0,
	SG = 1,
}
