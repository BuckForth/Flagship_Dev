using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class FlagshipTwinSet : Entity 
{
	public FlagshipController[] TwinSet = new FlagshipController[2];
	public float TwinShipHull = 0;
	public float TwinShipSheild = 0;
	
	[HideInInspector]
	public int livingCount = 0;

	void Start ()
	{
		GameObject.Find ("Hud").GetComponent<HudController> ().BossStatEnt = this;
	}

	void Update()
	{
		livingCount = 0;
		for (int ii = 0; ii < TwinSet.Length; ii ++)
		{
			if (TwinSet[ii] != null)
			{
				livingCount++;
			}
		}
		if(livingCount < 1)
		{
			ShipExplode();
		}
		float allHealth = 0;
		float allMaxHealth = 0;
		float allSheild = 0;
		float allMaxSheild = 0;
		for (int ii = 0; ii < TwinSet.Length; ii ++)
		{
			if (TwinSet[ii] != null && TwinSet[ii].ActivePhase != (TwinSet.Length - livingCount))
			{
				allHealth += TwinSet[ii].ShipHull;
				allMaxHealth += TwinSet[ii].Shiptype.ShipHull;
				allSheild += TwinSet[ii].ShipSheild;
				allMaxSheild += TwinSet[ii].Sheildtype.MaxSheild;
				TwinSet[ii].ActivePhase = TwinSet.Length - livingCount;
				TwinSet[ii].BattlePhase[TwinSet[ii].ActivePhase].PhaseAI.Move_timer = 0;
				TwinSet[ii].BattlePhase[TwinSet[ii].ActivePhase].PhaseAI.Move_order = 0;
			}
		}

		TwinShipHull = (allHealth / allMaxHealth) * 200;
		TwinShipSheild = (allSheild / allMaxSheild) * 200;

		
	}
/*
	public void DamageEntity (float HeatDamage , float EnergyDamage , float PhysicalDamage)
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
/*/
	void ShipExplode()
	{
		if (Explosion != null)
		{
			GameObject Explosion_Spawned = (GameObject)Instantiate(Explosion);
			Explosion_Spawned.transform.position = this.transform.position;
		}
		this.ShipHull = 0;
		this.Vulnerable = false;
		this.GetComponent<TimeDelayDestroy>().isActive = true;
		
		this.transform.Find ("Main").GetComponent<SpriteRenderer> ().sprite = null;
		this.transform.Find ("Second").GetComponent<SpriteRenderer> ().sprite = null;
		this.transform.Find ("Cockpit").GetComponent<SpriteRenderer> ().sprite = null;
		
		this.isAlive = false;
	}
}