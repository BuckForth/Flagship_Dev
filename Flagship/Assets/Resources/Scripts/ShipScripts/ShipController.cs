using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class ShipController : Entity 
{

	public bool isPlayer;
	public bool LoadCustom;
	public bool isFodder = false;
	public SheildType Sheildtype;
	public RadiatorType Radiatortype;
	public GeneratorType Generatortype;
	public WeaponType MainWeapon;
	public WeaponType WingWeapon;
	public Vector4 Bounds = new Vector4(-25,-13,11,13);

	public Color32 MainColor;
	public Color32 SecondaryColor;
	public Color32 CockpitColor;

	public string NameShip;

	[HideInInspector]
	public Color ActiveMainColor;
	[HideInInspector]
	public Color ActiveSecondaryColor;
	[HideInInspector]
	public Color ActiveCockpitColor;

	private float lastSynchronizationTime = 0f;
	private float syncDelay = 0f;
	private float syncTime = 0f;
	private Vector3 syncStartPosition = Vector3.zero;
	private Vector3 syncEndPosition = Vector3.zero;


	
	void Start ()
	{
		if (GetComponent<NetworkView> () == null)
		{
			if (LoadCustom)
			{
				LoadShip ();
			}
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

		}
		else
		{
			if (GetComponent<NetworkView> ().isMine)
			{
				if (LoadCustom)
				{
					LoadShip ();
				}

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
			}
			else
			{
				transform.Rotate (new Vector3 (-90f, 0f, 0f));
			}
		}
		MainWeapon = new WeaponType (MainWeapon);
		WingWeapon = new WeaponType (WingWeapon);
	}
	
	void Update () 
	{	
		currentTime += Time.deltaTime;
		if (GetComponent<NetworkView> () == null)
		{
			if (isPlayer && isAlive)
			{
				MoveShip ();
				ShootGuns ();
			}
			if (isAlive) 
			{
				BonkThings ();
				UpdateStats ();
				UpdateColor ();
				RunWeapons ();
			}
		}
		else
		{
			if (GetComponent<NetworkView> ().isMine)
			{
				if (isPlayer && isAlive)
				{
					MoveShip ();
					ShootGuns ();
				}
				if (isAlive) 
				{
					BonkThings ();
					UpdateStats ();
					UpdateColor ();
					RunWeapons ();
				}
			}
			else
			{
				if (isAlive) 
				{
					SyncedMovement ();
					UpdateStats ();
					UpdateColor ();
				}
			}
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
			if (MainWeapon.IsBeam && MainWeapon != null)
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
		if (isPlayer)
		{
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
		}
		else
		{
			Destroy(this.gameObject);
		}
	}

	void LoadShip()
	{
		if (File.Exists (Application.persistentDataPath + "/" + fileName)) 
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/" + fileName , FileMode.Open);
			PlayerShip LoadShip = (PlayerShip)bf.Deserialize(file);
			Shiptype = Resources.Load("ShipParts/ShipsTypes/" + LoadShip.shiptype) as ShipType;
			Sheildtype = Resources.Load("ShipParts/SheildTypes/" + LoadShip.sheildtype) as SheildType;
			Radiatortype = Resources.Load("ShipParts/RadiatorTypes/" + LoadShip.radiatortype) as RadiatorType;
			Generatortype = Resources.Load("ShipParts/GeneratorTypes/" + LoadShip.generatortype) as GeneratorType;
			MainWeapon = Resources.Load("ShipParts/WeaponTypes/" + LoadShip.mainweaponname) as WeaponType;
			WingWeapon = Resources.Load("ShipParts/WeaponTypes/" + LoadShip.wingweaponname) as WeaponType;
			MainColor = LoadShip.maincolor.GetColor();
			SecondaryColor = LoadShip.secondarycolor.GetColor();
			CockpitColor = LoadShip.cockpitcolor.GetColor();
			NameShip = LoadShip.shipname;
			file.Close();
			Debug.Log ("Load Complete");
		}
		else
		{
			Debug.Log ("No Saved File");
		}
	}
		
	void MoveShip()
	{
		Vector3 MousePos = Input.mousePosition;
		Vector3 ShipPos = this.transform.position;
		Vector3 OldPos = ShipPos;
		MousePos.z = this.transform.position.z - Camera.main.transform.position.z;
		MousePos = Camera.main.ScreenToWorldPoint (MousePos);
		float MoveX = Input.GetAxis ("Mouse X") * (ShipSpeed/40);
		float MoveY = Input.GetAxis ("Mouse Y") * (ShipSpeed/40);
		Vector3 NextPos = new Vector3(this.transform.position.x +(MoveX),this.transform.position.y + (MoveY) , 0);
		//this.transform.position = Vector3.MoveTowards(ShipPos, MousePos, ShipSpeed * Time.deltaTime);
		if (NextPos.x < Bounds.x)
		{
			NextPos.x = Bounds.x;
		}
		if (NextPos.y < Bounds.y)
		{
			NextPos.y = Bounds.y;
		}
		if (NextPos.x > Bounds.z)
		{
			NextPos.x = Bounds.z;
		}
		if (NextPos.y > Bounds.w)
		{
			NextPos.y = Bounds.w;
		}
		this.transform.position = NextPos;
		ShipPos = this.transform.position;
		RotateShip (OldPos , ShipPos);
	}

	public void RotateShip(Vector3 OldLoc , Vector3 NewLoc)
	{
		float ShipRotation = ((OldLoc.x - NewLoc.x) / 2.5f) / Time.deltaTime;
		int NewSprite = 2;
		if (ShipRotation < -3) 
		{
			NewSprite = 3;
		}
		if (ShipRotation < -9) 
		{
			NewSprite = 4;
		}
		if (ShipRotation > 3) 
		{
			NewSprite = 1;
		}
		if (ShipRotation > 9) 
		{
			NewSprite = 0;
		}
		this.transform.Find ("Main").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteSet[NewSprite];
		this.transform.Find ("Second").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteSecondSet[NewSprite];
		this.transform.Find ("Cockpit").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteCockpitSet[NewSprite];
	}

	void ShootGuns()
	{
		if (Input.GetButton ("Fire1")) 
		{
			IsMainShoot = true;
		}
		else
		{
			IsMainShoot = false;
		}

		if (Input.GetButton ("Fire2")) 
		{
			IsWingShoot= true;
		}
		else
		{
			IsWingShoot = false;
		}
	}

	private void SyncedMovement()
	{
		Vector3 oldPos = GetComponent<Rigidbody> ().position;
		syncTime += Time.deltaTime;
		GetComponent<Rigidbody>().position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
		RotateShip (oldPos,GetComponent<Rigidbody>().position);
	}

	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncMain = new Vector3 (MainColor.r / 255f, MainColor.g  / 255f, MainColor.b / 255f);
		Vector3 syncSecondary = new Vector3 (SecondaryColor.r / 255f, SecondaryColor.g / 255f , SecondaryColor.b / 255f);
		Vector3 syncCockpit = new Vector3 (CockpitColor.r / 255f, CockpitColor.g / 255f , CockpitColor.b / 255f);

		int syncShip = Shiptype.ItemID;
		int syncSheild = Sheildtype.ItemID;
		int syncRad = Radiatortype.ItemID;
		int syncGen = Generatortype.ItemID;
		int syncMainWeap = MainWeapon.ItemID;
		int syncWingWeap = WingWeapon.ItemID;

		if (stream.isWriting)
		{
			stream.Serialize(ref syncMain);
			stream.Serialize(ref syncSecondary);
			stream.Serialize(ref syncCockpit);

			stream.Serialize(ref syncShip);
			stream.Serialize(ref syncSheild);
			stream.Serialize(ref syncRad);
			stream.Serialize(ref syncGen);
			stream.Serialize(ref syncMainWeap);
			stream.Serialize(ref syncWingWeap);



			syncPosition = GetComponent<Rigidbody>().position;
			stream.Serialize(ref syncPosition);


		}
		else
		{
			ItemIDNET targetlist = Resources.Load ("Scripts/FrameWork/MasterID") as ItemIDNET;

			stream.Serialize(ref syncMain);
			stream.Serialize(ref syncSecondary);
			stream.Serialize(ref syncCockpit);
			MainColor = new Color(syncMain.x, syncMain.y, syncMain.z, 1f);
			SecondaryColor = new Color(syncSecondary.x, syncSecondary.y, syncSecondary.z, 1f);
			CockpitColor = new Color(syncCockpit.x, syncCockpit.y, syncCockpit.z, 1f);

			stream.Serialize(ref syncShip);
			stream.Serialize(ref syncSheild);
			stream.Serialize(ref syncRad);
			stream.Serialize(ref syncGen);
			stream.Serialize(ref syncMainWeap);
			stream.Serialize(ref syncWingWeap);

			Shiptype = targetlist.ShipID [syncShip];
			Sheildtype = targetlist.SheildID [syncSheild];
			Radiatortype = targetlist.RadiatorID [syncRad];
			Generatortype = targetlist.GeneratorID [syncGen];
			MainWeapon = targetlist.WeaponID [syncMainWeap];
			WingWeapon = targetlist.WeaponID [syncWingWeap];

			stream.Serialize(ref syncPosition);
			syncTime = 0f;
			syncDelay = Time.time - lastSynchronizationTime;
			lastSynchronizationTime = Time.time;
			syncEndPosition = syncPosition;
			syncStartPosition = GetComponent<Rigidbody>().position;

		}
	}
		

}