using UnityEngine;
using System.Collections;

public class MatchObjectController : MonoBehaviour {

	public float smoothing;
	public float acceleration;

	[HideInInspector]
	public int yPos;

	private Transform cubeTransform;
	private int gridY;

	// Use this for initialization
	void Start ()
	{
		cubeTransform = GetComponentsInChildren<Transform> ()[1];
		gridY = GameObject.FindWithTag("GameController").GetComponent <SecondaryController> ().ySpawn;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		// Rotator
		cubeTransform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);

		// Fall into position
		float newPosition = Mathf.MoveTowards (transform.position.y, yPos, Time.deltaTime * (smoothing  * ((gridY - transform.position.y) * acceleration )));
		transform.position = new Vector3 (transform.position.x, newPosition, 0);
	}
}
