using UnityEngine;
using System.Collections;

public class PS_InGameManager : MonoBehaviour
{
	private LobbyClient lobbyClient;

	public GameObject playerNumber = null;
	public GameObject controllerLight = null;
	public GameObject controllerDirection = null;
	public GameObject touchLight = null;
	
	public GameObject characterFace = null;
	public GameObject characterName = null;

	private Vector2 stickCoreCenter;

	public Sprite[] playerNumberSprite = new Sprite[6];
	public Sprite[] characterFaceSprite = new Sprite[7];
	public Sprite[] characterNameSprite = new Sprite[7];
	
	private Vector2 touchPos;
	private int touchAngle;
	private int previousAngle;
	private bool sendPacket = false;

	private int directionControllerFingerID = -1;

	private bool isJoystickDown = false;

	private int myScore = 0;
	public UILabel myScoreLabel = null;
	private Color playerColor = Color.white;
	
	public AudioClip diamondSound = null;
	public AudioClip ringSound = null;
	public AudioClip goldSound = null;
	public AudioClip silverSound = null;
	public AudioClip appleSound = null;

	public AudioClip speedSound = null;
	public AudioClip magnetSound = null;
	public AudioClip bonusSound = null;

	public AudioClip sellSound = null;
	public AudioClip specialSellSound = null;

	public AudioClip stealEmojiSound = null;
	public AudioClip angryEmojiSound = null;
	public AudioClip crashEmojiSound = null;

	void OnGUI()
	{
#if DEVELOPMENT_BUILD
		if(Input.touchCount > 0)
		{
			GUI.Label(new Rect(0, 0, 1000, 50), "StickCoreCenter : " + stickCoreCenter);
			GUI.Label(new Rect(0, 50, 1000, 50), "RealTouchPosition : " + Input.GetTouch(0).position);
			GUI.Label(new Rect(0, 100, 1000, 50), "WorldTouchPosition : " + touchPos);
			GUI.Label(new Rect(0, 150, 1000, 50), "Angle : " + touchAngle);
		}
#endif
	}

	void Awake()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep; 
		Screen.orientation = ScreenOrientation.Portrait;
	}

	void Start ()
	{
		lobbyClient = GameObject.Find ("LobbyClient").GetComponent<LobbyClient>();

		playerNumber.GetComponent<SpriteRenderer>().sprite = playerNumberSprite[lobbyClient.GetPlayerID()];

		characterFace.GetComponent<SpriteRenderer>().sprite = characterFaceSprite[(int)lobbyClient.myCharacter];
		characterName.GetComponent<SpriteRenderer>().sprite = characterNameSprite[(int)lobbyClient.myCharacter];

		stickCoreCenter = characterFace.transform.position;

		myScore = 0;
		myScoreLabel.text = "" + 0;
		switch(lobbyClient.GetPlayerID())
		{
		case 0:
			playerColor = new Color(1.0f, 0.08f, 0.08f, 1.0f); // Red
			break;
		case 1:
			playerColor = new Color(1.0f, 0.65f, 0.0f, 1.0f); // Orange
			break;
		case 2:
			playerColor = new Color(1.0f, 0.93f, 0.0f, 1.0f); // Yellow
			break;
		case 3:
			playerColor = new Color(0.27f, 0.95f, 1.0f, 1.0f); // Blue
			break;
		case 4:
			playerColor = new Color(0.85f, 0.19f, 1.0f, 1.0f); // Violet
			break;
		case 5:
			playerColor = new Color(1.0f, 1.0f, 1.0f, 1.0f); // White
			break;
		}
		myScoreLabel.color = playerColor;
	}

	void Update ()
	{
		sendPacket = !sendPacket;

	    if(Input.touchCount > 0)
		{
			for(int i = 0; i < Input.touchCount; i++)
			{
				Vector3 touch = Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position);
				RaycastHit2D hit = Physics2D.Raycast(touch, Vector3.zero);
				int fingerID = Input.GetTouch(i).fingerId;

				if(hit.transform != null)
				{
					touchPos = hit.point;

					if(Input.GetTouch(i).phase == TouchPhase.Began)
					{
						directionControllerFingerID = fingerID;

						Vector2 variationVector = touchPos - stickCoreCenter;
                        touchAngle = (int)(Mathf.Atan2(variationVector.x, variationVector.y) * Mathf.Rad2Deg / 4) * 4;

						controllerLight.SetActive(true);

						controllerDirection.SetActive(true);
						controllerDirection.transform.rotation = Quaternion.AngleAxis(touchAngle, -Vector3.forward);

						touchLight.SetActive(true);
						touchLight.transform.position = touchPos;

						if(touchAngle != previousAngle && sendPacket)
						{
							lobbyClient.SendToHost("JoystickDown," + touchAngle);
							previousAngle = touchAngle;
						}
						else if(isJoystickDown == false && sendPacket)
						{
                            lobbyClient.SendToHost("JoystickDown," + touchAngle);
						}
						
						isJoystickDown = true;
					}
					else if(Input.GetTouch(i).phase == TouchPhase.Moved && directionControllerFingerID == fingerID && directionControllerFingerID != -1)
					{
						Vector2 variationVector = touchPos - stickCoreCenter;
                        touchAngle = (int)(Mathf.Atan2(variationVector.x, variationVector.y) * Mathf.Rad2Deg / 4) * 4;

						controllerLight.SetActive(true);

						controllerDirection.SetActive(true);
						controllerDirection.transform.rotation = Quaternion.AngleAxis(touchAngle, -Vector3.forward);

						touchLight.SetActive(true);
						touchLight.transform.position = touchPos;

						if(touchAngle != previousAngle && sendPacket)
						{
                            lobbyClient.SendToHost("JoystickDown," + touchAngle);
							previousAngle = touchAngle;
						}
						else if(isJoystickDown == false && sendPacket)
						{
                            lobbyClient.SendToHost("JoystickDown," + touchAngle);
						}
						
						isJoystickDown = true;
					}
					else if(Input.GetTouch(i).phase == TouchPhase.Ended && directionControllerFingerID == fingerID && directionControllerFingerID != -1)
					{
						isJoystickDown = false;

						controllerLight.SetActive(false);
						controllerDirection.SetActive(false);
						touchLight.SetActive(false);

						directionControllerFingerID = -1;

						lobbyClient.SendToHost("JoystickUp");
					}
				}
			}
		}
		else
		{
			controllerLight.SetActive(false);
			controllerDirection.SetActive(false);
			touchLight.SetActive(false);
		}

		myScoreLabel.text = "" + myScore;
	}

	public void PlayDiamondSound(int score)
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(diamondSound);
		myScore = score;
	}

	public void PlayRingSound(int score)
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(ringSound);
		myScore = score;
	}

	public void PlayGoldSound(int score)
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(goldSound);
		myScore = score;
	}

	public void PlaySilverSound(int score)
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(silverSound);
		myScore = score;
	}

	public void PlayAppleSound(int score)
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(appleSound);
		myScore = score;
	}

	public void PlaySellSound(int score)
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(sellSound);
		myScore = score;
	}

	public void PlaySpecialSellSound(int score)
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(specialSellSound);
		myScore = score;
	}

	public void PlaySpeedSound(int score)
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(speedSound);
		myScore = score;
	}
	
	public void PlayMagnetSound(int score)
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(magnetSound);
		myScore = score;
	}
	
	public void PlayBonusSound(int score)
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(bonusSound);
		myScore = score;
	}

	public void PlayStealEmojiSound()
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(stealEmojiSound);
	}

	public void PlayAngryEmojiSound()
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(angryEmojiSound);
#if UNITY_ANDROID
		Handheld.Vibrate();
#endif
	}

	public void PlayCrashEmojiSound()
	{
		Camera.main.GetComponent<AudioSource>().PlayOneShot(crashEmojiSound);
#if UNITY_ANDROID
		Handheld.Vibrate();
#endif
	}
}
