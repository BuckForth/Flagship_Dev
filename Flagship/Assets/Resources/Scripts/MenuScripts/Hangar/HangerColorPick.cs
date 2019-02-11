using UnityEngine;
using System.Collections;

public class HangerColorPick : MonoBehaviour {
	public string colorLayer;
	public bool isRed;
	public bool isGreen;
	public bool isBlue;
	[HideInInspector]
	public float StartX;
	public float BarSize = 4.2688f;//Comes from (64 pixels/100 pixels per unit) * Scale
	// Use this for initialization
	void Start () 
	{
		StartX = this.transform.position.x;
	}
	
	// Update is called once per frame
	void Update () 
	{
		byte DataValue = 0;
		HangerDataManager DataMan = this.GetComponentInParent<HangerDataManager> ();
		if(colorLayer == "Main")
		{
			if (isRed)
			{
				DataValue = DataMan.thismaincolor.r;
			}
			if (isBlue)
			{
				DataValue = DataMan.thismaincolor.b;
			}
			if (isGreen)
			{
				DataValue = DataMan.thismaincolor.g;
			}
		}
		
		if(colorLayer == "Second")
		{
			if (isRed)
			{
				DataValue = DataMan.thissecondarycolor.r;
			}
			if (isBlue)
			{
				DataValue = DataMan.thissecondarycolor.b;
			}
			if (isGreen)
			{
				DataValue = DataMan.thissecondarycolor.g;
			}
			
		}
		
		if(colorLayer == "Cockpit")
		{
			if (isRed)
			{
				DataValue = DataMan.thiscockpitcolor.r;
			}
			if (isBlue)
			{
				DataValue = DataMan.thiscockpitcolor.b;
			}
			if (isGreen)
			{
				DataValue = DataMan.thiscockpitcolor.g;
			}
		}
		float PointLoc = (((float)(DataValue)-25.0f)/175.0f)*0.64f;

		this.transform.Find ("Indicator").transform.localPosition = new Vector3 (0,PointLoc,0);
	}

	void OnMouseOver()
	{
		if (Input.GetButton ("Fire1"))
		{
			float mouseX = (Input.mousePosition.x);
			float mouseY = (Input.mousePosition.y);
			Vector3 mouseposition = Camera.main.ScreenToWorldPoint(new Vector3 (mouseX,mouseY,0));
			float PointX = mouseposition.x - StartX;
			byte ColorValue = (byte)(((PointX / BarSize) * 175 ) + 25);
			HangerDataManager DataMan = this.GetComponentInParent<HangerDataManager> ();

			if(colorLayer == "Main")
			{
				if (isRed)
				{
					DataMan.thismaincolor = new Color32 (ColorValue, DataMan.thismaincolor.g, DataMan.thismaincolor.b, 255);
					DataMan.UpdateShip();
				}
				if (isBlue)
				{
					DataMan.thismaincolor = new Color32 (DataMan.thismaincolor.r , DataMan.thismaincolor.g, ColorValue, 255);
					DataMan.UpdateShip();
				}
				if (isGreen)
				{
					DataMan.thismaincolor = new Color32 (DataMan.thismaincolor.r, ColorValue, DataMan.thismaincolor.b, 255);
					DataMan.UpdateShip();
				}
			}

			if(colorLayer == "Second")
			{
				if (isRed)
				{
					DataMan.thissecondarycolor = new Color32 (ColorValue, DataMan.thissecondarycolor.g, DataMan.thissecondarycolor.b, 255);
					DataMan.UpdateShip();
				}
				if (isBlue)
				{
					DataMan.thissecondarycolor = new Color32 (DataMan.thissecondarycolor.r , DataMan.thissecondarycolor.g, ColorValue, 255);
					DataMan.UpdateShip();
				}
				if (isGreen)
				{
					DataMan.thissecondarycolor = new Color32 (DataMan.thissecondarycolor.r, ColorValue, DataMan.thissecondarycolor.b, 255);
					DataMan.UpdateShip();
				}
				
			}
	
			if(colorLayer == "Cockpit")
			{
				if (isRed)
				{
					DataMan.thiscockpitcolor = new Color32 (ColorValue, DataMan.thiscockpitcolor.g, DataMan.thiscockpitcolor.b, 255);
					DataMan.UpdateShip();
				}
				if (isBlue)
				{
					DataMan.thiscockpitcolor = new Color32 (DataMan.thiscockpitcolor.r , DataMan.thiscockpitcolor.g, ColorValue, 255);
					DataMan.UpdateShip();
				}
				if (isGreen)
				{
					DataMan.thiscockpitcolor = new Color32 (DataMan.thiscockpitcolor.r, ColorValue, DataMan.thiscockpitcolor.b, 255);
					DataMan.UpdateShip();
				}
			}
		}
	}


}
