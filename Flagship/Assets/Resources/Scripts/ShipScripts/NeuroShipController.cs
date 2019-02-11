using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class NeuroShipController : Entity 
{
	public SheildType Sheildtype;
	public RadiatorType Radiatortype;
	public GeneratorType Generatortype;
	public WeaponType MainWeapon;
	public WeaponType WingWeapon;

	public Vector4 Bounds = new Vector4(-25,-13,11,13);

	public Color32 MainColor;
	public Color32 SecondaryColor;
	public Color32 CockpitColor;

	public bool isTraining = true;

	public NeuroNet brain = new NeuroNet(10,8,4);
	public int hits = 0;
	[HideInInspector]
	public Vector3 StartPos;
	[HideInInspector]
	public Color ActiveMainColor;
	[HideInInspector]
	public Color ActiveSecondaryColor;
	[HideInInspector]
	public Color ActiveCockpitColor;

	public GameObject Player;
	public float xbaseMove = 0.05f;
	public float xMid = -3.4f;

	public float xLoc = 0;
	public float yLoc = 0;

	public bool isBottom = false;
	private Vector3 oldXMove = Vector3.zero;
	
	void Start () 
	{
		MainColor = this.transform.Find ("Main").GetComponent<SpriteRenderer> ().color;
		SecondaryColor = this.transform.Find ("Second").GetComponent<SpriteRenderer> ().color;
		CockpitColor = this.transform.Find ("Cockpit").GetComponent<SpriteRenderer> ().color;
		StartPos = this.transform.position;
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

	void BrainUpdate ()
	{
		float[] inputs = new float[10];
		inputs [0] = ShipHull / Shiptype.ShipHull;
		inputs [1] = ShipSheild / Sheildtype.MaxSheild;
		inputs [2] = ShipHeat / 200f;
		inputs [3] = ShipPower / 200f;
		inputs [4] = (transform.position.x - Bounds.x)/(Bounds.z - Bounds.x);
		inputs [5] = (transform.position.y - Bounds.y)/(Bounds.w - Bounds.y);
		inputs [6] = (Player.transform.position.x - Bounds.x)/(Bounds.x - Bounds.z);
		inputs [7] = (Player.transform.position.y - Bounds.y)/(Bounds.y - Bounds.w);

		ProjectileController[] projectiles = FindObjectsOfType<ProjectileController> ();
		float dist = float.MaxValue;
		GameObject closest = null;
		for (int ii = 0; ii < projectiles.Length; ii++)
		{
			if (projectiles [ii].gameObject.layer != this.gameObject.layer)
			{
				if (Math.Sqrt (Math.Pow (Math.Abs (transform.position.x - projectiles [ii].transform.position.x), 2d) + Math.Pow (Math.Abs (transform.position.y - projectiles [ii].transform.position.y), 2d)) > dist)
				{
					dist = (float)Math.Sqrt (Math.Pow (Math.Abs (transform.position.x - projectiles [ii].transform.position.x), 2d) + Math.Pow (Math.Abs (transform.position.y - projectiles [ii].transform.position.y), 2d));
					closest = projectiles [ii].gameObject;
				}
			}
		}
		inputs [8] = 0f;
		inputs [9] = 0f;

		if (closest != null)
		{
			inputs [8] = (closest.transform.position.x - Bounds.x)/(Bounds.x - Bounds.z);
			inputs [9] = (closest.transform.position.y - Bounds.y)/(Bounds.y - Bounds.w);
		}

		if (isBottom)
		{
			inputs [4] = 1f - inputs [4];
			inputs [5] = 1f - inputs [5];
			inputs [6] = 1f - inputs [6];
			inputs [7] = 1f - inputs [7];
			inputs [8] = 1f - inputs [8];
			inputs [9] = 1f - inputs [9];
		}

		brain.inputValues = inputs;
		brain.updateStep ();

		float[] outs = brain.outputLayer;

		xLoc = outs [0];
		yLoc = outs [1];
		//yLoc = 1f - yLoc;
		if (isBottom)
		{
			xLoc = -xLoc;
			yLoc = 1f - yLoc;
		}

		IsMainShoot = (outs[2]>0f);
		IsWingShoot = (outs[3]>0f);
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
		BrainUpdate ();
		MoveShip ();
		if (isAlive) 
		{
			BonkThings ();
			UpdateStats ();
			UpdateColor ();
			RunWeapons ();
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
		this.ShipHull = 0;
		this.Vulnerable = false;
		this.ActiveMainColor = Color.clear;
		this.ActiveSecondaryColor = Color.clear;
		this.ActiveCockpitColor = Color.clear;
		this.transform.Find ("Main").GetComponent<SpriteRenderer> ().sprite = null;
		this.transform.Find ("Second").GetComponent<SpriteRenderer> ().sprite = null;
		this.transform.Find ("Cockpit").GetComponent<SpriteRenderer> ().sprite = null;
		this.isAlive = false;
		GameObject.Find ("Trainer").GetComponent<TrainingBot> ().EndRound();
	}



	void MoveShip()
	{

		Vector3 OldPos = this.transform.position;
		float xPos = transform.position.x;
		float yPos = transform.position.y;
		//Movement stuff here
		float xtarg =(xLoc * (Bounds.z - Bounds.x)) + Bounds.x;

		Debug.Log (xLoc + " : " + xtarg);
		if (Math.Abs (transform.position.x - xtarg) > 0.0001f)
		{
			if (transform.position.x < xtarg)
			{
				if (Math.Abs(transform.position.x - xtarg) < (Shiptype.ShipSpeed * Time.deltaTime))
				{
					xPos = xtarg;
				}
				else
				{
					xPos += (Shiptype.ShipSpeed * Time.deltaTime);
				}
			}
			else
			{
				if (Math.Abs(transform.position.x - xtarg) < (Shiptype.ShipSpeed * Time.deltaTime))
				{
					xPos = xtarg;
				}
				else
				{
					xPos -= (Shiptype.ShipSpeed * Time.deltaTime);
				}
			}
		}
		float ytarg =(yLoc * (Bounds.w - Bounds.y)) + Bounds.y;
		if (Math.Abs (transform.position.y - ytarg) > 0.0001f)
		{
			if (transform.position.y < ytarg)
			{
				if (Math.Abs(transform.position.y - ytarg) < (Shiptype.ShipSpeed * Time.deltaTime))
				{
					yPos = ytarg;
				}
				else
				{
					yPos += (Shiptype.ShipSpeed * Time.deltaTime);
				}
			}
			else
			{
				if (Math.Abs(transform.position.y - ytarg) < (Shiptype.ShipSpeed * Time.deltaTime))
				{
					yPos = ytarg;
				}
				else
				{
					yPos -= (Shiptype.ShipSpeed * Time.deltaTime);
				}
			}
		}

		if (xPos < Bounds.x)
		{
			xPos = Bounds.x;
		}
		if (xPos > Bounds.z)
		{
			xPos = Bounds.z;
		}

		if (yPos < Bounds.y)
		{
			yPos = Bounds.y;
		}
		if (yPos > Bounds.w)
		{
			yPos = Bounds.w;
		}
		this.transform.position = new Vector3(xPos,yPos,transform.position.z);
		RotateShip (OldPos, transform.position);//RotateShip (OldPos + oldXMove, transform.position) + XMove);
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