using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class DreadnoughtController_Eye : DreadnoughtController 
{
	public Color darkened;
	void Start () 
	{

		GameObject.Find ("Hud").GetComponent<HudController> ().BossStatEnt = this;
		ShipHull = BattlePhase[ActivePhase].PhaseHull;
		DreadSpeed = BattlePhase[ActivePhase].PhaseSpeed;
		Shiptype.Shipsize = BattlePhase[ActivePhase].PhaseSize;
		UpdateStats ();

	}
	
	void Update () 
	{	
		currentTime += Time.deltaTime;
		BattlePhase [ActivePhase].PhaseAI.Move (BattlePhase[ActivePhase].PhaseSpeed);
		UpdateStats ();
		BattlePhase [ActivePhase].PhaseAI.AIUpdate ();
	}

	void UpdateSprite ()
	{
		this.transform.Find ("Main").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteSet[ActivePhase];
	}
	
	void UpdateStats()
	{


		if (ActivePhase == 0)
		{
			Eyeball_Claw[] claws = FindObjectsOfType<Eyeball_Claw> ();
			if (ShipHull < 500)
			{
				foreach (Eyeball_Claw claw in claws)
				{
					claw.ShipHull = 0;
					claw.DamageEntity (0f, 0f, 10f);
				}
			}
			if (claws.Length == 0)
			{
				BattlePhase[ActivePhase].PhaseAI.Move_timer = 0;
				BattlePhase[ActivePhase].PhaseAI.Move_order = 0;
				for (int ii = 0 ; ii < BattlePhase[ActivePhase].PhaseAI.WeaponFire.Length ; ii++)
				{
					BattlePhase[ActivePhase].PhaseAI.WeaponFire[ii] = false;
				}
				ActivePhase++;
				if (ActivePhase == BattlePhase.Length)
				{
					ShipHull = 0;
					ShipExplode();
				}
				else
				{
					if (ActivePhase == 1)
					{
						transform.Find ("BackPeice").GetComponent<SpriteRenderer> ().color = darkened;
						transform.Find ("BackPeice").parent = null;
					}
					GameObject Explosion_Spawned = (GameObject)Instantiate(BattlePhase[ActivePhase - 1].PhaseAI.ExplosionObj);
					Explosion_Spawned.transform.position = this.transform.position;
					Explosion_Spawned.transform.parent = this.transform;

					BattlePhase[ActivePhase].PhaseAI.Move_timer = 0;
					BattlePhase[ActivePhase].PhaseAI.Move_order = 0;
					DreadSpeed = BattlePhase[ActivePhase].PhaseSpeed;
					Shiptype.Shipsize = BattlePhase[ActivePhase].PhaseSize;
					UpdateSprite();

				}
			}
		}
		if (BattlePhase [ActivePhase].PhaseAI.Move_order <= BattlePhase [ActivePhase].PhaseAI.MoveVunerableFrom)
		{
			Vulnerable = false;
		}
		else
		{
			Vulnerable = true;
		}

		if (ShipHull < 0)//Handle Hull THIS IS ALWAYS LAST
		{
			BattlePhase[ActivePhase].PhaseAI.Move_timer = 0;
			BattlePhase[ActivePhase].PhaseAI.Move_order = 0;
			for (int ii = 0 ; ii < BattlePhase[ActivePhase].PhaseAI.WeaponFire.Length ; ii++)
			{
				BattlePhase[ActivePhase].PhaseAI.WeaponFire[ii] = false;
			}
			ActivePhase++;
			if (ActivePhase == BattlePhase.Length)
			{
				ShipHull = 0;
				ShipExplode();
			}
			else
			{
				GameObject Explosion_Spawned = (GameObject)Instantiate(BattlePhase[ActivePhase - 1].PhaseAI.ExplosionObj);
				Explosion_Spawned.transform.position = this.transform.position;
				Explosion_Spawned.transform.parent = this.transform;

				BattlePhase[ActivePhase].PhaseAI.Move_timer = 0;
				BattlePhase[ActivePhase].PhaseAI.Move_order = 0;
				ShipHull = BattlePhase[ActivePhase].PhaseHull;
				DreadSpeed = BattlePhase[ActivePhase].PhaseSpeed;
				Shiptype.Shipsize = BattlePhase[ActivePhase].PhaseSize;
				UpdateSprite();

			}
		}


	}
	
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
		Destroy(this.transform.Find ("Smoke_Trail"));
		this.transform.Find ("Main").GetComponent<SpriteRenderer> ().color = darkened;
		this.transform.Find ("Second").GetComponent<SpriteRenderer> ().color = darkened;
		this.transform.Find ("Cockpit").GetComponent<SpriteRenderer> ().color = darkened;
		
		this.isAlive = false;
	}


}