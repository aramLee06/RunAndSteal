using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ErrorMessage : MonoBehaviour {

	public static string errorMessage;
	public Text errorText;
	// Use this for initialization
	void Start () {
		errorText.text = errorMessage;	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
