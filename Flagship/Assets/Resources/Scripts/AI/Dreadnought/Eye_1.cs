using UnityEngine;
using System.Collections;


public class Eye_1: Dreadnought_AI 
{
	

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
	}

	public override void Move(float Speed)
	{
		if (Move_order == 0)
		{
			MoveTarget = new Vector2((Mathf.Sin(Move_timer* 2)*1f) - 3.54f ,(Mathf.Sin(Move_timer * 2+ (Mathf.PI/2f))*1f) + 8.25f);
		}
		thisShip.transform.position = Vector3.MoveTowards (thisShip.transform.position, new Vector3 (MoveTarget.x, MoveTarget.y, 0), Speed * Time.deltaTime);
	}

}
