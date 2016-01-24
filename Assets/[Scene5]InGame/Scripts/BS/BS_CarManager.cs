using UnityEngine;
using System.Collections;

public enum DIRECTION
{
	DIRECTION_NORTH = 0,
	DIRECTION_EAST,
	DIRECTION_SOUTH,
	DIRECTION_WEST
}

public class BS_CarManager : MonoBehaviour
{
	public GameObject[] car = new GameObject[4];
	public Vector3[] carStartPos = new Vector3[4];

	private float moveTime = 10.0f;

	public GameObject itemBoxWest = null;
	public GameObject itemBoxNorth = null;
	public GameObject itemBoxEast = null;
	public GameObject itemBoxSouth = null;

	public AudioClip clacsonSound = null;

	void Start ()
	{
		for(int i = 0; i < car.Length; i++)
		{
			carStartPos[i] = car[i].transform.position;
			car[i].SetActive(false);
		}
	}

	public void CarStart(DIRECTION direction1, DIRECTION direction2)
	{
		StartCoroutine(CarMove(direction1, direction2));
	}
	
	IEnumerator CarMove(DIRECTION direction1, DIRECTION direction2)
	{
		for(int i = 0; i < car.Length; i++)
		{
			car[i].SetActive(false);
		}

		string carPath1 = "";
		string carPath2 = "";

		switch(direction1)
		{
		case DIRECTION.DIRECTION_NORTH:
			carPath1 = "NorthPath";
			itemBoxNorth.GetComponent<BS_ItemBox>().MakeItemBox();
			break;
		case DIRECTION.DIRECTION_EAST:
			carPath1 = "EastPath";
			itemBoxEast.GetComponent<BS_ItemBox>().MakeItemBox();
			break;
		case DIRECTION.DIRECTION_SOUTH:
			carPath1 = "SouthPath";
			itemBoxSouth.GetComponent<BS_ItemBox>().MakeItemBox();
			break;
		case DIRECTION.DIRECTION_WEST:
			carPath1 = "WestPath";
			itemBoxWest.GetComponent<BS_ItemBox>().MakeItemBox();
			break;
		}

		switch(direction2)
		{
		case DIRECTION.DIRECTION_NORTH:
			carPath2 = "NorthPath";
			itemBoxNorth.GetComponent<BS_ItemBox>().MakeItemBox();
			break;
		case DIRECTION.DIRECTION_EAST:
			carPath2 = "EastPath";
			itemBoxEast.GetComponent<BS_ItemBox>().MakeItemBox();
			break;
		case DIRECTION.DIRECTION_SOUTH:
			carPath2 = "SouthPath";
			itemBoxSouth.GetComponent<BS_ItemBox>().MakeItemBox();
			break;
		case DIRECTION.DIRECTION_WEST:
			carPath2 = "WestPath";
			itemBoxWest.GetComponent<BS_ItemBox>().MakeItemBox();
			break;
		}

		car[(int)direction1].SetActive(true);
		car[(int)direction2].SetActive(true);

		car[(int)direction1].GetComponent<BS_Car>().MoveCar(moveTime, carPath1);
		car[(int)direction2].GetComponent<BS_Car>().MoveCar(moveTime, carPath2);

		StartCoroutine(ClacsonSound());

		yield return new WaitForSeconds(moveTime);

		for(int i = 0; i < car.Length; i++)
		{
			car[i].transform.position = carStartPos[i];
			car[i].SetActive(false);
		}
	}

	IEnumerator ClacsonSound()
	{
		yield return new WaitForSeconds(1.0f);

		Camera.main.GetComponent<AudioSource>().PlayOneShot(clacsonSound);
	}
}
