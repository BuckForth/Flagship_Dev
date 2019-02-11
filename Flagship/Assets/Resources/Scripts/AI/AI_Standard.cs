using UnityEngine;
using System.Collections;

public class AI_Standard : MonoBehaviour 
{
	public float Aggression = 0.5f;
	public float Self_Preservation = 0.5f;
	[HideInInspector]
	public ShipController ThisShip;
	[HideInInspector]
	public ShipController TargetShip;
	[HideInInspector]
	public bool RunningRight;
	[HideInInspector]
	public bool HidingDown = false;
	[HideInInspector]
	public float swapTimer;
	[HideInInspector]
	public float swapTimer2;
	[HideInInspector]
	public bool IsShootMain;
	[HideInInspector]
	public bool IsShootWing;
	// Use this for initialization
	void Start () 
	{
		ThisShip = this.GetComponent<ShipController>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		ShootMainGuns ();
		ShootWingGuns ();
		ShipController[] ShipControllers = FindObjectsOfType<ShipController>() as ShipController[];
		float LastClose = 100.0f;
		foreach (ShipController Ship in ShipControllers) 
		{
			if (Mathf.Abs(Ship.transform.position.x - ThisShip.transform.position.x) < LastClose && Ship != ThisShip && Ship.gameObject.layer != ThisShip.gameObject.layer)
			{
				TargetShip = Ship as ShipController;
				LastClose = Mathf.Abs(Ship.transform.position.x - ThisShip.transform.position.x);
			}
		}

		if (ThisShip.ShipSheild < (Self_Preservation * ThisShip.Sheildtype.MaxSheild) || ThisShip.ShipPower < (Self_Preservation * 200) || ThisShip.ShipHeat > (Self_Preservation * ThisShip.Shiptype.OverHeatThreshold))
		{

			Defensive ();
		}
		else
		{

			Offensive();
		}
	}

	void Defensive ()
	{
		IsShootMain = false;
		IsShootWing = false;
		if (!HidingDown)
		{
			swapTimer += Time.deltaTime;
		}
		Vector3 OldLocation = this.transform.position;
		Vector3 Target = TargetShip.transform.position;
		float step = ThisShip.ShipSpeed;
		float YTarget = Target.y + (7 / Aggression);
		float XTarget = Target.x;
		if (RunningRight)
		{
			XTarget += 7/Aggression;
		}
		else
		{
			XTarget += -7/Aggression;
		}
		
		if(Mathf.Abs(OldLocation.x - XTarget) < 3)
		{
			RunningRight = !RunningRight;
		}
		if (Mathf.Abs(Target.x - OldLocation.x) < (Mathf.Abs(Target.x - XTarget)/3) && Target.y < OldLocation.y + 3)
		{
			IsShootMain = true;
			if ((ThisShip.ShipPower > (Self_Preservation * 50)) && (ThisShip.ShipHeat < (Self_Preservation * ThisShip.Shiptype.OverHeatThreshold / 2)))
			{
				IsShootWing = true;
			}
		}

		if (HidingDown)
		{
			YTarget = ThisShip.Bounds.y;
			swapTimer2 += Time.deltaTime;
			if (RunningRight)
			{
				XTarget = ThisShip.Bounds.z;
			}
			else
			{
				XTarget = ThisShip.Bounds.x;
			}
			if (Mathf.Abs(OldLocation.y - Target.y) <= (7 * Self_Preservation) && swapTimer2 > 0.5f)
			{
				HidingDown = !HidingDown;
				swapTimer2 = 0;
			}
		}
		else
		{
			YTarget = ThisShip.Bounds.w;
			swapTimer2 += Time.deltaTime;
			if (Mathf.Abs(OldLocation.y - Target.y) <= (40 * Self_Preservation) && swapTimer2 > 0.5f)
			{
				HidingDown = !HidingDown;
				swapTimer2 = 0;
			}
		}
		if (Mathf.Abs(Target.x - OldLocation.x) < (Mathf.Abs(Target.x - XTarget)/3))
		{
			IsShootMain = true;
		}

		
		if (XTarget < ThisShip.Bounds.x)
		{
			XTarget = ThisShip.Bounds.x;
		}
		if (YTarget < ThisShip.Bounds.y)
		{
			YTarget = ThisShip.Bounds.y;
		}
		if (XTarget > ThisShip.Bounds.z)
		{
			XTarget = ThisShip.Bounds.z;
		}
		if (YTarget > ThisShip.Bounds.w)
		{
			YTarget = ThisShip.Bounds.w;
		}
		Target = new Vector3 (XTarget, YTarget, 0);
		this.transform.position = Vector3.MoveTowards(transform.position, Target, step * Time.deltaTime);
		ThisShip.RotateShip (OldLocation , this.transform.position);
	}

	void Offensive()
	{
		IsShootMain = true;
		IsShootWing = false;
		swapTimer += Time.deltaTime;
		Vector3 OldLocation = this.transform.position;
		Vector3 Target = TargetShip.transform.position;
		float step = ThisShip.ShipSpeed;
		float YTarget = Target.y + (5 / Aggression);
		float XTarget = Target.x;
		if (swapTimer >= (.15f / Aggression))
		{
			RunningRight = !RunningRight;
			swapTimer -= .15f / Aggression;
		}
		if (RunningRight)
		{
			XTarget += 1/Aggression;
		}
		else
		{
			XTarget += -1/Aggression;
		}
		if (Mathf.Abs(Target.x - OldLocation.x) < (Mathf.Abs(Target.x - XTarget)/3))
		{
			IsShootMain = true;
			if ((ThisShip.ShipPower > (Self_Preservation * 50)) && (ThisShip.ShipHeat < (Self_Preservation * ThisShip.Shiptype.OverHeatThreshold / 2)))
			{
				IsShootWing = true;
			}
		}

		if (XTarget < ThisShip.Bounds.x)
		{
			XTarget = ThisShip.Bounds.x;
		}
		if (YTarget < ThisShip.Bounds.y)
		{
			YTarget = ThisShip.Bounds.y;
		}
		if (XTarget > ThisShip.Bounds.z)
		{
			XTarget = ThisShip.Bounds.z;
		}
		if (YTarget > ThisShip.Bounds.w)
		{
			YTarget = ThisShip.Bounds.w;
		}
		Target = new Vector3 (XTarget, YTarget, 0);
		this.transform.position = Vector3.MoveTowards(transform.position, Target, step * Time.deltaTime);
		ThisShip.RotateShip (OldLocation , this.transform.position);
	}

	void ShootWingGuns()
	{
		ThisShip.IsWingShoot = IsShootWing;
	}

	void ShootMainGuns()
	{
		ThisShip.IsMainShoot = IsShootMain;
	}

}
