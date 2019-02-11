using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class Flagship_AI : MonoBehaviour 
	{	
	public GameObject ExplosionObj;

	[HideInInspector]
	public FlagshipController thisShip;

	[HideInInspector]
	public int Move_order = 0;

	[HideInInspector]
	public Vector2 MoveTarget;

	[HideInInspector]
	public float Move_timer = 0;

	[HideInInspector]
	public bool[] WeaponFire;

	[HideInInspector]
	public float[] LastWeaponFire;

	public float[] MoveTimes;

	public int MoveRepeatFrom = 0;
	public int MoveVunerableFrom = -999;

	public WeaponType[] WeaponList;

	void Start () 
	{
		thisShip = this.GetComponentInParent<FlagshipController> ();
		WeaponFire = new bool[WeaponList.Length];
		LastWeaponFire = new float[WeaponList.Length];
	}


	void Update () 
	{	

		if (thisShip.ShipHeat < thisShip.Shiptype.FreezeThreshold)
		{
			Move_timer += Time.deltaTime / 5;
		}
		else
		{
			Move_timer += Time.deltaTime;
		}

		if (Move_timer > MoveTimes[Move_order])
		{
			Move_timer = 0;
			Move_order++;
			if (Move_order == MoveTimes.Length)
			{
				Move_order = MoveRepeatFrom;
			}
		}

		RunWeapons ();

	}

	void RunWeapons ()
	{
		int index = 0;
		foreach(bool GunBool in WeaponFire)
		{
			if (GunBool)
			{ 
				if (WeaponList[index].IsBeam == false)
				{
					if (thisShip.currentTime - LastWeaponFire[index] > WeaponList[index].FireRate)
					{
						WeaponList[index].ThisObject = this.gameObject;
						LastWeaponFire[index] = thisShip.currentTime;
						Shoot(index);
					} 
				}
				else 
				{
					if (thisShip.currentTime - LastWeaponFire[index] > WeaponList[index].FireRate )
					{
						WeaponList[index].ThisObject = this.gameObject;
						WeaponList[index].shoot_Beam();
						LastWeaponFire[index] = thisShip.currentTime;
						if(thisShip.GetComponent<AudioSource>().isPlaying)
						{
							thisShip.GetComponent<AudioSource>().clip = WeaponList[index].WeaponSound; this.GetComponent<AudioSource>().Play();
						}
					}
				}
			}
			else
			{
				if (WeaponList[index].IsBeam)
				{
					if(thisShip.GetComponent<AudioSource>().isPlaying)
					{
						thisShip.GetComponent<AudioSource>().Stop();
					}
				}
			}

			index++;
		}
	}

	public void Shoot(int Index)
	{
		LastWeaponFire[Index] = thisShip.currentTime;
		if (!WeaponList[Index].AlternateEmitPoints)
		{
			WeaponList[Index].indexemitt = 0;
			foreach (Vector3 Emitter in WeaponList[Index].EmitterPos) 
			{
				this.transform.GetComponent<AudioSource>().PlayOneShot(WeaponList[Index].WeaponSound , 0.5f);
				GameObject Projectile_Spawned = (GameObject)Instantiate(WeaponList[Index].Projectile);
				Projectile_Spawned.transform.position = new Vector3(this.transform.position.x + Emitter.x , this.transform.position.y + Emitter.y ,0);
				
				if (this.GetComponentInParent<Entity>().FaceDown)
				{
					Projectile_Spawned.transform.position = new Vector3(this.transform.position.x - Emitter.x , this.transform.position.y - Emitter.y ,0);
				}
				
				Projectile_Spawned.transform.eulerAngles = new Vector3( WeaponList[Index].EmitterRot[WeaponList[Index].indexemitt].x ,0 , this.transform.eulerAngles.x  +  WeaponList[Index].EmitterRot[WeaponList[Index].indexemitt].z + 90);
				Vector3 Spread = UnityEngine.Random.insideUnitSphere * WeaponList[Index].WeaponSpread;
				Spread.z = 0;
				Projectile_Spawned.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,WeaponList[Index].WeaponSpeed,0)  + Spread);
				Projectile_Spawned.GetComponent<ProjectileController>().WeaponLife = WeaponList[Index].WeaponLife;
				Projectile_Spawned.GetComponent<ProjectileController>().WeaponHeat = WeaponList[Index].WeaponHeat;
				Projectile_Spawned.GetComponent<ProjectileController>().WeaponPhysicalDamage = WeaponList[Index].WeaponPhysicalDamage;
				Projectile_Spawned.GetComponent<ProjectileController>().WeaponEnergyDamage = WeaponList[Index].WeaponEnergyDamage;
				Projectile_Spawned.GetComponent<ProjectileController>().WeaponSeek = WeaponList[Index].WeaponSeek;
				Projectile_Spawned.GetComponent<ProjectileController>().IsFlash = WeaponList[Index].IsFlashWeapon;
				Projectile_Spawned.layer = this.gameObject.layer;
				WeaponList[Index].indexemitt += 1;
			}
		}
		
		else ///Alternate Emitter
		{
			if (WeaponList[Index].indexemitt > (WeaponList[Index].EmitterPos.Length - 1))
			{
				WeaponList[Index].indexemitt = 0;
			}
			this.transform.GetComponent<AudioSource>().PlayOneShot(WeaponList[Index].WeaponSound , 0.5f);
			GameObject Projectile_Spawned = (GameObject)Instantiate(WeaponList[Index].Projectile);
			Projectile_Spawned.transform.position = new Vector3(this.transform.position.x + WeaponList[Index].EmitterPos[WeaponList[Index].indexemitt].x , this.transform.position.y + WeaponList[Index].EmitterPos[WeaponList[Index].indexemitt].y ,0);
			
			
			if (this.GetComponentInParent<Entity>().FaceDown)
			{
				Projectile_Spawned.transform.position = new Vector3(this.transform.position.x - WeaponList[Index].EmitterPos[WeaponList[Index].indexemitt].x , this.transform.position.y - WeaponList[Index].EmitterPos[WeaponList[Index].indexemitt].y ,0);
			}
			
			Projectile_Spawned.transform.eulerAngles = new Vector3(WeaponList[Index].EmitterRot[WeaponList[Index].indexemitt].x ,0 ,  this.transform.eulerAngles.x + WeaponList[Index].EmitterRot[WeaponList[Index].indexemitt].z + 90);
			Vector3 Spread = UnityEngine.Random.insideUnitSphere * WeaponList[Index].WeaponSpread;
			Spread.z = 0;
			Projectile_Spawned.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,WeaponList[Index].WeaponSpeed,0)  + Spread);
			Projectile_Spawned.GetComponent<ProjectileController>().WeaponLife = WeaponList[Index].WeaponLife;
			Projectile_Spawned.GetComponent<ProjectileController>().WeaponHeat = WeaponList[Index].WeaponHeat;
			Projectile_Spawned.GetComponent<ProjectileController>().WeaponPhysicalDamage = WeaponList[Index].WeaponPhysicalDamage;
			Projectile_Spawned.GetComponent<ProjectileController>().WeaponEnergyDamage = WeaponList[Index].WeaponEnergyDamage;
			Projectile_Spawned.GetComponent<ProjectileController>().WeaponSeek = WeaponList[Index].WeaponSeek;
			Projectile_Spawned.GetComponent<ProjectileController>().IsFlash = WeaponList[Index].IsFlashWeapon;
			Projectile_Spawned.layer = this.gameObject.layer;
			WeaponList[Index].indexemitt += 1;
		}
	}//ShootEnds

	public virtual void AIUpdate()
	{
		
	}

	public virtual void Move(float Speed)
	{
		
	}

}
