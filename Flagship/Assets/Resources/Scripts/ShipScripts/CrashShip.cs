using UnityEngine;
using System.Collections;

public class CrashShip : Entity
{
	[HideInInspector]
	public Entity TargetShip;
	public float WeaponSeek;
	// Use this for initialization
	void Start () 
	{
		Entity[] Entitys = FindObjectsOfType<Entity>() as Entity[];
		float LastClose = 100.0f;
		foreach (Entity entity in Entitys) 
		{
			if (Mathf.Abs(entity.transform.position.x - this.transform.position.x) < LastClose && entity.gameObject.layer != this.gameObject.layer && entity != null)
			{
				TargetShip = entity as Entity;
				LastClose = Mathf.Abs(entity.transform.position.x - this.transform.position.x);
			}
		}
	}
	
	// Update is called once per frame
	// Update is called once per frame
	void Update () 
	{

		if (TargetShip != null)
		{
			Vector3 dir = Vector3.zero;
			dir = (TargetShip.transform.position - transform.position).normalized * (WeaponSeek / 10);
			this.GetComponent<Rigidbody> ().AddForce(dir);
			this.GetComponent<Rigidbody> ().velocity = this.GetComponent<Rigidbody> ().velocity.normalized * ShipSpeed;
		}

	}
}
