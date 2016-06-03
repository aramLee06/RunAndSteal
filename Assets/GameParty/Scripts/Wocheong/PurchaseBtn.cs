using UnityEngine;
using UnityEngine.UI;
using UXLib;
using System.Collections;

public class PurchaseBtn : MonoBehaviour {

	private UXClientController cc;
	// Use this for initialization
	void Start () {
		cc = UXClientController.Instance;	
		this.gameObject.GetComponent<Button> ().onClick.AddListener (delegate {
			cc.SendDataToHost("TimeLimitPurchase,");	
		});
	}
}
