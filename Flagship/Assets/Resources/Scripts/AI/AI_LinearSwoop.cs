using UnityEngine;
using System.Collections;

public class AI_LinearSwoop : MonoBehaviour 
{
	public float TriggerY = 0.5f;
	public float Xspeed = 0.5f;
	public float Yspeed = 0.5f;
	public bool Shoots = false;
	[HideInInspector]
	public bool IsActive = false;
	[HideInInspector]
	public ShipController ThisShip;
	
	// Use this for initialization
	void Start () 
	{
		ThisShip = this.GetComponent<ShipController>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 Oldpos = this.transform.position;

		if (Oldpos.y < TriggerY && !IsActive)
		{
			IsActive = true;
		}

		if (IsActive) 
		{

			ShootMainGuns ();
			this.transform.position = new Vector3(Oldpos.x + (Time.deltaTime * Xspeed) ,Oldpos.y + (Time.deltaTime * -Yspeed) ,0f);
			ThisShip.RotateShip(Oldpos, this.transform.position);
		}
		if (this.transform.position.y < -15)
		{
			Destroy(this.gameObject);
		}

	}

	void ShootMainGuns()
	{
		if (Shoots) 
		{
			this.GetComponent<ShipController>().IsMainShoot = true;
		}
		else
		{
			this.GetComponent<ShipController>().IsMainShoot = false;
		}
	}

	
}
