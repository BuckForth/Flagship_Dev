using UnityEngine;
using System.Collections;


public class Eye_2 : Dreadnought_AI 
{

	public override void AIUpdate()
	{
		if (this.transform.position != new Vector3(MoveTarget.x,MoveTarget.y,0) && Move_order < 5 && Move_order != 0)
		{
		//	WeaponFire[1] = true;
		}
		else if (this.transform.position == new Vector3(MoveTarget.x,MoveTarget.y,0) && Move_order < 5)
		{
		//	WeaponFire[1] = false;
		}

		if (Move_order == 5)
		{
		//	WeaponFire[0] = true;
		}
		else
		{
		//	WeaponFire[0] = false;
		}
	}

	public override void Move(float Speed)
	{
		if (Move_order == 0)
		{
			MoveTarget = new Vector2 ((Mathf.Sin (Move_timer * 12) * 1f) - 3.54f, (Mathf.Sin (Move_timer * 12 + (Mathf.PI / 2f)) * 1f) + 8.25f);
		}
		else if (Move_order == 1 || Move_order == 4)
		{
			//Hunt Script Start
			ShipController TargetShip = null;
			ShipController[] ShipControllers = FindObjectsOfType<ShipController> () as ShipController[];
			float LastClose = 100.0f;
			foreach (ShipController Ship in ShipControllers)
			{
				if (Mathf.Abs (Ship.transform.position.x - thisShip.transform.position.x) < LastClose && Ship != thisShip && Ship.gameObject.layer != thisShip.gameObject.layer)
				{
					TargetShip = Ship as ShipController;
					LastClose = Mathf.Abs (Ship.transform.position.x - thisShip.transform.position.x);
				}
			}
			Vector3 Target = TargetShip.transform.position;
			float YTarget = Target.y + (15);
			float XTarget = Target.x;
			XTarget += Mathf.Sin (Move_timer / Mathf.PI * 20) * 5f;
			if (YTarget < thisShip.Bounds.y)
			{
				YTarget = thisShip.Bounds.y;
			}
			if (YTarget > thisShip.Bounds.w)
			{
				YTarget = thisShip.Bounds.w;
			}
			MoveTarget = new Vector2 (XTarget, YTarget);
			//Hunt Script End
		}
		else if (Move_order == 3)
		{
			MoveTarget = new Vector2 (-20f, 8.25f);
		}
		else if (Move_order == 6)
		{
			MoveTarget = new Vector2 (13.4f , 8.25f);
		}

		if (Move_order == 3 || Move_order == 6)
		{
			thisShip.transform.position = Vector3.MoveTowards (thisShip.transform.position, new Vector3 (MoveTarget.x, MoveTarget.y, 0), (Speed / 1.5f) * Time.deltaTime);
		}
		else if (Move_order == 2 || Move_order == 5)
		{
			// Don't Move
		}
		else
		{
			thisShip.transform.position = Vector3.MoveTowards(thisShip.transform.position, new Vector3(MoveTarget.x,MoveTarget.y,0), Speed * Time.deltaTime);
		}
	}

}
