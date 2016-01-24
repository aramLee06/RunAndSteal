using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class ZoomInScaleBtn : MonoBehaviour 
{
	//public GameObject button;

	void Start () 
	{

	}
	
	void Update () 
	{
	
	}

	public void PressEvent(){
		Debug.Log ("Press:");

		gameObject.transform.DOScale(4.0f,0.5f).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.OutExpo);
		
	}

	public void Paurse(){
		Debug.Log ("UP");
		DOTween.PauseAll();
		gameObject.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
	}

}
