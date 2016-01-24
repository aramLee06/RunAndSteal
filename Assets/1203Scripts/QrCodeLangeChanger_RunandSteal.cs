using UnityEngine;
using System.Collections;

public class QrCodeLangeChanger_RunandSteal : MonoBehaviour
{
    enum QRDescType
    {
        Player = 0,
        Store = 1,
    }

    [SerializeField]
    private QRDescType qrDescType;

    private UnityEngine.UI.Text text;
    private Setting_RunandSteal_Host.LangType nowLang = Setting_RunandSteal_Host.LangType.ENGLISH;

    void Start()
    {
        text = GetComponent<UnityEngine.UI.Text>();

        nowLang = Setting_RunandSteal_Host.langType;
        UpdateText();
    }

    void Update()
    {
        nowLang = Setting_RunandSteal_Host.langType;
        UpdateText();
    }

    void UpdateText()
    {
        switch (nowLang)
        {
            case Setting_RunandSteal_Host.LangType.CHINA:
                if (qrDescType == QRDescType.Store)
                    text.text = "请输入房间号码还是拍QR码";
                else if (qrDescType == QRDescType.Player)
                    text.text = "请下载Game party player";

                break;
            case Setting_RunandSteal_Host.LangType.ENGLISH:
                if (qrDescType == QRDescType.Store)
                    text.text = "Enter your room number or Scan QR code";
                else if (qrDescType == QRDescType.Player)
                    text.text = "Please download the GamePartyPlayer";

                break;
        }
    }
}
