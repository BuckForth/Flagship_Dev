using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class Entity : MonoBehaviour 
{
	public int ScoreValue = 0;
	public GameObject Explosion;
	public ShipType Shiptype;
	public bool Vulnerable = true;
	public bool isAlive = true;

	public bool FaceDown;

	public float ShipHull = 0;

	public float ShipSheild = 0;
	[HideInInspector]
	public float ShipSheildCycle = 0;
	[HideInInspector]
	public float ShipHeat = 0;
	[HideInInspector]
	public float ShipHeatCycle = 0;
	[HideInInspector]
	public float ShipPower = 0;
	[HideInInspector]
	public float ShipSpeed = 0;
	//[HideInInspector]
	public bool IsMainShoot = false;
	//[HideInInspector]
	public bool IsWingShoot = false;
	[HideInInspector]
	public float LastMainShot = -100f;
	[HideInInspector]
	public float LastWingShot = -100f;
	[HideInInspector]
	public float currentTime = 0.0f;
	[HideInInspector]
	public string fileName = "PlayerShip.fs";

	protected bool bonkable = true;

	public virtual void DamageEntity (float HeatDamage , float EnergyDamage , float PhysicalDamage)
	{
		if (Vulnerable)
		{
			float DammageToHull = 0;
			this.ShipHeat += HeatDamage;
			if (this.ShipSheild > 0)
			{
				if (EnergyDamage > this.ShipSheild)
				{
					this.ShipSheild = 0;
				}
				else
				{
					this.ShipSheild -= EnergyDamage;
					this.GetComponentInChildren<SheildRun>().ActiveSheild();
				}
			
				if ((PhysicalDamage/2) > this.ShipSheild)
				{
					DammageToHull = ((PhysicalDamage/2) - this.ShipSheild)*2;
					this.ShipSheild = 0;
					if (DammageToHull > 0) 
					{
						this.ShipHull -= DammageToHull;
					}
				}
				else
				{
					this.ShipSheild -= (PhysicalDamage/2);
				}
			}
			else
			{
			this.ShipHull -= PhysicalDamage;
			}
		}
		else
		{
			Debug.Log("Tink!");
		}
	}

	public bool isBonkable()
	{
		return bonkable;
	}

	public void BonkThings () 
	{	
		Entity[] Entitys = FindObjectsOfType<Entity>() as Entity[];
		foreach (Entity entity in Entitys) 
		{
			if (entity.isBonkable() == true && (entity.transform.position - this.transform.position).magnitude < (entity.Shiptype.Shipsize - this.Shiptype.Shipsize).magnitude && entity.gameObject.layer != this.gameObject.layer)
			{
				this.DamageEntity(0, 400 * Time.deltaTime , 800 * Time.deltaTime);
			}
		}
	}
}