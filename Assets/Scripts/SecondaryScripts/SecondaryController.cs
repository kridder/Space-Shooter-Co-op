﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecondaryController : MonoBehaviour {

	public const int gridX = 8;
	public const int gridY = 12;

	public int ySpawn;

	public GUIText mouseText;
	public GUIText mouseTextWorld;

	public GameObject[] powerups;
	//private GameObject[,] grid = new GameObject[8,12];
	private GameObject[][] grid = new GameObject[gridX][];
	private List<GameObject> Match = new List<GameObject>();
	private List<int> Selected = new List<int>();
	private GameObject Swap;


	private new Camera camera;
	private Vector3 mouseVector;
	private int mousePositionX;
	private int mousePositionY;

	// Use this for initialization
	void Start () {
		ySpawn = gridY + 1;

		camera  = GameObject.FindWithTag("MainCamera").GetComponent <Camera> ();

		for (int i = 0; i < grid.Length; i++)
		{
			grid[i] = new GameObject[gridY];
		}
		StartCoroutine (InitSpawnPowerups ());
	}
	
	// Update is called once per frame
	void Update () {
		mouseVector = camera.ScreenToWorldPoint(Input.mousePosition);
		mousePositionX = Mathf.RoundToInt(mouseVector.x);
		mousePositionY = Mathf.RoundToInt(mouseVector.y);

		if (Input.GetMouseButtonDown(0) && mousePositionX >= 0 && mousePositionX < gridX && mousePositionY >= 0 && mousePositionY < gridY)
		{
			Debug.Log("Selected (grid[" + mousePositionX +"]["+ mousePositionY+ "]");
			if (Selected.Count == 0)
			{
				Selected.Add(mousePositionX);
				Selected.Add(mousePositionY);
			} else if (Selected[0] == mousePositionX && (Selected[1] == (mousePositionY - 1) || Selected[1] == (mousePositionY + 1))
				|| Selected[1] == mousePositionY && (Selected[0] == (mousePositionX - 1) || Selected[0] == (mousePositionX + 1)))
			{

				Swap = grid[mousePositionX][mousePositionY];

				grid[mousePositionX][mousePositionY] = grid[Selected[0]][Selected[1]];
				grid[mousePositionX][mousePositionY].GetComponent<MatchObjectController> ().xPos = mousePositionX;
				grid[mousePositionX][mousePositionY].GetComponent<MatchObjectController> ().yPos = mousePositionY;
				grid[mousePositionX][mousePositionY].GetComponent<MatchObjectController> ().swap = true;

				grid[Selected[0]][Selected[1]] = Swap;
				grid[Selected[0]][Selected[1]].GetComponent<MatchObjectController> ().xPos = Selected[0];
				grid[Selected[0]][Selected[1]].GetComponent<MatchObjectController> ().yPos = Selected[1];
				grid[Selected[0]][Selected[1]].GetComponent<MatchObjectController> ().swap = true;

				if (checkMatch())
				{
					Debug.Log("Matches found.");

					StartCoroutine (checkCycle ());

//					deleteMatches();
//					StartCoroutine (spawnPowerups ());
				} else {
					Debug.Log("No matches found.");

					StartCoroutine (revertSwap(Selected[0], Selected[1], mousePositionX, mousePositionY, Swap));

//					grid[Selected[0]][Selected[1]] = grid[mousePositionX][mousePositionY];
//					grid[Selected[0]][Selected[1]].GetComponent<MatchObjectController> ().xPos = Selected[0];
//					grid[Selected[0]][Selected[1]].GetComponent<MatchObjectController> ().yPos = Selected[1];
//					grid[Selected[0]][Selected[1]].GetComponent<MatchObjectController> ().swap = true;
//
//					grid[mousePositionX][mousePositionY] = Swap;
//					grid[mousePositionX][mousePositionY].GetComponent<MatchObjectController> ().xPos = mousePositionX;
//					grid[mousePositionX][mousePositionY].GetComponent<MatchObjectController> ().yPos = mousePositionY;
//					grid[mousePositionX][mousePositionY].GetComponent<MatchObjectController> ().swap = true;
				}

				Swap = null;

				Selected.Clear();
				Selected.TrimExcess();

			} else {
				Selected.Clear();
				Selected.TrimExcess();
			}
		}
			
		if (Input.GetKeyDown (KeyCode.X)) 
		{
			StartCoroutine (spawnPowerups ());
		}

		// Print list of logged powerups
		if (Input.GetKeyDown (KeyCode.Z)) 
		{
			foreach (GameObject i in grid[0])
			{
				if (i != null)
				{
					Debug.Log (i.transform.position + " " + i.tag);
				}
			}
		}

		if (Input.GetKeyDown (KeyCode.C)) 
		{
			checkMatch ();
		}

		if (Input.GetKeyDown (KeyCode.V)) 
		{
			deleteMatches();
		}

		if (Input.GetKeyDown (KeyCode.B)) 
		{
			StartCoroutine (checkCycle ());
		}

		// mouseText.text = "Mouse position: " + Input.mousePosition;
		mouseText.text = "Mouse position: (" + mousePositionX + ", " + mousePositionY + ")";

		mouseTextWorld.text = "Mouse position: " + camera.ScreenToWorldPoint(Input.mousePosition);

	}
		
	// Spawn initial playfield
	IEnumerator InitSpawnPowerups ()
	{
		for (int i = 0; i < gridX; i++)
		{
			for (int ii = 0; ii < gridY; ii++)
			{
				GameObject powerup = powerups [Random.Range (0, powerups.Length)];
				Vector3 spawnPosition = new Vector3 (i, gridY, 0);
				Quaternion spawnRotation = Quaternion.identity;
				GameObject spawned = Instantiate (powerup, spawnPosition, spawnRotation) as GameObject;
				spawned.GetComponent<MatchObjectController> ().xPos = i;
				spawned.GetComponent<MatchObjectController> ().yPos = ii;
				grid[i][ii] = spawned;
				yield return new WaitForSeconds (0.05f);
			}
		}
	}

	// Powerup Spawner
	IEnumerator spawnPowerups ()
	{
		for (int i = 0; i < gridX; i++)
		{
			bool spawnNew = false;
			for (int ii = 0; ii < gridY; ii++)
			{
				if (!spawnNew && grid[i][ii] == null)
				{
					spawnNew = true;
					for (int iii = ii + 1; iii < gridY; iii++)
					{
						if (grid[i][iii] != null)
						{
							grid[i][ii] = grid[i][iii];
							grid[i][ii].GetComponent<MatchObjectController> ().yPos = ii;
							grid[i][iii] = null;
							spawnNew = false;
							break;
						}
					}
				}

				if (spawnNew)
				{
					GameObject powerup = powerups [Random.Range (0, powerups.Length)];
					Vector3 spawnPosition = new Vector3 (i, gridY, 0);
					Quaternion spawnRotation = Quaternion.identity;
					GameObject spawned = Instantiate (powerup, spawnPosition, spawnRotation) as GameObject;
					spawned.GetComponent<MatchObjectController> ().xPos = i;
					spawned.GetComponent<MatchObjectController> ().yPos = ii;
					grid[i][ii] = spawned;
					yield return new WaitForSeconds (0.05f);
				}
			}
		}
	}

	// Check for matches
	bool checkMatch()
	{
		bool ret = false;
		// Check for horizontal matches
		for (int i = 0; i < gridY; i++)
		{
			for (int ii = 0; ii < gridX; ii++)
			{
				//Debug.Log("grid["+i+"]["+ii+"]");
				List<GameObject> tempMatch = new List<GameObject>();
				tempMatch.Add (grid[ii][i]);
				int iii;
				for (iii = ii + 1; iii < gridX; iii++)
				{
					if (grid[ii][i].tag == grid[iii][i].tag)
					{
						tempMatch.Add (grid[iii][i]);
					} else {
						break;
					}
				}

				if (tempMatch.Count >= 3)
				{
					ret = true;
					Debug.Log("Horizontal match on object y."+i+" x."+ii+"; counted: "+tempMatch.Count+"; of type: "+grid[ii][i].tag);
					foreach (GameObject x in tempMatch)
					{
						Match.Add(x);
					}
				}

				ii = iii - 1;
			}
		}

		// Check for vertical matches
		for (int i = 0; i < gridX; i++)
		{
			for (int ii = 0; ii < gridY; ii++)
			{
				List<GameObject> tempMatch = new List<GameObject>();
				tempMatch.Add (grid[i][ii]);
				int iii;
				for (iii = ii + 1; iii < gridY; iii++)
				{
					if (grid[i][ii].tag == grid[i][iii].tag)
					{
						tempMatch.Add (grid[i][iii]);
					} else {
						break;
					}
				}

				if (tempMatch.Count >= 3)
				{
					ret = true;
					Debug.Log("Vertical match on object x."+i+" y."+ii+"; counted: "+tempMatch.Count+"; of type: "+grid[i][ii].tag);
					foreach (GameObject x in tempMatch)
					{
						Match.Add(x);
					}
				}

				ii = iii - 1;
			}
		}
		return ret;
	}

	// Delete matches
	void deleteMatches()
	{
		foreach (GameObject i in Match)
		{
			Destroy(i);
		}
		Match.Clear();
		Match.TrimExcess();
	}

	IEnumerator checkCycle ()
	{
		bool run = true;
		while (run)
		{
			yield return new WaitForSeconds (0.25f);
			deleteMatches();
			yield return new WaitForSeconds (0.05f);
			yield return StartCoroutine (spawnPowerups ());
			yield return new WaitForSeconds (0.05f);
			Debug.Log("Finsihed checkCycle");
			run = checkMatch();
		}
	}

	IEnumerator revertSwap(int x, int y, int mousePositionX, int mousePositionY, GameObject Swap)
	{
		yield return new WaitForSeconds (0.25f);
		grid[x][y] = grid[mousePositionX][mousePositionY];
		grid[x][y].GetComponent<MatchObjectController> ().xPos = x;
		grid[x][y].GetComponent<MatchObjectController> ().yPos = y;
		grid[x][y].GetComponent<MatchObjectController> ().swap = true;

		grid[mousePositionX][mousePositionY] = Swap;
		grid[mousePositionX][mousePositionY].GetComponent<MatchObjectController> ().xPos = mousePositionX;
		grid[mousePositionX][mousePositionY].GetComponent<MatchObjectController> ().yPos = mousePositionY;
		grid[mousePositionX][mousePositionY].GetComponent<MatchObjectController> ().swap = true;
	}
}
