using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using UXLib;

public class Setting_RunandSteal_Client : MonoBehaviour
{
    public enum LangType
    {
        CHINA = 0,
        ENGLISH = 1,
    }

    public static LangType langType;

    public GameObject panel;
    public GameObject chinaLang;
    public GameObject englishLang;

    public Text serverState;
    public Text version;

    RectTransform setting;

    public const string CHINA = "chi";
    public const string ENGLISH = "eng";
    public const string LANGUAGE = "Language";

    bool isOpen = false;

    UXAndroidManager androidManager;


    // Use this for initialization
    void Awake()
    {

#if UNITY_ANDROID && !UNITY_EDITOR
		GameObject go = GameObject.Find ("AndroidManager");
		androidManager = go.GetComponent<UXAndroidManager> ();
		androidManager.InitAndroid();
#endif
    }
    void Start()
    {
        setting = panel.GetComponent<RectTransform>() as RectTransform;

        if (((ServerList)PlayerPrefs.GetInt("ServerList")) == ServerList.CN)
        {
            serverState.text = "China";
        }
        else if (((ServerList)PlayerPrefs.GetInt("ServerList")) == ServerList.SG)
        {
            serverState.text = "Singapore";
        }

        if (PlayerPrefs.GetString(LANGUAGE) == CHINA)
        {
            PlayerPrefs.SetString(LANGUAGE, CHINA);
            chinaLang.SetActive(true);
            englishLang.SetActive(false);
        }
        else
        {
            PlayerPrefs.SetString(LANGUAGE, ENGLISH);
            chinaLang.SetActive(false);
            englishLang.SetActive(true);
        }

        TextUpdate();

#if UNITY_ANDROID && !UNITY_EDITOR
		version.text = "ver " + androidManager.GetVersionName ("com.cspmedia.gamepartyplayer");
#endif
    }

    void Update()
    {
        if (isOpen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Down ");
                PointerEventData pointer = new PointerEventData(EventSystem.current);
                pointer.position = Input.mousePosition;

                List<RaycastResult> raycastResults = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, raycastResults);

                if (raycastResults.Count > 0)
                {

                    GameObject obj = raycastResults[0].gameObject;
                    Debug.Log(obj.transform.parent);

                    Transform[] childObj = gameObject.GetComponentsInChildren<Transform>();

                    int count = 0;

                    foreach (Transform tr in childObj)
                    {
                        if (tr.name == obj.name)
                        {

                            count++;

                            Debug.Log("tr : " + tr.name + " obj : " + obj.name);
                        }
                    }
                    if (count == 0)
                        setting.transform.DOLocalMoveX(635f, 0.5f);
                }
            }
        }
    }

    public void SettingOpen()
    {
        Debug.Log("dddd" + isOpen);
        if (isOpen == true)
        {
            isOpen = false;
            setting.transform.DOLocalMoveX(635f, 0.5f);
            return;
        }
        isOpen = true;
        setting.transform.DOLocalMoveX(85f, 0.5f);
        Debug.Log("hohiohihihihihihihihi");
    }

    public void LangButton()
    {

        if (chinaLang.activeSelf == true)
        {
            PlayerPrefs.SetString(LANGUAGE, ENGLISH);
            //CommonLang.instance.SelectLanguage(PlayerPrefs.GetString(LANGUAGE));
            chinaLang.SetActive(false);
            englishLang.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetString(LANGUAGE, CHINA);
            //CommonLang.instance.SelectLanguage(PlayerPrefs.GetString(LANGUAGE));

            chinaLang.SetActive(true);
            englishLang.SetActive(false);
        }
        TextUpdate();
    }


    private void TextUpdate()
    {
        //...HELL
        switch (PlayerPrefs.GetString(LANGUAGE))
        {
            case "chi":
                langType = LangType.CHINA;
                break;
            case "eng":
                langType = LangType.ENGLISH;
                break;
        }
    }
}
