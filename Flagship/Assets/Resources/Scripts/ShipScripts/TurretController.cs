using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class TurretController : Entity 
{
	
	public WeaponType MainWeapon;
	public float LifeSpan;
	public float ShootDelay;

	private bool playingAnim = false;
	private float timeShot = 0;
	
	void Start () 
	{
		bonkable = false;
		UpdateStats ();
		ShipHull = Shiptype.ShipHull;
		ShipHeat = 0;
		ShipPower = 200;
		if (MainWeapon != null)
		{
			MainWeapon = new WeaponType (MainWeapon);
		}
	}
	
	void Update () 
	{	
		if (transform.position.y < 15 && transform.position.y > -15)
		{
			currentTime += Time.deltaTime;
			if (isAlive)
			{
				if (currentTime > ShootDelay)
				{
					ShootGuns ();
				}
				if (playingAnim)
				{
					int dex = (int)(((currentTime - timeShot) * 16) % 3 + 1);
					if ((int)((currentTime - timeShot) * 16) >= 3)
					{
						playingAnim = false;
						this.transform.Find ("Main").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteSet [0];
						this.transform.Find ("Second").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteSecondSet [0];
						this.transform.Find ("Detail").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteCockpitSet [0];
					}
					else
					{
						this.transform.Find ("Main").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteSet [dex];
						this.transform.Find ("Second").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteSecondSet [dex];
						this.transform.Find ("Detail").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteCockpitSet [dex];
					}
				}
				UpdateStats ();
				RunWeapons ();
			}
		}
	}
		

	void RunWeapons ()
	{
		if (IsMainShoot && MainWeapon != null)
		{ 
			if (currentTime - LastMainShot > MainWeapon.FireRate)
			{
					
				if (ShipPower - (MainWeapon.UseEnergy) > 0)
				{
					MainWeapon.ThisObject = this.gameObject;
					LastMainShot = currentTime;
					timeShot = currentTime;
					MainWeapon.shoot ();
					ShipPower -= (MainWeapon.UseEnergy);

					playingAnim = true;
				}
			} 
		}
	}

	void UpdateStats()
	{
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
		this.transform.Find ("Main").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteSet[4];
		this.transform.Find ("Second").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteSecondSet[4];
		this.transform.Find ("Detail").GetComponent<SpriteRenderer> ().sprite = Shiptype.SpriteCockpitSet[4];
		this.isAlive = false;
		Destroy (this);
	}
		
	void ShootGuns()
	{
		IsMainShoot = true;
	}
}