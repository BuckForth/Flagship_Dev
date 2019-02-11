using UnityEngine;
using System.Collections;

public class HangerWeaponPick : MonoBehaviour {
	public WeaponType TypeGiven ;
	public bool isWing;
	public string Stat1Name = "Fire Rate: ";
	public string Stat2Name = "Energy Cost: ";
	public string Stat3Name = "Heat Produced: ";
	public string Stat4Name = "Heat Dammage: ";
	public string Stat5Name = "Physical Dammage: ";
	public string Stat6Name = "Energy Dammage: ";
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
		if ((TypeGiven.Name == GameObject.Find("Ship_Display").GetComponent<ShipController>().MainWeapon.Name && !isWing) || ((TypeGiven.Name == GameObject.Find("Ship_Display").GetComponent<ShipController>().WingWeapon.Name && isWing) ))
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
		GameObject.Find("ToolTip/Stat1").GetComponent<TextMesh>().text= (Stat1Name + (1 / TypeGiven.FireRate) + " per sec");
		GameObject.Find("ToolTip/Stat2").GetComponent<TextMesh>().text= (Stat2Name + TypeGiven.UseEnergy);
		GameObject.Find("ToolTip/Stat3").GetComponent<TextMesh>().text= (Stat3Name + TypeGiven.UseHeat);
		GameObject.Find("ToolTip/Stat4").GetComponent<TextMesh>().text= (Stat4Name + TypeGiven.WeaponHeat);

		if (TypeGiven.UseHeat < 0)
		{
			GameObject.Find("ToolTip/Stat3").GetComponent<TextMesh>().text= ("Frost Produced: " + (-TypeGiven.UseHeat));
		}
		if (TypeGiven.WeaponHeat < 0)
		{
			GameObject.Find("ToolTip/Stat4").GetComponent<TextMesh>().text= ("Ice Dammage: " + (-TypeGiven.WeaponHeat));
		}

		GameObject.Find("ToolTip/Stat5").GetComponent<TextMesh>().text= (Stat5Name + TypeGiven.WeaponPhysicalDamage);
		GameObject.Find("ToolTip/Stat6").GetComponent<TextMesh>().text= (Stat6Name + TypeGiven.WeaponEnergyDamage);
		
		Color EquipColor = GameObject.Find ("ToolTip/Equiped").GetComponent<TextMesh> ().color;
		if ((TypeGiven.Name == GameObject.Find("Ship_Display").GetComponent<ShipController>().MainWeapon.Name && !isWing) || ((TypeGiven.Name == GameObject.Find("Ship_Display").GetComponent<ShipController>().WingWeapon.Name && isWing) ))
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
		if(!isWing)
		{
			DataMan.thismainweapon = TypeGiven;
		}
		else
		{
			DataMan.thiswingweapon = TypeGiven;
		}
		DataMan.UpdateShip();
	}


}
