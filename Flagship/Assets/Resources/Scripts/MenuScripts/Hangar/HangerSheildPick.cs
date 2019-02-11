using UnityEngine;
using System.Collections;

public class HangerSheildPick : MonoBehaviour {
	public SheildType TypeGiven ;
	public string Stat1Name = "Max Sheild: ";
	public string Stat2Name = "Charge Time: ";
	public string Stat3Name = "Recharge Strength: ";
	public string Stat4Name = "Energy Cost: ";
	public string Stat5Name = "Heat Produced: ";
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
		if (TypeGiven.Name == GameObject.Find("Ship_Display").GetComponent<ShipController>().Sheildtype.Name)
		{
			this.transform.Find("BackDrop").GetComponent<SpriteRenderer>().color = Color.red;
		}
		else
		{
			this.transform.Find("BackDrop").GetComponent<SpriteRenderer>().color = starterColor;
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
		GameObject.Find("ToolTip/Stat1").GetComponent<TextMesh>().text= (Stat1Name + TypeGiven.MaxSheild);
		GameObject.Find("ToolTip/Stat2").GetComponent<TextMesh>().text= (Stat2Name + (200 / TypeGiven.ChargeRate) + " sec");
		GameObject.Find("ToolTip/Stat3").GetComponent<TextMesh>().text= (Stat3Name + TypeGiven.CycleRecharge);
		GameObject.Find("ToolTip/Stat4").GetComponent<TextMesh>().text= (Stat4Name + TypeGiven.CycleEnergyCost);
		GameObject.Find("ToolTip/Stat5").GetComponent<TextMesh>().text= (Stat5Name + TypeGiven.CycleHeatUse);
		GameObject.Find("ToolTip/Stat6").GetComponent<TextMesh>().text= "";
		
		Color EquipColor = GameObject.Find ("ToolTip/Equiped").GetComponent<TextMesh> ().color;
		if (TypeGiven.Name == GameObject.Find("Ship_Display").GetComponent<ShipController>().Sheildtype.Name)
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
		DataMan.thissheildtype = TypeGiven;
		DataMan.UpdateShip();
	}


}
