using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class FlagshipController : Entity 
{

	public Vector4 Bounds = new Vector4(-25,-13,11,13);
	public SheildType Sheildtype;
	public RadiatorType Radiatortype;
	public GeneratorType Generatortype;

	public FlagPhase[] BattlePhase;

	public Color32 MainColor;
	public Color32 SecondaryColor;
	public Color32 CockpitColor;
	[HideInInspector]
	public Color ActiveMainColor;
	[HideInInspector]
	public Color ActiveSecondaryColor;
	[HideInInspector]
	public Color ActiveCockpitColor;
	
	[HideInInspector]
	public int ActivePhase = 0;
	[HideInInspector]
	public float DreadSpeed;

	void Start () 
	{
		UpdateStats ();
		UpdateColor ();
		ShipHull = Shiptype.ShipHull;
		DreadSpeed = BattlePhase [ActivePhase].PhaseSpeed;
	}
	
	void Update () 
	{	
		if (isAlive)
		{
			if (ShipHeat < Shiptype.FreezeThreshold)
			{
				currentTime += Time.deltaTime / 5;
				BattlePhase [ActivePhase].PhaseAI.Move (BattlePhase[ActivePhase].PhaseSpeed / 5);

			}
			else
			{
				currentTime += Time.deltaTime;
				BattlePhase [ActivePhase].PhaseAI.Move (BattlePhase[ActivePhase].PhaseSpeed);

			}
			UpdateStats ();
			BattlePhase [ActivePhase].PhaseAI.AIUpdate ();
			UpdateColor ();
		}

	}

	void UpdateSprite ()
	{
		this.transform.Find ("Main").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteSet[ActivePhase];
		this.transform.Find ("Second").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteSet[ActivePhase];
		this.transform.Find ("Cockpit").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteSet[ActivePhase];

	}
	
	void UpdateStats()
	{
		if (BattlePhase [ActivePhase].PhaseAI.Move_order <= BattlePhase [ActivePhase].PhaseAI.MoveVunerableFrom)
		{
			Vulnerable = false;
		}
		else
		{
			Vulnerable = true;
		}
		
		ActiveMainColor = MainColor;//Start by fixing color
		ActiveSecondaryColor = SecondaryColor;//Start by fixing color
		ActiveCockpitColor = CockpitColor;//Start by fixing color
		
		//Handle Power
		if (ShipPower + (Generatortype.RechargeRate * Time.deltaTime) > 200) 
		{
			ShipPower = 200;
		}
		else
		{
			ShipPower += (Generatortype.RechargeRate * Time.deltaTime);
			ShipHeat += (Generatortype.HeatRate * Time.deltaTime);
		}
		
		ShipSpeed = Shiptype.ShipSpeed;
		if (ShipHeat < Shiptype.FreezeThreshold)
		{
			ShipSpeed = Shiptype.ShipSpeed / 5;
			ActiveMainColor = new Color32 (140,200,255,255);
			ActiveSecondaryColor = new Color32 (140,200,255,255);
			ActiveCockpitColor = new Color32 (140,200,255,255);
		}
		if (ShipHeat < 0)
		{
			ShipHeat += (Time.deltaTime * 5);
		}
		
		//Handle Sheild
		if (ShipSheild < Sheildtype.MaxSheild)
		{
			if (ShipSheildCycle + (Sheildtype.ChargeRate * Time.deltaTime)> 200)
			{
				ShipSheildCycle = 200;
			}
			else
			{
				ShipSheildCycle += (Sheildtype.ChargeRate * Time.deltaTime);
			}
		}
		else
		{
			ShipSheildCycle = 0;
		}
		
		//Handle EndSheild Cycle
		if (ShipSheildCycle == 200)
		{
			if (ShipPower - Sheildtype.CycleEnergyCost > 0)
			{
				ShipSheildCycle = 0;
				ShipPower -= Sheildtype.CycleEnergyCost;
				ShipHeat += Sheildtype.CycleHeatUse;
				if (ShipSheild + Sheildtype.CycleRecharge <  Sheildtype.MaxSheild)
				{
					ShipSheild += Sheildtype.CycleRecharge;
				}
				else
				{
					ShipSheild = Sheildtype.MaxSheild;
				}
			}
		}
		
		
		//Handle Heat
		if (ShipHeat > 200) 
		{
			ShipHeat = 200;
		}
		if (ShipHeat < -200) 
		{
			ShipHeat = -200;
		}
		
		if (ShipHeat > 5)
		{
			if (ShipHeatCycle + (Radiatortype.ChargeRate * Time.deltaTime)> 200)
			{
				ShipHeatCycle = 200;
			}
			else
			{
				ShipHeatCycle += (Radiatortype.ChargeRate * Time.deltaTime);
			}
		}
		else
		{
			ShipHeatCycle = 0;
		}
		
		//Handle EndHeat Cycle
		if (ShipHeatCycle == 200)
		{
			if (ShipPower - Radiatortype.CycleEnergyCost > 0)
			{
				ShipHeatCycle = 0;
				ShipPower -= Radiatortype.CycleEnergyCost;
				if (ShipHeat - Radiatortype.CycleCoolDown > 0)
				{
					ShipHeat -= Radiatortype.CycleCoolDown;
				}
				else
				{
					ShipHeat = 0;
				}
			}
		}
		
		if (ShipHeat > Shiptype.OverHeatThreshold) //HandleOverHeat
		{
			if (ShipHull - 8*Time.deltaTime > 20)
			{
				ShipHull -= 8*Time.deltaTime;
			}
			ActiveMainColor = Color.red;
			ActiveSecondaryColor = Color.red;
			ActiveCockpitColor = Color.red;
		}

		if (ShipHull < 0)//Handle Hull THIS IS ALWAYS LAST
		{
			BattlePhase[ActivePhase].PhaseAI.Move_timer = 0;
			BattlePhase[ActivePhase].PhaseAI.Move_order = 0;
			for (int ii = 0 ; ii < BattlePhase[ActivePhase].PhaseAI.WeaponFire.Length ; ii++)
			{
				BattlePhase[ActivePhase].PhaseAI.WeaponFire[ii] = false;
			}  
			ShipHull = 0;
			ShipExplode();
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
		
		this.transform.Find ("Main").GetComponent<SpriteRenderer> ().sprite = null;
		this.transform.Find ("Second").GetComponent<SpriteRenderer> ().sprite = null;
		this.transform.Find ("Cockpit").GetComponent<SpriteRenderer> ().sprite = null;

		
		this.isAlive = false;
	}

	public void RotateShip(Vector3 OldLoc , Vector3 NewLoc)
	{
		float ShipRotation = (OldLoc.x - NewLoc.x) * 50;
		int NewSprite = 2;
		if (ShipRotation < -5) 
		{
			NewSprite = 3;
		}
		if (ShipRotation < -20) 
		{
			NewSprite = 4;
		}
		if (ShipRotation > 5) 
		{
			NewSprite = 1;
		}
		if (ShipRotation > 20) 
		{
			NewSprite = 0;
		}
		this.transform.Find ("Main").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteSet[NewSprite];
		this.transform.Find ("Second").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteSecondSet[NewSprite];
		this.transform.Find ("Cockpit").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteCockpitSet[NewSprite];
	}

	void UpdateColor()
	{
		this.transform.Find ("Main").GetComponent<SpriteRenderer> ().color = ActiveMainColor;
		this.transform.Find ("Second").GetComponent<SpriteRenderer> ().color = ActiveSecondaryColor;
		this.transform.Find ("Cockpit").GetComponent<SpriteRenderer> ().color = ActiveCockpitColor;
		this.transform.Find ("Sheild").GetComponent<SpriteRenderer> ().color = Sheildtype.SheildHue;
	}

}