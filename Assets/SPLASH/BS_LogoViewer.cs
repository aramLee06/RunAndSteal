using UnityEngine;
using System.Collections;

public enum BUILDTYPE
{
    DEFAULT, MHT, TOGIC, TAIWAN,
}
public class BS_LogoViewer : MonoBehaviour
{
    [SerializeField]
    private GameObject gamePartyLogo;
    [SerializeField]
    private GameObject modanLogo;
    [SerializeField]
    private BUILDTYPE buildType;
    [SerializeField]
    private string moveSceneName;

    public static BUILDTYPE BuildType { get; private set; }

    void Awake()
    {
        gamePartyLogo.SetActive(false);
        modanLogo.SetActive(false);
    }

    IEnumerator Start()
    {
        BuildType = buildType;
        Debug.Log("FIFIFIFIFIFIFIFI" + BuildType.ToString());

        switch (buildType)
        {
            case BUILDTYPE.DEFAULT:
                //UXLib.UXConnectController.ROOM_SERVER_PORT = 4000;
                //UXLib.UXConnectController.REST_SERVER_PORT = 3000;
                break;
            case BUILDTYPE.MHT:
            case BUILDTYPE.TOGIC:
            case BUILDTYPE.TAIWAN:
                //UXLib.UXConnectController.ROOM_SERVER_PORT = 4000;
                //UXLib.UXConnectController.REST_SERVER_PORT = 3000;
                break;
        }

        switch (buildType)
        {
            case BUILDTYPE.DEFAULT:
                gamePartyLogo.SetActive(true);
                break;
            case BUILDTYPE.MHT:
                modanLogo.SetActive(true);
                break;
            case BUILDTYPE.TOGIC:
                gamePartyLogo.SetActive(true);
                break;
            case BUILDTYPE.TAIWAN:
                gamePartyLogo.SetActive(true);
                break;
        }

        yield return new WaitForSeconds(3.0f);

        Application.LoadLevel(moveSceneName);
        GameObject.Destroy(this);
        yield return null;
    }


}
