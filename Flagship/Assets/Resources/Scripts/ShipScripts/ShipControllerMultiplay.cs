using UnityEngine;
using System.Collections;

public class ShipControllerMultiplay : ShipController 
{

	void Start ()
	{
		if (GetComponent<NetworkView> ().isMine)
		{
			if (LoadCustom)
			{
		//		LoadShip ();
			}
		//	UpdateStats ();
		//	UpdateColor ();
			RotateShip (this.transform.position, this.transform.position);
			ActiveMainColor = MainColor;
			ActiveSecondaryColor = SecondaryColor;
			ActiveCockpitColor = CockpitColor;
			ShipHull = Shiptype.ShipHull;
			ShipSheild = Sheildtype.MaxSheild;
			ShipHeat = 0;
			ShipPower = 200;
		}

	}

	void Update () 
	{	
		currentTime += Time.deltaTime;
		if (isPlayer && isAlive)
		{
			//MoveShip ();
			//ShootGuns ();
		}
		if (isAlive) 
		{
			BonkThings ();
		//	UpdateStats ();
		//	UpdateColor ();
		//	RunWeapons ();
		}
	}
}
