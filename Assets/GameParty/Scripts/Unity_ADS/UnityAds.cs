using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class UnityAds : MonoBehaviour
{
	public string zoneId = "video";
	public static UnityAds instance;

	void Awake ()
	{
		if (instance == null) {
			instance = this; 
		} else {
			
		}
	}

	public void ShowAdPlacement ()
	{
		if (string.IsNullOrEmpty (zoneId)) 
		{
			zoneId = "video";
		}

		ShowOptions options = new ShowOptions();
		options.resultCallback = HandleShowResult;

		Advertisement.Show (zoneId, options);
	}

	private void HandleShowResult (ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished: // 광고 시청 완료. -> 보상제공... 안할거야^^!
			Debug.Log ("Video completed. Offer a reward to the player.");
			break;
		case ShowResult.Skipped:
			Debug.LogWarning ("Video was skipped.");
			break;
		case ShowResult.Failed:
			Debug.LogError ("Video failed to show.");
			break;
		}
	}
}
