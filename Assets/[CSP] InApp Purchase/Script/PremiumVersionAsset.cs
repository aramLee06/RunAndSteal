using UnityEngine;
using System.Collections;
using Soomla.Store;


public class PremiumVersionAsset : IStoreAssets {

	// TODO : 향후 실제 InApp Production ID로 교체
	public static string PREMIUM_VERSION_PRODUCT_ID = "premium_version"; 
	//public static string PREMIUM_VERSION_PRODUCT_ID = "android.test.purchased";

	public int GetVersion() {
		return 2;
	}

	public VirtualCurrency[] GetCurrencies()
	{
		return new VirtualCurrency[] { };
	}

	public VirtualGood[] GetGoods()
	{
		return new VirtualGood[] { PremiumPack };
	}

	public VirtualCurrencyPack[] GetCurrencyPacks()
	{
		return new VirtualCurrencyPack[] { };
	}

	public VirtualCategory[] GetCategories()
	{
		return new VirtualCategory[] { };
	}


	///** Virtual Goods **/

	public static VirtualGood PremiumPack = new LifetimeVG(
		"pre_ver",       // name
		"게임에 접속할 수 있는 유저의 수를 4명까지 확장합니다.",       // description
		PREMIUM_VERSION_PRODUCT_ID,      // item id
		//new PurchaseWithMarket(CASH_PACK01_ID, 0.99));
		new PurchaseWithMarket(PREMIUM_VERSION_PRODUCT_ID, 0.99));
}
