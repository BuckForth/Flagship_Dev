using UnityEngine;
using System.Collections;


public class Rock_2 : Dreadnought_AI 
{
	[HideInInspector]
	public float swapTimer = 0;
	[HideInInspector]
	public bool RunningRight;

	public override void AIUpdate()
	{
		if (Move_order == 0)
		{
			WeaponFire[0] = true;
		}
		else
		{
			WeaponFire[0] = false;
		}
		if (Move_order == 1)
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
		if (Move_order == 0)
		{
			MoveTarget = new Vector2((Mathf.Sin(Move_timer* 2)*18.76f) - 3.54f ,(Mathf.Sin(Move_timer * 4 )*2) + 8.25f);
		}
		else if (Move_order == 1)
		{
			MoveTarget = new Vector2(- 3.54f, (Mathf.Sin(Move_timer) * 2f ) + 8.25f);
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
			float XTarget = Target.x;
			if (swapTimer >= (.5f))
			{
				RunningRight = !RunningRight;
				swapTimer -= .5f;
			}
			if (RunningRight)
			{
				XTarget += 2;
			}
			else
			{
				XTarget += -2;
			}
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

		thisShip.transform.position = Vector3.MoveTowards(thisShip.transform.position, new Vector3(MoveTarget.x,MoveTarget.y,0), Speed * Time.deltaTime);
	}

}
