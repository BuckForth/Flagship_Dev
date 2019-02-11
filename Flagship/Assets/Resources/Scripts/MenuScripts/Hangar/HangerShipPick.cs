using UnityEngine;
using System.Collections;

public class HangerShipPick : MonoBehaviour {
	public ShipType TypeGiven ;
	public string Stat1Name = "Speed: ";
	public string Stat2Name = "Armor: ";
	public string Stat3Name = "Overheat Point: ";
	public string Stat4Name = "Freezing Point: ";
	public GameObject ToolTip;
	[HideInInspector]
	public Color starterColor;

	void Start () 
	{
		starterColor = this.transform.Find("BackDrop").GetComponent<SpriteRenderer>().color;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (TypeGiven.Name == GameObject.Find("Ship_Display").GetComponent<ShipController>().Shiptype.Name)
		{
			this.transform.Find("BackDrop").GetComponent<SpriteRenderer>().color = Color.red;
			this.GetComponent<SpriteRenderer>().color = Color.red;
		}
		else
		{
			this.transform.Find("BackDrop").GetComponent<SpriteRenderer>().color = starterColor;
			this.GetComponent<SpriteRenderer>().color = starterColor;
		}
	}

	void OnMouseOver()
	{
		ToolTip.SetActive(true);
		GameObject.Find("ToolTip/Name").GetComponent<TextMesh>().text= (TypeGiven.Name);
		GameObject.Find("ToolTip/Decription1").GetComponent<TextMesh>().text= (TypeGiven.Decription[0]);
		GameObject.Find("ToolTip/Decription2").GetComponent<TextMesh>().text= (TypeGiven.Decription[1]);
		GameObject.Find("ToolTip/Decription3").GetComponent<TextMesh>().text= (TypeGiven.Decription[2]);
		GameObject.Find("ToolTip/Decription4").GetComponent<TextMesh>().text= (TypeGiven.Decription[3]);
		GameObject.Find("ToolTip/Decription5").GetComponent<TextMesh>().text= (TypeGiven.Decription[4]);
		GameObject.Find("ToolTip/Stat1").GetComponent<TextMesh>().text= (Stat1Name + TypeGiven.ShipSpeed);
		GameObject.Find("ToolTip/Stat2").GetComponent<TextMesh>().text= (Stat2Name + TypeGiven.ShipHull);
		GameObject.Find("ToolTip/Stat3").GetComponent<TextMesh>().text= (Stat3Name + TypeGiven.OverHeatThreshold);
		GameObject.Find("ToolTip/Stat4").GetComponent<TextMesh>().text= (Stat4Name + TypeGiven.FreezeThreshold);
		GameObject.Find("ToolTip/Stat5").GetComponent<TextMesh>().text= "";
		GameObject.Find("ToolTip/Stat6").GetComponent<TextMesh>().text= "";

		Color EquipColor = GameObject.Find ("ToolTip/Equiped").GetComponent<TextMesh> ().color;
		if (TypeGiven.Name == GameObject.Find("Ship_Display").GetComponent<ShipController>().Shiptype.Name)
		{
			GameObject.Find("ToolTip/Equiped").GetComponent<TextMesh>().color = new Color (EquipColor.r , EquipColor.g ,EquipColor.b , 1);
		}
		else
		{
			GameObject.Find("ToolTip/Equiped").GetComponent<TextMesh>().color = new Color (EquipColor.r , EquipColor.g ,EquipColor.b , 0);
		}

	}
	
	void OnMouseExit()
	{
		ToolTip.SetActive(false);
	}
	void OnMouseDown()
	{
		HangerDataManager DataMan = this.GetComponentInParent<HangerDataManager> ();
		DataMan.thisshiptype = TypeGiven;
		DataMan.UpdateShip();
	}


}
