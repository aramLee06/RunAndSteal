using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QROnOff_ras : MonoBehaviour
{
    public GameObject button_QR;
    Image black;
    int index;

    LobbyClient clientLobby = null;
    bool qrOn = true;
    public void Start()
    {
    }

    void Update()
    {
        if (clientLobby == null)
            clientLobby = GameObject.Find("LobbyClient").GetComponent<LobbyClient>();
        else
            GetPlayerIndex(clientLobby.GetPlayerID());
    }

    public void GetPlayerIndex(int id)
    {
        index = id;

        //Debug.Log("index:" + index);
        if (index == 0)
            button_QR.SetActive(true);
        else
            button_QR.SetActive(false);
    }

    public void SetQROnOff()
    {
		Debug.Log ("QR BTN");
        if (qrOn)
        {
            clientLobby.SendAll("QROff");
            qrOn = false;
        }
        else
        {
            clientLobby.SendAll("QROn");
            qrOn = true;
        }
    }

    public void SetQROnOff(bool value)
    {
        qrOn = value;
    }

}
