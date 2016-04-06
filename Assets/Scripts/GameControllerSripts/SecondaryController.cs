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
		StartCoroutine (spawnPowerups ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Powerup Spawner
	IEnumerator spawnPowerups ()
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
