using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {
	public GameObject Player;
	public float xbaseMove = 0.05f;
	public float xMid = -3.4f;
	public float Yspeed = 0.03f;
	[HideInInspector]
	public float timestamp = 0.0f;
	private Vector3 startPos;

	void Start () 
	{
		startPos = this.transform.position;
		Update ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Player != null)
		{
			timestamp = GameObject.Find ("Level_Controller").GetComponent<Level_Controller> ().LevelTime;
			float XMove = (Player.transform.position.x - xMid) * xbaseMove;
			float yy = (timestamp * (-Yspeed)) * 50f;
			Vector2 offset = new Vector2 (-XMove, yy);

			this.transform.position = new Vector3(startPos.x + offset.x, startPos.y + offset.y);
		}
	}
}
