using UnityEngine;
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
			Destroy(grid[mousePositionX][mousePositionY]);
			Debug.Log("Destroy(grid[" + mousePositionX +"]["+ mousePositionY+ "]");
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

		// mouseText.text = "Mouse position: " + Input.mousePosition;
		mouseText.text = "Mouse position: (" + mousePositionX + ", " + mousePositionY + ")";

		mouseTextWorld.text = "Mouse position: " + camera.ScreenToWorldPoint(Input.mousePosition);

	}

//	void OnMouseUpAsButton()
//	{
//		Debug.Log("Button pressed.");
//	}
		
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
				spawned.GetComponent<MatchObjectController> ().yPos = ii;
				grid[i][ii] = spawned;
				yield return new WaitForSeconds (0.05f);
			}
		}

		/*
		foreach (GameObject i in grid[0])
		{
			Debug.Log (i.tag);
		}
		Debug.Log (grid[3][9].tag);
		*/
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
					spawned.GetComponent<MatchObjectController> ().yPos = ii;
					grid[i][ii] = spawned;
					yield return new WaitForSeconds (0.05f);
				}
			}
		}

//		foreach (GameObject i in grid[0])
//		{
//			Debug.Log (i.tag);
//		}
//		Debug.Log ("object x.3 y.9"+grid[3][9].tag);
	}

	// Check for matches
	void checkMatch()
	{
		//Debug.Log (grid[0][0].tag);
		// Check for horizontal matches
		for (int i = 0; i < gridY; i++)
		{
			for (int ii = 0; ii < gridX; ii++)
			{
				//Debug.Log("grid["+i+"]["+ii+"]");
				//GameObject[] tempMatch = new GameObject[];
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
					// Debug.Log("Match: " + tempMatch);
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
				//Debug.Log("grid["+i+"]["+ii+"]");
				//GameObject[] tempMatch = new GameObject[];
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
					// Debug.Log("Match: " + tempMatch);
					Debug.Log("Vertical match on object x."+i+" y."+ii+"; counted: "+tempMatch.Count+"; of type: "+grid[i][ii].tag);
					foreach (GameObject x in tempMatch)
					{
						Match.Add(x);
					}
				}

				ii = iii - 1;
			}
		}
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
}
