using UnityEngine;
using System.Collections;

public class StateCollector : MonoBehaviour 
{
	public float collectRate = 0.5f;
	public float collectDelay = 2f;

	int sampleNumber = 0;
	int gunCount = 0;
	float timeStamp = 0;
	public float firerate;
	public GameObject Projectile;
	public ShipController ship;
	public GameObject target;
	private GameObject trackProj = null;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		timeStamp += Time.deltaTime;
		if ((timeStamp - collectDelay) / collectRate > sampleNumber)
		{
			sampleNumber++;
			TakeSample ();
		}
		if (timeStamp / firerate > gunCount)
		{
			gunCount++;
			Shoot ();
		}
	}

	void Shoot()
	{	
		GameObject Projectile_Spawned = (GameObject)Instantiate(Projectile);

		Projectile_Spawned.GetComponent<ProjectileController>().WeaponLife = 1f;
		Projectile_Spawned.GetComponent<ProjectileController>().WeaponHeat = 5f;
		Projectile_Spawned.GetComponent<ProjectileController>().WeaponPhysicalDamage = 5f;
		Projectile_Spawned.GetComponent<ProjectileController>().WeaponEnergyDamage = 5f;
		Projectile_Spawned.GetComponent<ProjectileController>().WeaponSeek = 0f;
		Projectile_Spawned.GetComponent<ProjectileController>().staticSpeed = false;
		Projectile_Spawned.GetComponent<ProjectileController>().WeaponSpeed = 40f;
		Projectile_Spawned.GetComponent<ProjectileController>().IsFlash = false;
		Projectile_Spawned.layer = gameObject.layer;

		Projectile_Spawned.transform.position = new Vector3(transform.position.x , transform.position.y ,0);
		Projectile_Spawned.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,-40f,0) );
		trackProj = Projectile_Spawned;
	}

	void TakeSample()
	{
		NetState sample = new NetState();
		sample.inputs.healthRatio = ship.ShipHull/ship.Shiptype.ShipHull;
		sample.inputs.powerRatio = ship.ShipPower / 200f;
		sample.inputs.sheildRatio = ship.ShipSheild/ship.Sheildtype.MaxSheild;
		sample.inputs.tempRatio = ship.ShipHeat / ship.Shiptype.OverHeatThreshold;
		sample.inputs.xRatio = (ship.transform.position.x - ship.Bounds.x) / (ship.Bounds.z - ship.Bounds.x) + ((Random.value - .5f)/5f);
		sample.inputs.yRatio = (ship.transform.position.y - ship.Bounds.y) / (ship.Bounds.w - ship.Bounds.y) + ((Random.value - .5f)/5f);
		sample.inputs.xERatio = (this.transform.position.x - ship.Bounds.x) / (ship.Bounds.z - ship.Bounds.x);
		sample.inputs.yERatio = (this.transform.position.y - ship.Bounds.y) / (ship.Bounds.w - ship.Bounds.y);
		sample.inputs.xSRatio = 0f;
		sample.inputs.ySRatio = 0f;
		if (trackProj != null)
		{
			sample.inputs.xERatio = (trackProj.transform.position.x - ship.Bounds.x) / (ship.Bounds.z - ship.Bounds.x);
			sample.inputs.yERatio = (trackProj.transform.position.y - ship.Bounds.y) / (ship.Bounds.w - ship.Bounds.y);
		}

		sample.outputs.main = ship.IsMainShoot;
		sample.outputs.wing = ship.IsWingShoot;
		sample.outputs.xTargRatio = (ship.transform.position.x - ship.Bounds.x) / (ship.Bounds.z - ship.Bounds.x);
		sample.outputs.yTargRatio = (ship.transform.position.y - ship.Bounds.y) / (ship.Bounds.w - ship.Bounds.y);
		sample.saveState ("sample" + sampleNumber);
	}
}
