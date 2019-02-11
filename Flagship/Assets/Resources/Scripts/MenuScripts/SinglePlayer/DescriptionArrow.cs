using UnityEngine;
using System.Collections;

public class DescriptionArrow : MonoBehaviour {
	public float linesPerClick = 1.0f;
	public float unitsPerLine = 0.389f;
	public GameObject ToolTip;
	public float movelength ;

	public Color StartColor;
	public Color HoverColor;
	public Color ClickColor;

	// Use this for initialization
	void Start () 
	{
	
	}

	void OnMouseUp()
	{
		Debug.Log(ToolTip.transform.Find ("Decription").transform.position.y);

		if(linesPerClick < 0 && ToolTip.transform.Find ("Decription").transform.position.y > 2.6) 
		{
		ToolTip.transform.Find ("Decription").transform.position = new Vector3(ToolTip.transform.Find ("Decription").transform.position.x  ,  ToolTip.transform.Find ("Decription").transform.position.y + (linesPerClick * unitsPerLine) ,  ToolTip.transform.Find ("Decription").transform.position.z);
		}

		if(linesPerClick > 0 && ToolTip.transform.Find ("Decription").transform.position.y  - 2.598 <= ToolTip.GetComponent<ToolTipCampaignn>().Mission.BreifingLength * unitsPerLine)
		{
			ToolTip.transform.Find ("Decription").transform.position = new Vector3(ToolTip.transform.Find ("Decription").transform.position.x  ,  ToolTip.transform.Find ("Decription").transform.position.y + (linesPerClick * unitsPerLine) ,  ToolTip.transform.Find ("Decription").transform.position.z);
		}
	}

	void OnMouseOver()
	{
		//change text color
		this.GetComponent<SpriteRenderer>().color = HoverColor;
		if (Input.GetButton ("Fire1"))
		{
			this.GetComponent<SpriteRenderer>().color = ClickColor;
		}
		
	}
	
	void OnMouseExit()
	{
		//change text color
		this.GetComponent<SpriteRenderer>().color = StartColor;
	}

	// Update is called once per frame
	void Update () 
	{
	
	}
}
