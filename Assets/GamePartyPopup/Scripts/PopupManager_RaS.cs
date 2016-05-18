using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/*
 * 1. PopupManager의 Prefeb은 각 Host, Client의 최초 진입 Scene에 넣는다.
 * 
 * 2. PopupManager.Instance.OpenPopup(POPUP_TYPE.POPUP_HOSTDISCONNECTED); 를 Host, Client Controller의 'OnDisconnected' 이벤트에서 호출시킨다.
 * 
 * 3. POPUP_GAMESCLOSE 팝업의 경우엔 알아서 호출된다.
 * 
 * 4. 103번째 줄의 SendCloseMessageToHost()함수 내용 작성해주세요.
 */
public class PopupManager_RaS : MonoBehaviour
{
    private static bool isFreeSetted = false;
    public static bool IsFree { get; private set; }
    public static void IsFreeSetter(bool value)
    {
        if (isFreeSetted)
            return;
        isFreeSetted = true;

        IsFree = value;
    }
    [SerializeField]
    private GameObject popupPanel;
    [SerializeField]
    private GameObject[] popup;
    [SerializeField]
    private GameObject popupEventSystem;
    [SerializeField]
    private Text autoExitText;

    public static PopupManager_RaS Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ClosePopup();
            DontDestroyOnLoad(this.gameObject);
            return;
        }
        else
        {
            GameObject.Destroy(this.gameObject);
            return;
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            OpenPopup(POPUP_TYPE_RaS.POPUP_GAMESCLOSE);
    }


    public void OpenPopup(POPUP_TYPE_RaS popupType)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		Handheld.Vibrate();
#endif
        popupPanel.SetActive(true);

        popup[(int)popupType].SetActive(true);
        popupEventSystem.SetActive(true);

        if (popupType == POPUP_TYPE_RaS.POPUP_HOSTDISCONNECTED)
            StartCoroutine(AutoExitCoru());
		
    }




	IEnumerator AutoExitCoru()
	{
		for (int i = 10; i > 0; i--)
		{
			yield return new WaitForSeconds(1f);
			string tempTxt = autoExitText.text;
			autoExitText.text = tempTxt.Replace (i.ToString (), (i - 1).ToString ());
		}
		CloseGame();
		yield return null;
	}


    #region Button Callbacks
    public void ClosePopup()
    {
        popupPanel.SetActive(false);

        popupEventSystem.SetActive(false);

        int popupCount = popup.Length;
        for (int i = 0; i < popupCount; i++)
            popup[i].SetActive(false);
    }


    public void CloseGame()
    {
        StopCoroutine(AutoExitCoru());
        SendCloseMessageToHost();

        ClosePopup();
        Debug.Log("1");

        CloseGameAndClear();
    }

    public static void CloseGameAndClear()
    {
        if (PopupManager_RaS.Instance == null)
            return;

        GameObject.Destroy(PopupManager_RaS.Instance.gameObject);
        PopupManager_RaS.Instance = null;

        GameObject hostGO = GameObject.Find("LobbyClient");

        if (PopupManager_RaS.IsFree)
        {
            Debug.Log("A");
            Debug.Break();
            Application.Quit();
        }
        else
        {
            if (hostGO != null)
                hostGO.GetComponent<LobbyClient>().Clear();

			Application.Quit ();
        }
    }
    #endregion
	
	
	void SendCloseMessageToHost()
    {
        //Client가 Host에게 모두 종료키시라고 메세지 보내주세용.
        //Host에서도 처리하여, 모든 Client에게 종료되라고 메세지 보내야합니당.
        GameObject hostGO = GameObject.Find("LobbyClient");
        if (hostGO != null)
            hostGO.GetComponent<LobbyClient>().SendAll("Exit");

        GameObject clientGO = GameObject.Find("LobbyHost");
        if (clientGO != null)
            clientGO.GetComponent<LobbyHost>().SendAll("Exit");
	}
}

public enum POPUP_TYPE_RaS
{
	POPUP_HOSTDISCONNECTED = 0,
	POPUP_GAMESCLOSE = 1,
}