using UnityEngine;
using System.Collections;


public class WeaponDreadnoughtType : ScriptableObject
{
	public string Name;
	public int ItemID;

	public bool IsBeam = false;
	public bool IsFlashWeapon = false;
	public bool AlternateEmitPoints = false;
	public AudioClip WeaponSound; 
	public float FireRate = 0.0f;
	public float UseEnergy = 0.0f;
	public float UseHeat = 0.0f;
	public float WeaponSeek = 0.0f;
	public float WeaponLife = 0.0f;
	public float WeaponHeat = 0.0f;
	public float WeaponSpeed = 50.0f;
	public float WeaponPhysicalDamage = 0.0f;
	public float WeaponEnergyDamage = 0.0f;
	public float WeaponSpread = 0.0f;
	public GameObject Projectile;
	public string[] Decription;
	public Vector3[] EmitterPos;
	public Vector3[] EmitterRot;

	[HideInInspector]
	public float LastShotTime = -100.0f;
	[HideInInspector]
	public GameObject ThisObject;
	[HideInInspector]
	public float currentTime = 0.0f;
	[HideInInspector]
	public bool IsShooting = false;
	[HideInInspector]
	int indexemitt = 0;

	public WeaponDreadnoughtType(WeaponType Oldtype)
	{
		Name = Oldtype.Name;
		ItemID = Oldtype.ItemID;
		
		IsBeam = Oldtype.IsBeam;
		IsFlashWeapon = Oldtype.IsFlashWeapon;
		AlternateEmitPoints = Oldtype.AlternateEmitPoints;
		WeaponSound = Oldtype.WeaponSound; 
		FireRate = Oldtype.FireRate;
		UseEnergy = Oldtype.UseEnergy;
		UseHeat = Oldtype.UseHeat;
		WeaponSeek = Oldtype.WeaponSeek;
		WeaponLife = Oldtype.WeaponLife;
		WeaponHeat = Oldtype.WeaponHeat;
		WeaponSpeed = Oldtype.WeaponSpeed;
		WeaponPhysicalDamage = Oldtype.WeaponPhysicalDamage;
		WeaponEnergyDamage = Oldtype.WeaponEnergyDamage;
		WeaponSpread = Oldtype.WeaponSpread;
		Projectile = Oldtype.Projectile;
		Decription = Oldtype.Decription;
		EmitterPos = Oldtype.EmitterPos;
		EmitterRot = Oldtype.EmitterRot;


		LastShotTime = -100.0f;

		currentTime = 0.0f;
		IsShooting = false;
		indexemitt = 0;

	}

	void Start () 
	{
		ShipController[] ObjectsWithWeapon = GameObject.FindObjectsOfType<ShipController>() as ShipController[];
		foreach (ShipController ObjectWithWeapon in ObjectsWithWeapon) 
		{
			if (ObjectWithWeapon.GetComponent<ShipController>().MainWeapon == this || ObjectWithWeapon.GetComponent<ShipController>().WingWeapon == this )
			{
				ThisObject = ObjectWithWeapon.gameObject;
			}
		}

		Dreadnought_AI[] BossWithWeapon = GameObject.FindObjectsOfType<Dreadnought_AI>() as Dreadnought_AI[];
		foreach (Dreadnought_AI BossWeapon in BossWithWeapon) 
		{
			for (int ii = 0; ii < BossWeapon.GetComponent<Dreadnought_AI>().WeaponList.Length; ii++)
			{
				if (BossWeapon.GetComponent<Dreadnought_AI>().WeaponList[ii] == this)
				{
					ThisObject = BossWeapon.gameObject;
				}
			}
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
		currentTime += Time.deltaTime;
		if (IsShooting)
		{ 
			Debug.Log("IsShootingTriggered");
			if (IsBeam == false)
			{
				if (currentTime - LastShotTime > FireRate)
				{

					if (ThisObject.GetComponentInParent<ShipController>().ShipPower - (UseEnergy) > 0)
					{

						shoot();
						ThisObject.GetComponentInParent<ShipController>().ShipPower -= (UseEnergy);
					}
				} 
			}
			else 
			{
				if (currentTime - LastShotTime > FireRate)
				{
					if (ThisObject.GetComponentInParent<ShipController>().ShipPower - (UseEnergy * Time.deltaTime) > 0)
					{
						shoot_Beam();
						ThisObject.GetComponentInParent<ShipController>().ShipPower -= (UseEnergy * Time.deltaTime);
						if(!ThisObject.transform.GetComponent<AudioSource>().isPlaying)
						{
						ThisObject.transform.GetComponent<AudioSource>().clip = WeaponSound; ThisObject.transform.GetComponent<AudioSource>().Play();
						}
					}
			}
			}
		}
		else
		{
			if (IsBeam)
			{
				if(ThisObject.transform.GetComponent<AudioSource>().isPlaying)
				{
					ThisObject.transform.GetComponent<AudioSource>().Stop();
				}
			}
		}
	}
	
	public void shoot()
	{
		LastShotTime = currentTime;
		if (!AlternateEmitPoints)
		{
			this.indexemitt = 0;
			foreach (Vector3 Emitter in EmitterPos) 
			{
				ThisObject.transform.GetComponent<AudioSource>().PlayOneShot(WeaponSound , 0.5f);
				GameObject Projectile_Spawned = (GameObject)Instantiate(Projectile);
				Projectile_Spawned.transform.position = new Vector3(ThisObject.transform.position.x + Emitter.x , ThisObject.transform.position.y + Emitter.y ,0);
				
				if (ThisObject.GetComponentInParent<Entity>().FaceDown)
				{
					Projectile_Spawned.transform.position = new Vector3(ThisObject.transform.position.x - Emitter.x , ThisObject.transform.position.y - Emitter.y ,0);
				}

				Projectile_Spawned.transform.eulerAngles = new Vector3( EmitterRot[this.indexemitt].x ,0 , ThisObject.transform.eulerAngles.x  +  EmitterRot[this.indexemitt].z + 90);
				Vector3 Spread = Random.insideUnitSphere * WeaponSpread;
				Spread.z = 0;
				Projectile_Spawned.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,WeaponSpeed,0)  + Spread);
				Projectile_Spawned.GetComponent<ProjectileController>().WeaponLife = WeaponLife;
				Projectile_Spawned.GetComponent<ProjectileController>().WeaponHeat = WeaponHeat;
				Projectile_Spawned.GetComponent<ProjectileController>().WeaponPhysicalDamage = WeaponPhysicalDamage;
				Projectile_Spawned.GetComponent<ProjectileController>().WeaponEnergyDamage = WeaponEnergyDamage;
				Projectile_Spawned.GetComponent<ProjectileController>().WeaponSeek = WeaponSeek;
				Projectile_Spawned.GetComponent<ProjectileController>().IsFlash = IsFlashWeapon;
				Projectile_Spawned.layer = ThisObject.gameObject.layer;
				if (ThisObject.GetComponentInParent<Entity>().ShipHeat + UseHeat > 200)
				{
					ThisObject.GetComponentInParent<Entity>().ShipHeat = 200;
				}
				else
				{
					ThisObject.GetComponentInParent<Entity>().ShipHeat += UseHeat;
				}
				this.indexemitt += 1;
			}
		}

		else ///Alternate Emitter
		{
			if (this.indexemitt > (EmitterPos.Length - 1))
			{
				this.indexemitt = 0;
			}
			ThisObject.transform.GetComponent<AudioSource>().PlayOneShot(WeaponSound , 0.5f);
			GameObject Projectile_Spawned = (GameObject)Instantiate(Projectile);
			Projectile_Spawned.transform.position = new Vector3(ThisObject.transform.position.x + EmitterPos[this.indexemitt].x , ThisObject.transform.position.y + EmitterPos[this.indexemitt].y ,0);

			
			if (ThisObject.GetComponent<ShipController>().FaceDown)
			{
				Projectile_Spawned.transform.position = new Vector3(ThisObject.transform.position.x - EmitterPos[indexemitt].x , ThisObject.transform.position.y - EmitterPos[this.indexemitt].y ,0);
			}

			Projectile_Spawned.transform.eulerAngles = new Vector3(EmitterRot[this.indexemitt].x ,0 ,  ThisObject.transform.eulerAngles.x + EmitterRot[this.indexemitt].z + 90);
			Vector3 Spread = Random.insideUnitSphere * WeaponSpread;
			Spread.z = 0;
			Projectile_Spawned.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,WeaponSpeed,0)  + Spread);
			Projectile_Spawned.GetComponent<ProjectileController>().WeaponLife = WeaponLife;
			Projectile_Spawned.GetComponent<ProjectileController>().WeaponHeat = WeaponHeat;
			Projectile_Spawned.GetComponent<ProjectileController>().WeaponPhysicalDamage = WeaponPhysicalDamage;
			Projectile_Spawned.GetComponent<ProjectileController>().WeaponEnergyDamage = WeaponEnergyDamage;
			Projectile_Spawned.GetComponent<ProjectileController>().WeaponSeek = WeaponSeek;
			Projectile_Spawned.GetComponent<ProjectileController>().IsFlash = IsFlashWeapon;
			Projectile_Spawned.layer = ThisObject.gameObject.layer;
			if (ThisObject.GetComponentInParent<ShipController>().ShipHeat + UseHeat > 200)
			{
				ThisObject.GetComponentInParent<ShipController>().ShipHeat = 200;
			}
			else
			{
				ThisObject.GetComponentInParent<ShipController>().ShipHeat += UseHeat;
			}
			this.indexemitt += 1;
		}
	}

	public void shoot_Beam()
	{
		LastShotTime = currentTime;
		if (!AlternateEmitPoints)
		{
			this.indexemitt = 0;
			foreach (Vector3 Emitter in EmitterPos) 
			{
				float Shot_Dist = 0;
				while (Shot_Dist < WeaponSpeed)
				{
					GameObject Projectile_Spawned = (GameObject)Instantiate(Projectile);
					Projectile_Spawned.transform.position = new Vector3(ThisObject.transform.position.x + Emitter.x ,ThisObject.transform.position.y + Emitter.y + Shot_Dist,0);
					Projectile_Spawned.transform.eulerAngles = new Vector3(EmitterRot[this.indexemitt].x ,0 ,EmitterRot[this.indexemitt].z );
					Projectile_Spawned.GetComponent<ProjectileController>().WeaponLife = WeaponLife;
					Projectile_Spawned.GetComponent<ProjectileController>().WeaponHeat = WeaponHeat* Time.deltaTime;
					if (!IsFlashWeapon)
					{
						Projectile_Spawned.GetComponent<ProjectileController>().WeaponPhysicalDamage = WeaponPhysicalDamage * Time.deltaTime;
						Projectile_Spawned.GetComponent<ProjectileController>().WeaponEnergyDamage = WeaponEnergyDamage * Time.deltaTime;
					}
					else
					{
						Projectile_Spawned.GetComponent<ProjectileController>().WeaponPhysicalDamage = WeaponPhysicalDamage;
						Projectile_Spawned.GetComponent<ProjectileController>().WeaponEnergyDamage = WeaponEnergyDamage;
					}
					Projectile_Spawned.GetComponent<ProjectileController>().IsFlash = IsFlashWeapon;
					Projectile_Spawned.layer = ThisObject.gameObject.layer;
					Shot_Dist += 1;
				}
				if (ThisObject.GetComponentInParent<ShipController>().ShipHeat + (UseHeat * Time.deltaTime) > 200)
				{
					ThisObject.GetComponentInParent<ShipController>().ShipHeat = 200;
				}
				else
				{
					ThisObject.GetComponentInParent<ShipController>().ShipHeat += (UseHeat * Time.deltaTime);
				}
			}
		}
		else
		{
			if (this.indexemitt > (EmitterPos.Length - 1))
			{
				this.indexemitt = 0;
			}
			float Shot_Dist = 0;
			while (Shot_Dist < WeaponSpeed)
			{
				GameObject Projectile_Spawned = (GameObject)Instantiate(Projectile);
				Projectile_Spawned.transform.position = new Vector3(ThisObject.transform.position.x + EmitterPos[this.indexemitt].x ,ThisObject.transform.position.y + EmitterPos[this.indexemitt].y + Shot_Dist,0);
				Projectile_Spawned.transform.eulerAngles = new Vector3(EmitterRot[this.indexemitt].x ,0 ,EmitterRot[this.indexemitt].z );
				Projectile_Spawned.GetComponent<ProjectileController>().WeaponLife = WeaponLife;
				Projectile_Spawned.GetComponent<ProjectileController>().WeaponHeat = WeaponHeat* Time.deltaTime;
				if (!IsFlashWeapon)
				{
					Projectile_Spawned.GetComponent<ProjectileController>().WeaponPhysicalDamage = WeaponPhysicalDamage * Time.deltaTime;
					Projectile_Spawned.GetComponent<ProjectileController>().WeaponEnergyDamage = WeaponEnergyDamage * Time.deltaTime;
				}
				else
				{
					Projectile_Spawned.GetComponent<ProjectileController>().WeaponPhysicalDamage = WeaponPhysicalDamage;
					Projectile_Spawned.GetComponent<ProjectileController>().WeaponEnergyDamage = WeaponEnergyDamage;
				}
				Projectile_Spawned.GetComponent<ProjectileController>().IsFlash = IsFlashWeapon;
				Projectile_Spawned.layer = ThisObject.gameObject.layer;
				Shot_Dist += 1;
			}
			if (ThisObject.GetComponentInParent<ShipController>().ShipHeat + (UseHeat * Time.deltaTime) > 200)
			{
				ThisObject.GetComponentInParent<ShipController>().ShipHeat = 200;
			}
			else
			{
				ThisObject.GetComponentInParent<ShipController>().ShipHeat += (UseHeat * Time.deltaTime);
			}
			this.indexemitt += 1;
		}
	}

	
}
