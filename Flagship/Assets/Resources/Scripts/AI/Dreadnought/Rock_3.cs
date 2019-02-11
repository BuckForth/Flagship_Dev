using UnityEngine;
using System.Collections;


public class Rock_3 : Dreadnought_AI 
{
	[HideInInspector]
	public float swapTimer = 0;
	[HideInInspector]
	public bool RunningRight;
	
	public override void AIUpdate()
	{
		if (Move_order == 1 )
		{
			WeaponFire[0] = true;
		}
		else
		{
			WeaponFire[0] = false;
		}
		if (Move_order == 2)
		{
			WeaponFire[1] = true;
		}
		else
		{
			WeaponFire[1] = false;
		}
	}

	public override void Move(float Speed)
	{
		if (Move_order == 0 || Move_order == 2)
		{
			MoveTarget = new Vector2((Mathf.Sin(Move_timer * 16) * 3) - 3.54f ,(Mathf.Cos(Move_timer * 16) * 2) + 8.25f);
		}
		else if (Move_order == 1)
		{
			MoveTarget = new Vector2((Mathf.Sin(Move_timer* 2)*(22.76f) - 3.54f) , Mathf.Sin(Move_timer* 8f) + 10.25f);
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
			Vector3 Target = TargetShip.transform.position;
			float YTarget = 11f;
			float XTarget = (Mathf.Sin(Move_timer * 4)*(5)) + Target.x;
			MoveTarget = new Vector2 (XTarget, YTarget);
			//Hunt Script End
		}

		thisShip.transform.position = Vector3.MoveTowards(thisShip.transform.position, new Vector3(MoveTarget.x,MoveTarget.y,0), Speed * Time.deltaTime);
	}

}
