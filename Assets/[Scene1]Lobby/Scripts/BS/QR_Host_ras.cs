using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QR_Host_ras : MonoBehaviour {

	public RawImage qr_player, qr_store;
	public Text roomNumberText;
	LobbyHost hostLobby;

	void Start () {
		hostLobby = GameObject.Find ("LobbyHost").GetComponent<LobbyHost> ();
		StartCoroutine (SetQR ());
	}

	IEnumerator SetQR()
	{
		yield return new WaitForSeconds (0.1f);

        string room_no = hostLobby.roomNumberLabel.text;
        roomNumberText.text = room_no;
		Texture2D qrTexture_store = QRGenerator.EncodeString(room_no, Color.black, Color.white);
		Texture2D qrTexture_player = QRGenerator.EncodeString("http://playgameparty-china-apk.oss-cn-hangzhou.aliyuncs.com/bigscreen/gameparty_player_v0.1.98_cn.apk", Color.black, Color.white);
		
		qr_store.texture = qrTexture_store;
		qr_player.texture = qrTexture_player;
	}

}
