using UnityEngine;
using System.Collections;

public class HangerGenPick : MonoBehaviour {
	public GeneratorType TypeGiven ;
	public string Stat1Name = "Energy Produced/sec: ";
	public string Stat2Name = "Heat Produced/sec: ";
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
		if (TypeGiven.Name == GameObject.Find("Ship_Display").GetComponent<ShipController>().Generatortype.Name)
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
		GameObject.Find("ToolTip/Stat1").GetComponent<TextMesh>().text= (Stat1Name + TypeGiven.RechargeRate);
		GameObject.Find("ToolTip/Stat2").GetComponent<TextMesh>().text= (Stat2Name + TypeGiven.HeatRate);
		GameObject.Find("ToolTip/Stat3").GetComponent<TextMesh>().text= "";
		GameObject.Find("ToolTip/Stat4").GetComponent<TextMesh>().text= "";
		GameObject.Find("ToolTip/Stat5").GetComponent<TextMesh>().text= "";
		GameObject.Find("ToolTip/Stat6").GetComponent<TextMesh>().text= "";
		
		Color EquipColor = GameObject.Find ("ToolTip/Equiped").GetComponent<TextMesh> ().color;
		if (TypeGiven.Name == GameObject.Find("Ship_Display").GetComponent<ShipController>().Generatortype.Name)
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
		DataMan.thisgeneratortype = TypeGiven;
		DataMan.UpdateShip();
	}


}
