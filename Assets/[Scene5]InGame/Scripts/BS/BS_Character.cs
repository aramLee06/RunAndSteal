using UnityEngine;
using System.Collections;

public enum CHARACTER_TYPE
{
	CHARACTER_NONE = -1,
	CHARACTER_ANDY = 0,
	CHARACTER_BUNNEE,
	CHARACTER_FRANK,
	CHARACTER_JEAN,
	CHARACTER_JOHNNY,
	CHARACTER_JOHNSON,
	CHARACTER_ROONY,
	CHARACTER_MAX,
	CHARACTER_DISCONNECTED = 100
}

public class BS_Character : MonoBehaviour
{
	private LobbyHost lobbyHost;

	public int playerNumber;

	private int characterType = (int)CHARACTER_TYPE.CHARACTER_NONE;
	public GameObject[] character = new GameObject[7];

	public GameObject scoreHUD = null;

	private int playerScore = 0;

	public ArrayList tails = new ArrayList();
	
	private int turnAngle = 4;
	private float moveSpeed = 10.0f;
	private float bondDistance = 1.5f;
	private float bondDamping = 10.0f;
	
	public GameObject moveDirectionArrow = null;

	private bool isJoystickInput = false;
	private bool isMoving = false;
	private int previousAngle;
	private int lerpAngle;
	private int finalAngle;

	private float maxVelocity = 6.0f;
	private float minVelocity = 0;
	private float currentVelocity = 0;

	private bool isSpeedItem = false;
	private bool isMagnetItem = false;
	private bool isBonusItem = false;

	public GameObject speedEffect = null;

	public GameObject magnetCollider = null;
	public GameObject magnetEffect = null;

	public GameObject bonusEffect = null;

	public GameObject confusionEmozi = null;
	public GameObject angryEmozi = null;
	public GameObject laughEmozi = null;

	private bool isStunned = false;

	void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.layer == LayerMask.NameToLayer("Item"))
		{
			if(other.gameObject.GetComponent<BS_Item>().IsTail() == false && isStunned == false) // Get items
			{
				tails.Add(other.gameObject);
				int index = tails.Count - 1;
				other.gameObject.GetComponent<BS_Item>().SetTail(playerNumber, index, tails.Count);

				Color playerColor = new Color(0, 0, 0);
				switch(playerNumber)
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
					playerColor = new Color(0.0f, 0.0f, 0.0f, 1.0f); // Black
					break;
				}

				if(isBonusItem == true)
				{
					BS_ScoreManager.Instance().PlusScore(playerNumber, 10);
					scoreHUD.gameObject.GetComponent<HUDText>().AddLocalized("10", playerColor, 0);

					GameObject bonusEffectClone = Instantiate(bonusEffect) as GameObject;
					bonusEffectClone.transform.position = other.transform.position;
					bonusEffectClone.transform.position += new Vector3(0, 1.0f, 0);
				}
				else
				{
					BS_ScoreManager.Instance().PlusScore(playerNumber, 5);
					scoreHUD.gameObject.GetComponent<HUDText>().AddLocalized("5", playerColor, 0);
				}

				int myScore = BS_ScoreManager.Instance().GetPlayerScore(playerNumber);
				switch(other.gameObject.GetComponent<BS_Item>().GetItemType())
				{
				case ITEM_TYPE.ITEM_APPLE:
					lobbyHost.SendTo(playerNumber, "AppleSound," + myScore);
					break;
				case ITEM_TYPE.ITEM_SILVER:
					lobbyHost.SendTo(playerNumber, "SilverSound," + myScore);
					break;
				case ITEM_TYPE.ITEM_GOLD:
					lobbyHost.SendTo(playerNumber, "GoldSound," + myScore);
					break;
				case ITEM_TYPE.ITEM_RING:
					lobbyHost.SendTo(playerNumber, "RingSound," + myScore);
					break;
				case ITEM_TYPE.ITEM_DIAMOND:
					lobbyHost.SendTo(playerNumber, "DiamondSound," + myScore);
					break;
				case ITEM_TYPE.ITEM_SPEED:
					lobbyHost.SendTo(playerNumber, "SpeedSound," + myScore);
					break;
				case ITEM_TYPE.ITEM_MAGNET:
					lobbyHost.SendTo(playerNumber, "MagnetSound," + myScore);
					break;
				case ITEM_TYPE.ITEM_BONUS:
					lobbyHost.SendTo(playerNumber, "BonusSound," + myScore);
					break;
				}
  				
			}
			else // Tail cutting, Take items
			{
				int enemyNumber = other.gameObject.GetComponent<BS_Item>().GetHeadPlayer();
				if(enemyNumber != playerNumber)
				{
					if(enemyNumber < 0) //array index exception - enemyNumber가 -1로 추정됨
						return;

					lobbyHost.SendTo(playerNumber, "StealEmojiSound");
					lobbyHost.SendTo(enemyNumber, "AngryEmojiSound");

					int enemyTailLength = this.transform.GetComponentInParent<BS_InGameManager>().character[enemyNumber].GetComponent<BS_Character>().tails.Count;
					int enemyTailCutPoint = other.gameObject.GetComponent<BS_Item>().GetTailIndex();

					for(int i = enemyTailCutPoint; i < enemyTailLength; i++)
					{
						GameObject cutTail = (GameObject)this.transform.GetComponentInParent<BS_InGameManager>().character[enemyNumber].GetComponent<BS_Character>().tails[enemyTailCutPoint];
						cutTail.GetComponent<BS_Item>().CutTail();
						this.transform.GetComponentInParent<BS_InGameManager>().character[enemyNumber].GetComponent<BS_Character>().tails.RemoveAt(enemyTailCutPoint);
						
						tails.Add(cutTail.gameObject);
						int index = tails.Count - 1;
						cutTail.gameObject.GetComponent<BS_Item>().SetTail(playerNumber, index, tails.Count);
					}

					StartCoroutine(LaughEmozi());
					StartCoroutine(this.transform.GetComponentInParent<BS_InGameManager>().character[enemyNumber].GetComponent<BS_Character>().AngryEmozi());
				}
			}
		}
	}

	public void MagnetCollision(Collider other)
	{
		tails.Add(other.gameObject);
		int index = tails.Count - 1;
		other.gameObject.GetComponent<BS_Item>().SetTail(playerNumber, index, tails.Count);
		
		Color playerColor = new Color(0, 0, 0);
		switch(playerNumber)
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
			playerColor = new Color(0.0f, 0.0f, 0.0f, 1.0f); // Black
			break;
		}
		
		if(isBonusItem == true)
		{
			BS_ScoreManager.Instance().PlusScore(playerNumber, 10);
			scoreHUD.gameObject.GetComponent<HUDText>().AddLocalized("10", playerColor, 0);

			GameObject bonusEffectClone = Instantiate(bonusEffect) as GameObject;
			bonusEffectClone.transform.position = other.transform.position;
			bonusEffectClone.transform.position += new Vector3(0, 1.0f, 0);
		}
		else
		{
			BS_ScoreManager.Instance().PlusScore(playerNumber, 5);
			scoreHUD.gameObject.GetComponent<HUDText>().AddLocalized("5", playerColor, 0);
		}
		
		int myScore = BS_ScoreManager.Instance().GetPlayerScore(playerNumber);
		switch(other.gameObject.GetComponent<BS_Item>().GetItemType())
		{
		case ITEM_TYPE.ITEM_APPLE:
			lobbyHost.SendTo(playerNumber, "AppleSound," + myScore);
			break;
		case ITEM_TYPE.ITEM_SILVER:
			lobbyHost.SendTo(playerNumber, "SilverSound," + myScore);
			break;
		case ITEM_TYPE.ITEM_GOLD:
			lobbyHost.SendTo(playerNumber, "GoldSound," + myScore);
			break;
		case ITEM_TYPE.ITEM_RING:
			lobbyHost.SendTo(playerNumber, "RingSound," + myScore);
			break;
		case ITEM_TYPE.ITEM_DIAMOND:
			lobbyHost.SendTo(playerNumber, "DiamondSound," + myScore);
			break;
		case ITEM_TYPE.ITEM_SPEED:
			lobbyHost.SendTo(playerNumber, "SpeedSound," + myScore);
			break;
		case ITEM_TYPE.ITEM_MAGNET:
			lobbyHost.SendTo(playerNumber, "MagnetSound," + myScore);
			break;
		case ITEM_TYPE.ITEM_BONUS:
			lobbyHost.SendTo(playerNumber, "BonusSound," + myScore);
			break;
		}
	}

	void Start ()
	{
		lobbyHost = GameObject.Find ("LobbyHost").GetComponent<LobbyHost>();

		characterType = lobbyHost.selectedPlayerCharacter[playerNumber];

		if(characterType == (int)CHARACTER_TYPE.CHARACTER_DISCONNECTED)
		{
			this.gameObject.SetActive(false);
			return;
		}
		
		if(characterType != (int)CHARACTER_TYPE.CHARACTER_NONE)
		{
			character[(int)characterType].SetActive(true);
		}

		previousAngle = (int)this.transform.eulerAngles.y;
	}

	void Update ()
	{
		if(characterType == (int)CHARACTER_TYPE.CHARACTER_DISCONNECTED)
		{
			this.gameObject.SetActive(false);
			return;
		}

		this.transform.position = new Vector3(this.transform.position.x, 0.0f, this.transform.position.z);
#if UNITY_EDITOR
		KeyboardController();
#endif

		SpecialItemSearch();

		if(isSpeedItem == true)
		{
			speedEffect.SetActive(true);
		}
		else
		{
			speedEffect.SetActive(false);
		}

		if(isMagnetItem == true)
		{
			magnetCollider.SetActive(true);
			magnetEffect.SetActive(true);
		}
		else
		{
			magnetCollider.SetActive(false);
			magnetEffect.SetActive(false);
		}

		if(isMoving == true && isJoystickInput == false)
		{
			MoveCharacter(previousAngle, false);
		}
		else if(isMoving == false)
		{
			if(this.GetComponent<Rigidbody>().velocity.magnitude > minVelocity)
			{
				currentVelocity -= 0.2f;
				if(currentVelocity <= minVelocity)
				{
					currentVelocity = minVelocity;
				}
			}

			this.GetComponent<Rigidbody>().velocity = currentVelocity * this.transform.forward;
			this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		}

		TailsMove();
		isJoystickInput = false;

		if(characterType != (int)CHARACTER_TYPE.CHARACTER_NONE)
		{
			if(this.GetComponent<Rigidbody>().velocity.magnitude > 0)
			{
				character[(int)characterType].GetComponent<Animation>().Play("Run");
			}
			else
			{
				character[(int)characterType].GetComponent<Animation>().Play("Idle");
			}
		}

		confusionEmozi.transform.eulerAngles = Vector3.zero;
		angryEmozi.transform.eulerAngles = Vector3.zero;
		laughEmozi.transform.eulerAngles = Vector3.zero;
	}

	public void MoveCharacter(int angle, bool isJoystick)
	{
		if(isStunned == true)
		{
			return;
		}

		if(angle > 180)
		{
			angle -= 360;
		}
		else if(angle < -180)
		{
			angle += 360;
		}

		if(isJoystick == true)
		{
			if(angle < 0 && previousAngle > 0 && Mathf.Abs(previousAngle - angle) > 180)
			{
				angle += 360;
			}
			else if(angle > 0 && previousAngle < 0 && Mathf.Abs(previousAngle - angle) > 180)
			{
				angle -= 360;
			}

			finalAngle = angle;
			
			if(Mathf.Abs(previousAngle - angle) >= turnAngle)
			{
				if(angle > previousAngle)
				{
					angle = previousAngle + turnAngle;
					lerpAngle = turnAngle;
				}
				else
				{
					angle = previousAngle - turnAngle;
					lerpAngle = -turnAngle;
				}

				if(Mathf.Abs(previousAngle - angle) < turnAngle)
				{
					angle = previousAngle;
				}
			}
			else
			{
				angle = previousAngle;
			}
		}
		else
		{
			if(angle < 0 && finalAngle > 0 && Mathf.Abs(finalAngle - angle) > 180)
			{
				angle += 360;
			}
			else if(angle > 0 && finalAngle < 0 && Mathf.Abs(finalAngle - angle) > 180)
			{
				angle -= 360;
			}

			if(finalAngle != angle)
			{
				angle += lerpAngle;
			}
		}

		moveDirectionArrow.transform.eulerAngles = new Vector3(90.0f, finalAngle, 0);

		Vector3 moveDirection = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0, Mathf.Cos(Mathf.Deg2Rad * angle));

		if(isSpeedItem == true)
		{
			maxVelocity *= 1.5f;
		}
		
		float additionVelocity = maxVelocity * Mathf.Abs(1 - ((float)((angle + 360) % 180) / 90));
		if(this.GetComponent<Rigidbody>().velocity.magnitude < maxVelocity + additionVelocity)
		{
			currentVelocity += 0.2f;
			if(currentVelocity >= maxVelocity + additionVelocity)
			{
				currentVelocity = maxVelocity + additionVelocity;
			}
		}
		else
		{
			currentVelocity = maxVelocity + additionVelocity;
		}

		if(isSpeedItem == true)
		{
			maxVelocity /= 1.5f;
		}

		this.GetComponent<Rigidbody>().velocity = currentVelocity * moveDirection;

		Quaternion wantedRotation = Quaternion.LookRotation(moveDirection, this.transform.up);
		this.transform.rotation = wantedRotation;

		isMoving = true;
		isJoystickInput = isJoystick;
		previousAngle = angle;
	}

	public void StopCharacter()
	{
		isMoving = false;
		isJoystickInput = false;
	}

	void TailsMove()
	{
		for(int i = 0; i < tails.Count; i++)
		{
			GameObject preTail;
			GameObject thisTail;

			if(i == 0)
			{
				preTail = this.gameObject;
			}
			else
			{
				preTail = (GameObject)tails[i - 1];
			}

			thisTail = (GameObject)tails[i];


			Vector3 wantedPosition = preTail.transform.TransformPoint(0, 0, -bondDistance); 
			thisTail.transform.position = Vector3.Lerp (thisTail.transform.position, wantedPosition, Time.deltaTime * bondDamping);
			thisTail.transform.position = new Vector3(thisTail.transform.position.x, 0.9411629f, thisTail.transform.position.z);
			thisTail.GetComponent<Rigidbody>().velocity = Vector3.zero;

			Quaternion wantedRotation = Quaternion.LookRotation(preTail.transform.position - thisTail.transform.position, preTail.transform.up);
			thisTail.transform.rotation = Quaternion.Slerp (thisTail.transform.rotation, wantedRotation, Time.deltaTime * bondDamping);
			thisTail.transform.eulerAngles = new Vector3(0, thisTail.transform.eulerAngles.y, thisTail.transform.eulerAngles.z);
		}
	}

	void SpecialItemSearch()
	{
		isSpeedItem = false;
		isMagnetItem = false;
		isBonusItem = false;

		for(int i = 0; i < tails.Count; i++)
		{
			GameObject tail = (GameObject)tails[i];
			ITEM_TYPE tailItemType = tail.GetComponent<BS_Item>().GetItemType();

			if(tailItemType == ITEM_TYPE.ITEM_SPEED)
			{
				isSpeedItem = true;
			}

			if(tailItemType == ITEM_TYPE.ITEM_MAGNET)
			{
				isMagnetItem = true;
			}

			if(tailItemType == ITEM_TYPE.ITEM_BONUS)
			{
				isBonusItem = true;
			}
		}
	}

	public void SellingStart(GameObject respawnPosition)
	{
		this.gameObject.SetActive(false);
		this.gameObject.transform.position = respawnPosition.transform.position;
		this.gameObject.transform.localEulerAngles = respawnPosition.transform.eulerAngles;

		Invoke("SellingEnd", 1.0f);
	}

	public void SellingEnd()
	{
		this.gameObject.SetActive(true);
	}

	public bool IsBonusItem()
	{
		return isBonusItem;
	}

	public void CarCrash()
	{
		StartCoroutine(ConfusionEmozi());

		for(int i = 0; i < tails.Count; i++)
		{
			GameObject cutTail = (GameObject)tails[i];
			cutTail.GetComponent<BS_Item>().CutTail();
		}

		tails.Clear();

		lobbyHost.SendTo(playerNumber, "CrashEmojiSound");
	}

	public IEnumerator ConfusionEmozi()
	{
		iTween.ScaleTo(confusionEmozi.gameObject, Vector3.one, 0.5f);

		iTween.ScaleTo(angryEmozi.gameObject, Vector3.zero, 0.5f);
		iTween.ScaleTo(laughEmozi.gameObject, Vector3.zero, 0.5f);

		isStunned = true;

		yield return new WaitForSeconds(2.0f);

		iTween.ScaleTo(confusionEmozi.gameObject, Vector3.zero, 0.5f);

		isStunned = false;
	}

	public IEnumerator AngryEmozi()
	{
		iTween.ScaleTo(angryEmozi.gameObject, Vector3.one, 0.5f);

		iTween.ScaleTo(confusionEmozi.gameObject, Vector3.zero, 0.5f);
		iTween.ScaleTo(laughEmozi.gameObject, Vector3.zero, 0.5f);

		yield return new WaitForSeconds(2.0f);

		iTween.ScaleTo(angryEmozi.gameObject, Vector3.zero, 0.5f);
	}

	public IEnumerator LaughEmozi()
	{
		iTween.ScaleTo(laughEmozi.gameObject, Vector3.one, 0.5f);

		iTween.ScaleTo(confusionEmozi.gameObject, Vector3.zero, 0.5f);
		iTween.ScaleTo(angryEmozi.gameObject, Vector3.zero, 0.5f);
		
		yield return new WaitForSeconds(2.0f);
		
		iTween.ScaleTo(laughEmozi.gameObject, Vector3.zero, 0.5f);
	}

	void KeyboardController()
	{
#if UNITY_EDITOR
		float hDirection = Input.GetAxis("Horizontal");
		float vDirection = Input.GetAxis("Vertical");

		Vector3 moveDirection = Vector3.zero;
		moveDirection = new Vector3(0, 0, vDirection);
		this.transform.Translate(moveDirection * Time.deltaTime * moveSpeed);
		this.transform.Rotate(Vector3.up * Time.deltaTime * hDirection * 20.0f * 10.0f);
#endif
	}
}
