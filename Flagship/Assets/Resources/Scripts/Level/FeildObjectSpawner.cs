using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class FeildObjectSpawner : MonoBehaviour 
{

	private GameObject Level_ControllerOBJ;
	public BoxCollider2D TargetArea;
	public float TriggerTime = 0.0f;
	public float RunTime = 0.5f;
	public float ObjFreq = 1.0f;
	public WeightedSpawnAndTrans[] SpawnList;
	[HideInInspector]
	public float timercount = 0.0f;
	[HideInInspector]
	public bool isactive = false;
	
	// Use this for initialization
	void Start () 
	{
				Level_ControllerOBJ = GameObject.Find ("Level_Controller");
		timercount = 0.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (TriggerTime < Level_ControllerOBJ.GetComponent<Level_Controller>().LevelTime)
		{
			if( Time.timeScale != 0 && UnityEngine.Random.value < ObjFreq)
			{
				float totalWeight = 0;
				foreach (WeightedSpawnAndTrans spawnableObject in SpawnList)
				{
					totalWeight += spawnableObject.Weight;
				}
				float selectedWeight = (UnityEngine.Random.value * totalWeight);
				GameObject objectToSpawn = this.gameObject; 
				foreach (WeightedSpawnAndTrans spawnableObject in SpawnList)
				{
					selectedWeight -= spawnableObject.Weight;
					if (selectedWeight <= 0 )
					{
						objectToSpawn = (GameObject)Instantiate(spawnableObject.spawnedObject);
						objectToSpawn.GetComponent<ObjectController>().MovementKey = spawnableObject.Trans;
						objectToSpawn.transform.position = new Vector3 (this.transform.position.x + ((UnityEngine.Random.value - 0.5f) * (this.TargetArea.size.x)) , this.transform.position.y + ((UnityEngine.Random.value - 0.5f) * (this.TargetArea.size.y)) ,0);
						objectToSpawn.layer = this.gameObject.layer;
						break;
					}
				}

			}

			isactive = true;
		}

		if (isactive)
		{
			timercount += Time.deltaTime * Time.timeScale;
		}

		if (timercount > RunTime)
		{
			Destroy(this.gameObject);
		}


	}
	
}