using UnityEngine;
using System.Collections;

public class ObjectController : Entity {

	public float LifeSpan;
	public TransformSet MovementKey;
	[HideInInspector]
	public Vector3 StartPos;
	
	// Use this for initialization
	void Start ()
	{
		StartPos = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () 
	{
		currentTime += Time.deltaTime;
		if(isAlive)
		{
			UpdateStats ();
			BonkThings ();
			MoveObject ();
			if (currentTime > LifeSpan)
			{
				Destroy(this.gameObject);
			}
		}
	}

	void MoveObject()
	{
		this.transform.position = MovementKey.GetPosition(StartPos,currentTime);
	}


	void DestroySelf()
	{
		Destroy (this.gameObject);
	}

	void ShipExplode()
	{
		GameObject.Find ("Level_Controller").GetComponent<Level_Controller> ().score += ScoreValue;
		if (Explosion != null)
		{
			GameObject Explosion_Spawned = (GameObject)Instantiate(Explosion);
			Explosion_Spawned.transform.position = this.transform.position;
		}
		this.ShipHull = 0;
		this.Vulnerable = false;
		this.GetComponent<TimeDelayDestroy>().isActive = true;
		this.GetComponent<SpriteRenderer>().sprite = null;
		this.isAlive = false;
		Destroy (this);
	}
	
	void UpdateStats()
	{

		if (ShipHull < 0)//Handle Hull THIS IS ALWAYS LAST
		{
			ShipHull = 0;
			ShipExplode();
		}
	}

}
