using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MainMenuDatMan : MonoBehaviour 
{
	public string fileName = "PlayerShip.fs";
	public string playerFile = "Player.fs";


	public ShipType thisshiptype;
	public SheildType thissheildtype;
	public RadiatorType thisradiatortype;
	public GeneratorType thisgeneratortype;
	public WeaponType thismainweapon;
	public WeaponType thiswingweapon;
	public Color32 thismaincolor;
	public Color32 thissecondarycolor;
	public Color32 thiscockpitcolor;
	
	void Update()
	{
		
	}

	void Start()
	{
		if (!File.Exists (Application.persistentDataPath + "/" + fileName)) 
		{
			CreateStarterShip();
		}

		if (!File.Exists (Application.persistentDataPath + "/" + playerFile)) 
		{
			CreateStarterProfile();
		}
	}

	void CreateStarterShip()
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
		bf.Serialize (SavFile, SaveData);
		SavFile.Close();
		Debug.Log ("Starter Ship Created");
	}

	void CreateStarterProfile()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream SavFile = File.Open(Application.persistentDataPath + "/" + playerFile, FileMode.OpenOrCreate);
		PlayerData SaveData = new PlayerData();
		SaveData.Credit = 1000;
		SaveData.MultiWins = 0;
		SaveData.MultiLoses = 0;
		SaveData.FodderDestroyed = 0;
		SaveData.SingleMissionFin = 0;

		SaveData.ShipHangarID = new bool[64];
		SaveData.WeapHangarID = new bool[64];
		SaveData.WingHangarID = new bool[64];
		SaveData.GenHangarID = new bool[64];
		SaveData.RadHangarID = new bool[64];
		SaveData.SheildHangarID = new bool[64];

		bf.Serialize (SavFile, SaveData);
		SavFile.Close();
		Debug.Log ("Starter Profile Created");
	}
}