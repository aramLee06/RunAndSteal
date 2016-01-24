using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Runtime;
using Soomla.Store;

public class PremiumVersionStore : MonoBehaviour {
	/// <summary>
	/// the id of requested product id
	/// </summary>
	private string buyID = PremiumVersionAsset.PREMIUM_VERSION_PRODUCT_ID;

	/// <summary>
	/// The on request. if request purchase data, will be true.
	/// </summary>
	private bool onRequest = false;

	private PremiumVersionStore instance;

	public delegate void OnPurchaseSuccessHandler ();
	public delegate void OnPurchaseFailedHandler ();

	public event OnPurchaseSuccessHandler OnPurchaseSuccess;
	public event OnPurchaseFailedHandler OnPurchaseFailed;

	public GameObject PurchaseButton;

	void Awake(){
		if(instance == null){ 	//making sure we only initialize one instance.
			instance = this;
			GameObject.DontDestroyOnLoad(this.gameObject);
		} else {					//Destroying unused instances.
			GameObject.Destroy(this);
		}
	}
		
	IEnumerator Start () {
		// Bind Event
		StoreEvents.OnSoomlaStoreInitialized += OnSoomlaStoreInitialized;
		StoreEvents.OnMarketPurchase += OnMarketPurchase;
		StoreEvents.OnMarketPurchaseCancelled += OnMarketPurchaseCancelled;

		if(SoomlaStore.Initialized == false)
		{
			// Store Initialize
			yield return SoomlaStore.Initialize(new PremiumVersionAsset());
		}
	}

	void Update(){
		if (IsPremiumVersion ()) {
			PurchaseButton.SetActive (false);
		}
	}

	// 씬 전환시에 IAB 서비스를 종료하고 이벤트를 해제한다.
	void OnDestroy()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		SoomlaStore.StopIabServiceInBg();
		#endif

		StoreEvents.OnSoomlaStoreInitialized -= OnSoomlaStoreInitialized;
		StoreEvents.OnMarketPurchase -= OnMarketPurchase;
		StoreEvents.OnMarketPurchaseCancelled -= OnMarketPurchaseCancelled;
	}

	public void RequestBuy()
	{
		if (!onRequest)
		{
			try
			{
				onRequest = true;
				//buyID = productID;
				StoreInventory.BuyItem(buyID);
			}
			catch (Exception e)
			{
				onRequest = false;
				Debug.Log("Exception : " + e.Message);
			}
		}
	}

	// Soomla 초기화후에 IAB 서비스를 개시한다.
	public void OnSoomlaStoreInitialized()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		SoomlaStore.StartIabServiceInBg();
		#endif
	}

	// 아이템 구매가 성공하면 호출되는 이벤트 핸들러.
	public void OnMarketPurchase(PurchasableVirtualItem pvi, string purchaseToken, System.Collections.Generic.Dictionary<string, string> payload)
	{
		Debug.Log("구매 성공 " + buyID);
		onRequest = false;
		OnPurchaseSuccess ();
	}

	// 아이템 구매가 사용자에 의해 또는 무언가의 문제로 취소되면 호출된다.
	public void OnMarketPurchaseCancelled(PurchasableVirtualItem pvi)
	{
		Debug.Log("구매 취소 " + buyID);
		onRequest = false;
		OnPurchaseFailed ();
	}

	/// <summary>
	/// Determines whether this instance is premium version.
	/// </summary>
	/// <returns><c>true</c> if this instance is premium version; otherwise, <c>false</c>.</returns>
	public bool IsPremiumVersion(){
		if (StoreInventory.GetItemBalance (buyID) > 0) { // 프리미엄 버전을 구매 한 경우
			return true;
		} else {
			return false;
		}
	}
}
