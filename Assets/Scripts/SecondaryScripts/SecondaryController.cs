using UnityEngine;
using System.Collections;

public class SecondaryController : MonoBehaviour {

	public GameObject[] powerups;
	//private GameObject[,] grid = new GameObject[8,12];
	private GameObject[][] grid = new GameObject[8][];

	// Use this for initialization
	void Start () {
		for (int i = 0; i < grid.Length; i++)
		{
			grid[i] = new GameObject[12];
		}
		StartCoroutine (InitSpawnPowerups ());
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.P)) 
		{
			StartCoroutine (spawnPowerups ());
			Debug.Log("p key was pressed");
		}
	}

	// Powerup Spawner
	IEnumerator spawnPowerups ()
	{
		for (int i = 0; i < grid.Length; i++)
		{
			for (int ii = 0; ii < grid[i].Length; ii++)
			{
				if (grid[i][ii] == null)
				{
					bool spawnNew = true;
					for (int iii = ii + 1; iii < grid[i].Length; iii++)
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

					if (spawnNew)
					{
						GameObject powerup = powerups [Random.Range (0, powerups.Length)];
						Vector3 spawnPosition = new Vector3 (i, 12, 0);
						Quaternion spawnRotation = Quaternion.identity;
						GameObject spawned = Instantiate (powerup, spawnPosition, spawnRotation) as GameObject;
						spawned.GetComponent<MatchObjectController> ().yPos = ii;
						grid[i][ii] = spawned;
						yield return new WaitForSeconds (0.05f);
					}
				}
			}
		}

		foreach (GameObject i in grid[0])
		{
			Debug.Log (i.tag);
		}
		Debug.Log (grid [3] [9].tag);
	}

	// Spawn initial playfield
	IEnumerator InitSpawnPowerups ()
	{
		for (int i = 0; i < grid.Length; i++)
		{
			for (int ii = 0; ii < grid[i].Length; ii++)
			{
				GameObject powerup = powerups [Random.Range (0, powerups.Length)];
				Vector3 spawnPosition = new Vector3 (i, 12, 0);
				Quaternion spawnRotation = Quaternion.identity;
				GameObject spawned = Instantiate (powerup, spawnPosition, spawnRotation) as GameObject;
				spawned.GetComponent<MatchObjectController> ().yPos = ii;
				grid[i][ii] = spawned;
				yield return new WaitForSeconds (0.05f);
			}
		}

		foreach (GameObject i in grid[0])
		{
			Debug.Log (i.tag);
		}
		Debug.Log (grid [3] [9].tag);
	}
}
