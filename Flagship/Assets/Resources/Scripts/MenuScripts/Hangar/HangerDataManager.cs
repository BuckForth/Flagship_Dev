using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class HangerDataManager : MonoBehaviour 
{
	
	public string fileName = "PlayerShip.fs";

	public string thisshipname;
	//[HideInInspector]
	public ShipType thisshiptype;
	//[HideInInspector]
	public SheildType thissheildtype;
	//[HideInInspector]
	public RadiatorType thisradiatortype;
	//[HideInInspector]
	public GeneratorType thisgeneratortype;
	//[HideInInspector]
	public WeaponType thismainweapon;
	//[HideInInspector]
	public WeaponType thiswingweapon;
	//[HideInInspector]
	public Color32 thismaincolor;
	//[HideInInspector]
	public Color32 thissecondarycolor;
	//[HideInInspector]
	public Color32 thiscockpitcolor;

	void Update()
	{

	}

	public void SaveGame()//Used for writing a Playership to a file
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream SavFile = File.Open(Application.persistentDataPath + "/" + fileName, FileMode.OpenOrCreate);
		PlayerShip SaveData = new PlayerShip();
		SaveData.shiptype = thisshiptype.Name;
		SaveData.sheildtype = thissheildtype.Name;
		SaveData.radiatortype = thisradiatortype.Name;
		SaveData.generatortype = thisgeneratortype.Name;
		SaveData.mainweaponname = thismainweapon.Name;
		SaveData.wingweaponname = thiswingweapon.Name;
		SaveData.maincolor = new SerializeableColor(thismaincolor);
		SaveData.secondarycolor = new SerializeableColor(thissecondarycolor);
		SaveData.cockpitcolor = new SerializeableColor(thiscockpitcolor);
		SaveData.shipname = thisshipname;
		bf.Serialize (SavFile, SaveData);
		SavFile.Close();
		Debug.Log ("Save Complete");
	}

	public void LoadGame()//Used for loading a Playership from a file
	{
		if (File.Exists (Application.persistentDataPath + "/" + fileName)) 
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/" + fileName , FileMode.Open);
			PlayerShip LoadShip = (PlayerShip)bf.Deserialize(file);
			thisshiptype = Resources.Load("ShipParts/ShipsTypes/" + LoadShip.shiptype) as ShipType;
			thissheildtype = Resources.Load("ShipParts/SheildTypes/" + LoadShip.sheildtype) as SheildType;
			thisradiatortype = Resources.Load("ShipParts/RadiatorTypes/" + LoadShip.radiatortype) as RadiatorType;
			thisgeneratortype = Resources.Load("ShipParts/GeneratorTypes/" + LoadShip.generatortype) as GeneratorType;
			thismainweapon = Resources.Load("ShipParts/WeaponTypes/" + LoadShip.mainweaponname) as WeaponType;
			thiswingweapon = Resources.Load("ShipParts/WeaponTypes/" + LoadShip.wingweaponname) as WeaponType;
			thismaincolor = LoadShip.maincolor.GetColor();
			thissecondarycolor = LoadShip.secondarycolor.GetColor();
			thiscockpitcolor = LoadShip.cockpitcolor.GetColor();
			if (LoadShip.shipname != null)
			{
				thisshipname = LoadShip.shipname;
			}
			else
			{
				thisshipname = "shipname";
			}

			file.Close();
			Debug.Log ("Load Complete");
		}
		else
		{
			Debug.Log ("No Saved File");
		}
	}

	void Start() 
	{
		LoadGame ();
		UpdateNameBoxPlaceHolder();
		CastShip ();
		
	}

	public void UpdateNameBoxPlaceHolder ()
	{
		this.transform.Find ("ShipName/InputField").GetComponent<InputField>().text = thisshipname;
	}

	public void UpdateShipName ()
	{
		thisshipname = this.transform.Find ("ShipName/InputField").GetComponent<InputField>().text;
		UpdateNameBoxPlaceHolder ();
		UpdateShip ();
	}

	public void UpdateShip ()
	{
		SaveGame ();
		LoadGame ();
		CastShip ();
	}

	void CastShip ()
	{
		this.transform.Find ("Ship_Display").GetComponent<ShipController> ().Shiptype = thisshiptype;
		this.transform.Find ("Ship_Display").GetComponent<ShipController> ().MainWeapon = thismainweapon;
		this.transform.Find ("Ship_Display").GetComponent<ShipController> ().WingWeapon = thiswingweapon;
		this.transform.Find ("Ship_Display").GetComponent<ShipController> ().Sheildtype = thissheildtype;
		this.transform.Find ("Ship_Display").GetComponent<ShipController> ().Radiatortype = thisradiatortype;
		this.transform.Find ("Ship_Display").GetComponent<ShipController> ().Generatortype = thisgeneratortype;
		this.transform.Find ("Ship_Display").GetComponent<ShipController> ().MainColor = thismaincolor;
		this.transform.Find ("Ship_Display").GetComponent<ShipController> ().SecondaryColor = thissecondarycolor;
		this.transform.Find ("Ship_Display").GetComponent<ShipController> ().CockpitColor = thiscockpitcolor;
		this.transform.Find ("Ship_Display/Main").GetComponent<SpriteRenderer>().sprite = thisshiptype.SpriteSet[2];
		this.transform.Find ("Ship_Display/Second").GetComponent<SpriteRenderer>().sprite = thisshiptype.SpriteSecondSet[2];
		this.transform.Find ("Ship_Display/Cockpit").GetComponent<SpriteRenderer>().sprite = thisshiptype.SpriteCockpitSet[2];

	}


}