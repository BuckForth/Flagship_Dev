using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour {

	public bool IsMain = true;
	public bool IsBeam = false;
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
	[HideInInspector]
	public float LastShotTime = -100.0f;
	[HideInInspector]
	public float currentTime = 0.0f;
	[HideInInspector]
	public bool IsShooting = false;
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		currentTime += Time.deltaTime;
		if (IsShooting)
		{ 
			if (IsBeam == false)
			{
				if (currentTime - LastShotTime > FireRate)
				{
					if (this.GetComponentInParent<ShipController>().ShipPower - (UseEnergy) > 0)
					{
						shoot();
						this.GetComponentInParent<ShipController>().ShipPower -= (UseEnergy);
					}
				} 
			}
			else 
			{
				if (this.GetComponentInParent<ShipController>().ShipPower - (UseEnergy * Time.deltaTime) > 0)
				{
					shoot_Beam();
					this.GetComponentInParent<ShipController>().ShipPower -= (UseEnergy * Time.deltaTime);
					if(!this.transform.GetComponent<AudioSource>().isPlaying)
					{
						this.transform.GetComponent<AudioSource>().clip = WeaponSound; this.transform.GetComponent<AudioSource>().Play();
					}
				}
			}
		}
		else
		{
			if (IsBeam)
			{
				if(this.transform.GetComponent<AudioSource>().isPlaying)
				{
					this.transform.GetComponent<AudioSource>().Stop();
				}
			}
		}
	}

	void shoot()
	{
		LastShotTime = currentTime;
		Component[] Emitters = GetComponentsInChildren<EmitterScript>() as Component[];
		foreach (Component Emitter in Emitters) 
		{

			this.transform.GetComponent<AudioSource>().PlayOneShot(WeaponSound , 0.5f);
			GameObject Projectile_Spawned = (GameObject)Instantiate(Projectile);
			Projectile_Spawned.transform.position = new Vector3(Emitter.transform.position.x ,Emitter.transform.position.y ,0);
			Projectile_Spawned.transform.eulerAngles = new Vector3(Emitter.transform.eulerAngles.x ,0 ,Emitter.transform.eulerAngles.z );
			Vector3 Spread = Random.insideUnitSphere * WeaponSpread;
			Spread.z = 0;
			Projectile_Spawned.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,WeaponSpeed,0)  + Spread);
			Projectile_Spawned.GetComponent<ProjectileController>().WeaponLife = WeaponLife;
			Projectile_Spawned.GetComponent<ProjectileController>().WeaponHeat = WeaponHeat;
			Projectile_Spawned.GetComponent<ProjectileController>().WeaponPhysicalDamage = WeaponPhysicalDamage;
			Projectile_Spawned.GetComponent<ProjectileController>().WeaponEnergyDamage = WeaponEnergyDamage;
			Projectile_Spawned.GetComponent<ProjectileController>().WeaponSeek = WeaponSeek;
			Projectile_Spawned.layer = this.gameObject.layer;
			if (this.GetComponentInParent<ShipController>().ShipHeat + UseHeat > 200)
			{
				this.GetComponentInParent<ShipController>().ShipHeat = 200;
			}
			else
			{
				this.GetComponentInParent<ShipController>().ShipHeat += UseHeat;
			}
		}
	}
	void shoot_Beam()
	{

		Component[] Emitters = GetComponentsInChildren<EmitterScript>() as Component[];
		foreach (Component Emitter in Emitters) 
		{
			float Shot_Dist = 0;
			while (Shot_Dist < WeaponSpeed)
			{
				GameObject Projectile_Spawned = (GameObject)Instantiate(Projectile);
				Projectile_Spawned.transform.position = new Vector3(Emitter.transform.position.x ,Emitter.transform.position.y + Shot_Dist,0);
				Projectile_Spawned.transform.eulerAngles = new Vector3(Emitter.transform.eulerAngles.x ,0 ,Emitter.transform.eulerAngles.z );
				Projectile_Spawned.GetComponent<ProjectileController>().WeaponLife = WeaponLife;
				Projectile_Spawned.GetComponent<ProjectileController>().WeaponHeat = WeaponHeat* Time.deltaTime;
				Projectile_Spawned.GetComponent<ProjectileController>().WeaponPhysicalDamage = WeaponPhysicalDamage * Time.deltaTime;
				Projectile_Spawned.GetComponent<ProjectileController>().WeaponEnergyDamage = WeaponEnergyDamage * Time.deltaTime;
				Projectile_Spawned.layer = this.gameObject.layer;
				Shot_Dist += 1;
			}
			if (this.GetComponentInParent<ShipController>().ShipHeat + (UseHeat * Time.deltaTime) > 200)
			{
				this.GetComponentInParent<ShipController>().ShipHeat = 200;
			}
			else
			{
				this.GetComponentInParent<ShipController>().ShipHeat += (UseHeat * Time.deltaTime);
			}
		}
	}

}
