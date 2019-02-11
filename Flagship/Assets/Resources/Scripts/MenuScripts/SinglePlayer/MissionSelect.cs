using UnityEngine;
using System.Collections;

public class MissionSelect : MonoBehaviour {
	public Mission Mission ;
	public GameObject ToolTip;
	[HideInInspector]
	public Color starterColor;

	void Start () 
	{
		starterColor = this.transform.GetComponent<SpriteRenderer>().color;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (this.name != "Cancel")
		{
			this.transform.Find("Title").GetComponent<TextMesh>().text = Mission.MissonName;
		}

		if (ToolTip.activeSelf == true && this.name != "Cancel") 
		{

			if(ToolTip.GetComponent<ToolTipCampaignn>().Mission.MissonName == Mission.MissonName)
			{
				this.transform.GetComponent<SpriteRenderer>().color = Color.red;
			}
			else
			{
				this.transform.GetComponent<SpriteRenderer>().color = starterColor;
			}
		}
		else
		{
			this.transform.GetComponent<SpriteRenderer>().color = starterColor;
		}
	}

	void OnMouseDown()
	{
		ToolTip.SetActive(true);
		ToolTip.GetComponent<ToolTipCampaignn>().Mission = Mission;
		this.transform.Find("Decription").transform.localPosition = new Vector3(this.transform.Find("Decription").transform.localPosition.x, 0.40f ,this.transform.Find("Decription").transform.localPosition.z);
	}


}
