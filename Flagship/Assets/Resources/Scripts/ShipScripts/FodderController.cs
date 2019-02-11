using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class FodderController : Entity 
{
	public bool LoadCustom;
	public bool isFodder = false;
	public SheildType Sheildtype;
	public RadiatorType Radiatortype;
	public GeneratorType Generatortype;
	public WeaponType MainWeapon;
	public WeaponType WingWeapon;
	public float LifeSpan;
	public float ShootDelay;
	public TransformKey MovementKey;
	public TransformSet idleKey;
	public Vector4 Bounds = new Vector4(-25,-13,11,13);

	public Color32 MainColor;
	public Color32 SecondaryColor;
	public Color32 CockpitColor;

	public float delay = 0;
	[HideInInspector]
	public Vector3 StartPos;
	[HideInInspector]
	public Color ActiveMainColor;
	[HideInInspector]
	public Color ActiveSecondaryColor;
	[HideInInspector]
	public Color ActiveCockpitColor;
	private bool started = false;

	public GameObject Player;
	public float xbaseMove = 0.05f;
	public float xMid = -3.4f;

	private Vector3 oldXMove = Vector3.zero;
	
	void Start () 
	{
		MainColor = this.transform.Find ("Main").GetComponent<SpriteRenderer> ().color;
		SecondaryColor = this.transform.Find ("Second").GetComponent<SpriteRenderer> ().color;
		CockpitColor = this.transform.Find ("Cockpit").GetComponent<SpriteRenderer> ().color;

		StartPos = this.transform.position;
		Player = GameObject.Find ("Ship_Player");
		UpdateStats ();
		UpdateColor ();
		RotateShip (this.transform.position, this.transform.position);
		ActiveMainColor = MainColor;
		ActiveSecondaryColor = SecondaryColor;
		ActiveCockpitColor = CockpitColor;
		ShipHull = Shiptype.ShipHull;
		ShipSheild = Sheildtype.MaxSheild;
		ShipHeat = 0;
		ShipPower = 200;
		currentTime = 0;
		if (MainWeapon != null)
		{
			MainWeapon = new WeaponType (MainWeapon);
		}
		if (WingWeapon != null)
		{
			WingWeapon = new WeaponType (WingWeapon);
		}
	}
	
	void Update ()
	{	
		if (ShipHeat < Shiptype.FreezeThreshold) 
		{
			currentTime += Time.deltaTime / 5;
		} else 
		{
			currentTime += Time.deltaTime;
		}
		MoveShip ();
		if (currentTime > ShootDelay) 
		{
			ShootGuns ();
		}

		if (isAlive) 
		{
			BonkThings ();
			UpdateStats ();
			UpdateColor ();
			RunWeapons ();
		}

		if (currentTime > LifeSpan) 
		{
			Destroy (this.gameObject);
		}

	}

	void UpdateColor()
	{
		this.transform.Find ("Main").GetComponent<SpriteRenderer> ().color = ActiveMainColor;
		this.transform.Find ("Second").GetComponent<SpriteRenderer> ().color = ActiveSecondaryColor;
		this.transform.Find ("Cockpit").GetComponent<SpriteRenderer> ().color = ActiveCockpitColor;
		this.transform.Find ("Sheild").GetComponent<SpriteRenderer> ().color = Sheildtype.SheildHue;
	}

	void RunWeapons ()
	{
		if (IsMainShoot && MainWeapon != null)
		{ 
			if (MainWeapon.IsBeam == false)
			{
				if (currentTime - LastMainShot > MainWeapon.FireRate)
				{
					
					if (ShipPower - (MainWeapon.UseEnergy) > 0)
					{
						MainWeapon.ThisObject = this.gameObject;
						LastMainShot = currentTime;
						MainWeapon.shoot();
						ShipPower -= (MainWeapon.UseEnergy);
					}
				} 
			}
			else 
			{
				if (currentTime - LastMainShot > MainWeapon.FireRate )
				{
					if (ShipPower - (MainWeapon.UseEnergy * Time.deltaTime) > 0)
					{
						MainWeapon.ThisObject = this.gameObject;
						MainWeapon.shoot_Beam();
						LastMainShot = currentTime;
						ShipPower -= (MainWeapon.UseEnergy * Time.deltaTime);
						if(this.GetComponent<AudioSource>().isPlaying)
						{
							this.GetComponent<AudioSource>().clip = MainWeapon.WeaponSound; this.GetComponent<AudioSource>().Play();
						}
					}
				}
			}
		}
		else
		{
			if (MainWeapon != null && MainWeapon.IsBeam)
			{
				if(this.GetComponent<AudioSource>().isPlaying)
				{
					this.GetComponent<AudioSource>().Stop();
				}
			}
		}

		if (IsWingShoot && WingWeapon != null)
		{ 
			if (WingWeapon.IsBeam == false)
			{
				if (currentTime - LastWingShot > WingWeapon.FireRate)
				{
					
					if (ShipPower - (WingWeapon.UseEnergy) > 0)
					{
						WingWeapon.ThisObject = this.gameObject;
						LastWingShot = currentTime;
						WingWeapon.shoot();
						ShipPower -= (WingWeapon.UseEnergy);
					}
				} 
			}
			else 
			{
				if (ShipPower - (WingWeapon.UseEnergy * Time.deltaTime) > 0)
				{
					if (currentTime - LastWingShot > WingWeapon.FireRate)
					{
						WingWeapon.ThisObject = this.gameObject;
						WingWeapon.shoot_Beam();
						LastMainShot = currentTime;
						ShipPower -= (WingWeapon.UseEnergy * Time.deltaTime);
						if(this.GetComponent<AudioSource>().isPlaying)
						{
							this.GetComponent<AudioSource>().clip = WingWeapon.WeaponSound; this.GetComponent<AudioSource>().Play();
						}
					}
				}
			}
		}
		else
		{
			if (WingWeapon != null && WingWeapon.IsBeam)
			{
				if(this.GetComponent<AudioSource>().isPlaying)
				{
					this.GetComponent<AudioSource>().Stop();
				}
			}
		}
	}

	void UpdateStats()
	{
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
			ShipHull = 0;
			ShipExplode();
		}
	}

	void ShipExplode()
	{
		GameObject.Find ("Level_Controller").GetComponent<Level_Controller> ().score += ScoreValue;
		GameObject.Find ("Level_Controller").GetComponent<Level_Controller> ().fodderkill += 1;
		if (Explosion != null)
		{
			GameObject Explosion_Spawned = (GameObject)Instantiate(Explosion);
			Explosion_Spawned.transform.position = this.transform.position;
		}
		this.ShipHull = 0;
		this.Vulnerable = false;
		this.GetComponent<TimeDelayDestroy>().isActive = true;
		this.ActiveMainColor = Color.clear;
		this.ActiveSecondaryColor = Color.clear;
		this.ActiveCockpitColor = Color.clear;
		this.transform.Find ("Main").GetComponent<SpriteRenderer> ().sprite = null;
		this.transform.Find ("Second").GetComponent<SpriteRenderer> ().sprite = null;
		this.transform.Find ("Cockpit").GetComponent<SpriteRenderer> ().sprite = null;
		this.isAlive = false;
		Destroy (this);
	}



	void MoveShip()
	{

		Vector3 OldPos = this.transform.position;
		OldPos.x = OldPos.x;
		if (currentTime > delay)
		{
			Vector3 StartyPosy = new Vector3 (idleKey.GetPosition (StartPos, delay).x + StartPos.x, idleKey.GetPosition (StartPos, delay).y + StartPos.y);
			this.transform.position = MovementKey.GetPosition (StartyPosy, currentTime - delay);
		}
		else
		{
			this.transform.position = idleKey.GetPosition (StartPos, currentTime);
		}
		Vector3 XMove = new Vector3((Player.transform.position.x - xMid) * xbaseMove,0,0);

		this.transform.position -= XMove;
		RotateShip (OldPos + oldXMove, transform.position + XMove);
		oldXMove = XMove;


	

	}

	public void RotateShip(Vector3 OldLoc , Vector3 NewLoc)
	{
		RotateShip(OldLoc , NewLoc , Vector3.zero);
	}

	public void RotateShip(Vector3 OldLoc , Vector3 NewLoc , Vector3 movement)
	{
		float ShipRotation = ((OldLoc.x - NewLoc.x) / 2f) / Time.deltaTime;
		int NewSprite = 2;
		if (ShipRotation < -1.5) 
		{
			NewSprite = 3;
		}
		if (ShipRotation < -3) 
		{
			NewSprite = 4;
		}
		if (ShipRotation > 1.5) 
		{
			NewSprite = 1;
		}
		if (ShipRotation > 3) 
		{
			NewSprite = 0;
		}
		this.transform.Find ("Main").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteSet[NewSprite];
		this.transform.Find ("Second").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteSecondSet[NewSprite];
		this.transform.Find ("Cockpit").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteCockpitSet[NewSprite];
	
	}

	void ShootGuns()
	{
		IsMainShoot = true;
		IsWingShoot= true;
	}
}