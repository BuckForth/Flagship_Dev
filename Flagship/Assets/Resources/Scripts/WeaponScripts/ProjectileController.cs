using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {
	public float Size = 2;
	[HideInInspector]
	public Entity TargetShip;
	[HideInInspector]
	public float WeaponSpeed = 0.0f;
	[HideInInspector]
	public float WeaponLife = 1.0f;
	[HideInInspector]
	public float WeaponSeek = 0.0f;
	[HideInInspector]
	public float WeaponHeat = 0.0f;
	[HideInInspector]
	public float WeaponPhysicalDamage = 0.0f;
	[HideInInspector]
	public float WeaponEnergyDamage = 0.0f;
	[HideInInspector]
	public float DammageToHull = 0.0f;
	[HideInInspector]
	public bool IsFlash = false;
	[HideInInspector]
	public bool staticSpeed = false;
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
	void Update () 
	{
		WeaponLife = WeaponLife - Time.deltaTime;
		if (WeaponLife < 0 || this.transform.position.y > 14) 
		{
			Destroy(this.gameObject);
		}

		if (TargetShip != null)
		{
			
			Vector3 dir = Vector3.zero;
			dir = (TargetShip.transform.position - transform.position).normalized * (WeaponSeek / 10);
			this.GetComponent<Rigidbody> ().AddForce(dir);

			if (staticSpeed)
			{
				if (this.GetComponent<Rigidbody> ().velocity.magnitude > WeaponSpeed)
				{
					this.GetComponent<Rigidbody> ().AddForce (this.GetComponent<Rigidbody> ().velocity.normalized * WeaponSpeed);
					this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				}
			}

		}


		Entity[] Entitys = FindObjectsOfType<Entity>() as Entity[];
		foreach (Entity entity in Entitys) 
		{
			if (Mathf.Abs(entity.transform.position.y - this.transform.position.y) < (Size +  entity.Shiptype.Shipsize.y) && Mathf.Abs(entity.transform.position.x - this.transform.position.x) < (Size +  entity.Shiptype.Shipsize.x) && entity.gameObject.layer != this.gameObject.layer  && entity != null )//This is distance and layer
			{
				if(!IsFlash)
				{
					Destroy(this.gameObject);
					entity.DamageEntity(WeaponHeat , WeaponEnergyDamage , WeaponPhysicalDamage);
				}
				else
				{
					entity.DamageEntity(WeaponHeat * Time.deltaTime , WeaponEnergyDamage * Time.deltaTime , WeaponPhysicalDamage * Time.deltaTime);
				}

				if (entity is NeuroShipController)
				{
					NeuroShipController nuroEnt = (NeuroShipController) entity;
					nuroEnt.hits ++;
				}
			}
		}


	}

}
