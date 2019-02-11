using UnityEngine;
using System.Collections;


public class TwinIce_1 : Flagship_AI 
{
	[HideInInspector]
	public float swapTimer = 0;
	[HideInInspector]
	public bool RunningRight;

	public override void AIUpdate()
	{
		if (Move_order == 1 || Move_order == 2)
		{
			WeaponFire[1] = true;
		}
		else
		{
			WeaponFire[1] = false;
		}
		if (Move_order == 3)
		{
			WeaponFire[0] = true;
		}
		else
		{
			WeaponFire[0] = false;
		}
		if (thisShip.ShipHeat < 0)
		{
			thisShip.ShipHeat = 0;
		}
	}

	public override void Move(float Speed)
	{
		Vector3 oldLoc = thisShip.gameObject.transform.position;
		if (Move_order == 0)
		{
			MoveTarget = new Vector2(-3.54f,10.25f);
		}
		else if (Move_order == 1  || Move_order == 2)
		{
			MoveTarget = new Vector2((Mathf.Sin(Move_timer)*-18.76f) - 3.54f , 10.25f);
		}
		else
		{
			//Hunt Script Start
			ShipController TargetShip = null;
			ShipController[] ShipControllers = FindObjectsOfType<ShipController>() as ShipController[];
			float LastClose = 100.0f;
			foreach (ShipController Ship in ShipControllers) 
			{
				if (Mathf.Abs(Ship.transform.position.x - thisShip.transform.position.x) < LastClose && Ship != thisShip && Ship.gameObject.layer != thisShip.gameObject.layer)
				{
					TargetShip = Ship as ShipController;
					LastClose = Mathf.Abs(Ship.transform.position.x - thisShip.transform.position.x);
				}
			}
			swapTimer += Time.deltaTime;
			Vector3 Target = TargetShip.transform.position;
			float YTarget = Target.y + (10);
			float XTarget = (Mathf.Sin(Move_timer * 2)*4f) + Target.x;

			MoveTarget = new Vector2 (XTarget, YTarget);
			//Hunt Script End
		}
		if (Move_order == 0)
		{
		thisShip.transform.position = Vector3.MoveTowards(thisShip.transform.position, new Vector3(MoveTarget.x,MoveTarget.y,0), Speed/3 * Time.deltaTime);
		}
		else
		{
			thisShip.transform.position = Vector3.MoveTowards(thisShip.transform.position, new Vector3(MoveTarget.x,MoveTarget.y,0), Speed/2 * Time.deltaTime);
		}
		thisShip.RotateShip (oldLoc , thisShip.gameObject.transform.position );
	}

}
