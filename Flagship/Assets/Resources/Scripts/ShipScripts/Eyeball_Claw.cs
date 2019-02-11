using UnityEngine;
using System.Collections;

public class Eyeball_Claw	: Entity 
{
	public Color darkened;
	public float distance;
	public float speed;
	public float offset;
	public float destroyDelay;
	public DreadnoughtController_Eye eyeBall;
	private float startX;
	private float personalTime = 0;


	void Start () 
	{
		startX = transform.position.x;
		ShipHull = Shiptype.ShipHull;
	}

	public float getTime()
	{
		return personalTime;
	}

	void Update () 
	{

		if (isAlive)
		{
			if (ShipHull < 0)//Handle Hull THIS IS ALWAYS LAST
			{
				ShipHull = 0;
				ShipExplode ();
			}

			personalTime += Time.deltaTime;
			transform.position = new Vector3 (startX + (distance * Mathf.Sin (((speed * personalTime) / Mathf.PI) + (offset / Mathf.PI))), transform.position.y);
		}
	}


	public override void DamageEntity (float HeatDamage , float EnergyDamage , float PhysicalDamage)
	{
		if (Vulnerable)
		{
			this.ShipHull -= PhysicalDamage + (EnergyDamage / 2f);
			eyeBall.DamageEntity (HeatDamage/4f, EnergyDamage/4f, PhysicalDamage/4f);
		}
		else
		{
			Debug.Log("Tink!");
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
		transform.GetComponent<SpriteRenderer> ().color = darkened;
		this.isAlive = false;
		Destroy (this);
	}
}
