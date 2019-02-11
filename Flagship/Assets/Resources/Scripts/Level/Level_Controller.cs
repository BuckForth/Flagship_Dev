using UnityEngine;
using System.Collections;

public class Level_Controller : MonoBehaviour {

	// Use this for initialization
	public string playerFile = "Player.fs";
	public int seed;
	public int score = 0;
	public int fodderkill = 0;
	public float LevelTime = 0.0f;
	public float timeScale = 1.0f;
	public bool TimeAdding = true;

	void Start () 
	{
		Random.seed = seed;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (TimeAdding)
		{
			LevelTime += (Time.deltaTime * timeScale);
		}


	}

}
