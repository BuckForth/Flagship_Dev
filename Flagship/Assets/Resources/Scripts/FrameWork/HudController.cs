using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class HudController : MonoBehaviour 
{
	public ShipController FocusShip;
	public AudioClip WarningSound;
	[HideInInspector]
	public GameObject MessageObject;
	[HideInInspector]
	public float BarHeight = 0;
	[HideInInspector]
	public float BossBarHeight = 0;
	[HideInInspector]
	public float genY = 0;
	[HideInInspector]
	public float heatY = 0;
	[HideInInspector]
	public float iceY = 0;
	[HideInInspector]
	public float BossHullX = 0;
	[HideInInspector]
	public float BossSheildX = 0;
	[HideInInspector]
	public float heatCycleY = 0;
	[HideInInspector]
	public float sheildY = 0;
	[HideInInspector]
	public float sheildCycleY = 0;
	[HideInInspector]
	public float hullY = 0;
	[HideInInspector]
	public bool WarningSheild = false;
	[HideInInspector]
	public bool WarningHeat = false;
	[HideInInspector]
	public bool WarningHull = false;
	[HideInInspector]
	public bool WarningIce = false;
	[HideInInspector]
	public bool Warning = false;
	[HideInInspector]
	public GameObject BossIconObject;
	[HideInInspector]
	public bool Is_Paused = false;
	[HideInInspector]
	public bool Is_Win = false;
	[HideInInspector]
	public bool Ended = false;
	[HideInInspector]
	public GameObject PauseObj;
	[HideInInspector]
	public GameObject WinObj;
	[HideInInspector]
	public GameObject GameOverObj;
	[HideInInspector]
	public PlayerData SaveFile;
	[HideInInspector]
	public string playerFile = "Player.fs";

	
	public Entity BossStatEnt = null;

	
	Vector3 HeatThreahPos;
	Vector3 IceThreahPos;
	Vector3 MaxSheildPos;
	Vector3 MaxHullPos;


	void Start () 
	{
		PauseObj = this.transform.Find ("PauseMenu").gameObject;
		PauseObj.SetActive (false);

		WinObj = this.transform.Find ("LevelCompleteMenu").gameObject;
		WinObj.SetActive (false);

		GameOverObj = this.transform.Find ("GameOverMenu").gameObject;
		GameOverObj.SetActive (false);
		
		MessageObject = this.transform.Find ("Message").gameObject;
		MessageObject.SetActive (false);

		BossIconObject = this.transform.Find ("BossMeter").gameObject;
		BossIconObject.SetActive (false);

		BarHeight = this.transform.Find ("GeneratorBar").transform.localScale.y;
		BossBarHeight = this.transform.Find ("BossMeter/Boss_Hull").transform.localScale.y;

		genY = this.transform.Find ("GeneratorBar").transform.localPosition.y;
		heatY = this.transform.Find ("HeatBar").transform.localPosition.y;
		iceY = this.transform.Find ("IceBar").transform.localPosition.y;
		heatCycleY = this.transform.Find ("HeatCycleBar").transform.localPosition.y;
		sheildY = this.transform.Find ("SheildBar").transform.localPosition.y;
		sheildCycleY = this.transform.Find ("SheildCycleBar").transform.localPosition.y;
		hullY = this.transform.Find ("HullBar").transform.localPosition.y;

		BossHullX = this.transform.Find ("BossMeter/Boss_Hull").transform.localPosition.x;
		BossSheildX = this.transform.Find ("BossMeter/Boss_Sheild").transform.localPosition.x;
		HeatThreahPos = this.transform.Find ("HeatThresh").transform.position;
		IceThreahPos = this.transform.Find ("IceThresh").transform.position;
		MaxSheildPos = this.transform.Find ("SheildMax").transform.position;
		MaxHullPos = this.transform.Find ("HullMax").transform.position;
		if (FocusShip != null)
		{
			this.transform.Find ("HeatThresh").transform.position = new Vector3 (HeatThreahPos.x, HeatThreahPos.y + ((FocusShip.Shiptype.OverHeatThreshold / 200) * 3.49f), HeatThreahPos.z);
			this.transform.Find ("IceThresh").transform.position = new Vector3 (IceThreahPos.x, IceThreahPos.y + ((-FocusShip.Shiptype.FreezeThreshold / 200) * 3.49f), IceThreahPos.z);
			this.transform.Find ("SheildMax").transform.position = new Vector3 (MaxSheildPos.x, MaxSheildPos.y + ((FocusShip.Sheildtype.MaxSheild / 200) * 3.49f), MaxSheildPos.z);
			this.transform.Find ("HullMax").transform.position = new Vector3 (MaxHullPos.x, MaxHullPos.y + ((FocusShip.Shiptype.ShipHull / 200) * 3.49f), MaxHullPos.z);
		}
	}

	public void ActiveateWin()
	{
		Time.timeScale = 0;
		if (File.Exists (Application.persistentDataPath + "/" + playerFile)) 
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/" + playerFile , FileMode.Open);
			SaveFile = (PlayerData)bf.Deserialize(file);
			file.Close();
			Debug.Log ("Load Complete");

			SaveFile.Credit += GameObject.Find ("Level_Controller").GetComponent<Level_Controller> ().score;
			SaveFile.FodderDestroyed += GameObject.Find ("Level_Controller").GetComponent<Level_Controller> ().fodderkill;
			SaveFile.SingleMissionFin += 1;

			FileStream Savestream = File.Open(Application.persistentDataPath + "/" + playerFile, FileMode.OpenOrCreate);
			bf.Serialize (Savestream, SaveFile);
			Savestream.Close();
			Debug.Log ("Save Complete");
		}
		else
		{
			Debug.Log ("No Saved File");
		}

		WinObj.SetActive (true);
		this.transform.Find ("LevelCompleteMenu/ScoreValue").gameObject.GetComponent<TextMesh>().text = GameObject.Find ("Level_Controller").GetComponent<Level_Controller> ().score.ToString();
		this.transform.Find ("LevelCompleteMenu/KillCount").gameObject.GetComponent<TextMesh>().text = GameObject.Find ("Level_Controller").GetComponent<Level_Controller> ().fodderkill.ToString();
		if (FocusShip != null)
		{
			this.FocusShip.GetComponent<Entity>().enabled = false;
		}
		Is_Win = true;
	}
	
	void WarningBells ()
	{

		if (FocusShip.ShipHull < FocusShip.Shiptype.ShipHull * 0.3f ) 
		{
			if (WarningHull == false) 
			{
				Debug.Log ("Hull Critical");
				WarningHull = true;
				if (!Warning)
				{
					this.transform.GetComponent<AudioSource> ().PlayOneShot (WarningSound, 0.5f);
				}
			}
		}
		else
		{
			WarningHull = false;
		}

		if (FocusShip.ShipSheild < FocusShip.Sheildtype.MaxSheild * 0.3f) 
		{
			if (WarningSheild == false) 
			{
				Debug.Log ("Sheild Critical");
				WarningSheild = true;
				if (!Warning)
				{
					this.transform.GetComponent<AudioSource> ().PlayOneShot (WarningSound, 0.5f);
				}
			}
		}
		else
		{
			WarningSheild = false;
		}


		if (FocusShip.ShipHeat < FocusShip.Shiptype.FreezeThreshold * 0.8f ) 
		{
			if (WarningIce == false) 
			{
				Debug.Log ("Iceing Critical");
				WarningIce = true;
				if (!Warning)
				{
					this.transform.GetComponent<AudioSource> ().PlayOneShot (WarningSound, 0.5f);
				}
			}
		}
		else
		{
			WarningIce = false;
		}


		if (FocusShip.ShipHeat > FocusShip.Shiptype.OverHeatThreshold * 0.8f ) 
		{
			if (WarningHeat == false) 
			{
				Debug.Log ("Heat Critical");
				WarningHeat = true;
				if (!Warning)
				{
					this.transform.GetComponent<AudioSource> ().PlayOneShot (WarningSound, 0.5f);
				}
			}
		}
		else
		{
			WarningHeat = false;
		}


		Warning = (WarningHeat || WarningHull || WarningSheild || WarningIce);
		this.transform.Find ("Warnings").gameObject.SetActive(Warning);
		this.transform.Find ("Warnings/Heat").gameObject.SetActive(WarningHeat);
		this.transform.Find ("Warnings/Sheild").gameObject.SetActive(WarningSheild);
		this.transform.Find ("Warnings/Hull").gameObject.SetActive(WarningHull);
		this.transform.Find ("Warnings/Ice").gameObject.SetActive(WarningIce);

	}

	void UpdateScore ()
	{
		this.transform.Find ("ScoreValue").GetComponent<TextMesh> ().text = GameObject.Find ("Level_Controller").GetComponent<Level_Controller> ().score.ToString ();
	}

	// Update is called once per frame
	void Update () 
	{
		PauseMenu ();
		GameOverMenu ();
		UpdateScore ();
		
		this.transform.Find ("HeatThresh").transform.position = new Vector3 (HeatThreahPos.x, HeatThreahPos.y + ((FocusShip.Shiptype.OverHeatThreshold / 200) * 3.49f), HeatThreahPos.z);
		this.transform.Find ("IceThresh").transform.position = new Vector3 (IceThreahPos.x, IceThreahPos.y + ((-FocusShip.Shiptype.FreezeThreshold / 200) * 3.49f), IceThreahPos.z);
		this.transform.Find ("SheildMax").transform.position = new Vector3 (MaxSheildPos.x, MaxSheildPos.y + ((FocusShip.Sheildtype.MaxSheild / 200) * 3.49f), MaxSheildPos.z);
		this.transform.Find ("HullMax").transform.position = new Vector3 (MaxHullPos.x, MaxHullPos.y + ((FocusShip.Shiptype.ShipHull / 200) * 3.49f), MaxHullPos.z);



		WarningBells ();

		Transform GeneratorBarTrans = this.transform.Find ("GeneratorBar").transform;
		Transform HeatBarTrans = this.transform.Find ("HeatBar").transform;
		Transform IceBarTrans = this.transform.Find ("IceBar").transform;

		Transform HeatCycleBarTrans = this.transform.Find ("HeatCycleBar").transform;
		Transform SheildBarTrans = this.transform.Find ("SheildBar").transform;
		Transform SheildCycleBarTrans = this.transform.Find ("SheildCycleBar").transform;
		Transform HullBarTrans = this.transform.Find ("HullBar").transform;

		GeneratorBarTrans.localScale = new Vector3 (GeneratorBarTrans.localScale.x,(BarHeight / 200) * FocusShip.ShipPower, GeneratorBarTrans.localScale.z);
		float newGenY = genY + (((FocusShip.ShipPower / 200) - 1) * (BarHeight / 2));
		GeneratorBarTrans.localPosition = new Vector3(GeneratorBarTrans.localPosition.x, newGenY , GeneratorBarTrans.localPosition.z);
		GeneratorBarTrans.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (1 , FocusShip.ShipPower / 200);

		if(FocusShip.ShipHeat >= 0)
		{
			HeatBarTrans.localScale = new Vector3 (HeatBarTrans.localScale.x,(BarHeight / 200) * FocusShip.ShipHeat, HeatBarTrans.localScale.z);
			float newGHeatY = heatY + (((FocusShip.ShipHeat / 200) - 1) * (BarHeight / 2));
			HeatBarTrans.localPosition = new Vector3(HeatBarTrans.localPosition.x, newGHeatY , HeatBarTrans.localPosition.z);
			HeatBarTrans.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (1 , FocusShip.ShipHeat / 200);

			IceBarTrans.localScale = new Vector3 (IceBarTrans.localScale.x,0, IceBarTrans.localScale.z);
			float newIceY = iceY + (((FocusShip.ShipHeat / 200) - 1) * (BarHeight / 2));
			IceBarTrans.localPosition = new Vector3(IceBarTrans.localPosition.x, newIceY , IceBarTrans.localPosition.z);
			IceBarTrans.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (1 , 0);
		}
		else
		{
			IceBarTrans.localScale = new Vector3 (IceBarTrans.localScale.x, (BarHeight / 200) * -FocusShip.ShipHeat, IceBarTrans.localScale.z);
			float newIceY = iceY + (((-FocusShip.ShipHeat / 200) - 1) * (BarHeight / 2));
			IceBarTrans.localPosition = new Vector3(IceBarTrans.localPosition.x, newIceY , IceBarTrans.localPosition.z);
			IceBarTrans.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (1 , -FocusShip.ShipHeat / 200);

			HeatBarTrans.localScale = new Vector3 (HeatBarTrans.localScale.x,0, HeatBarTrans.localScale.z);
			float newGHeatY = heatY + (((FocusShip.ShipHeat / 200) - 1) * (BarHeight / 2));
			HeatBarTrans.localPosition = new Vector3(HeatBarTrans.localPosition.x, newGHeatY , HeatBarTrans.localPosition.z);
			HeatBarTrans.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (1 , 0);
		}

		HeatCycleBarTrans.localScale = new Vector3 (HeatCycleBarTrans.localScale.x,(BarHeight / 200) * FocusShip.ShipHeatCycle, HeatCycleBarTrans.localScale.z);
		float newCycHeatY = heatCycleY + (((FocusShip.ShipHeatCycle / 200) - 1) * (BarHeight / 2));
		HeatCycleBarTrans.localPosition = new Vector3(HeatCycleBarTrans.localPosition.x, newCycHeatY , HeatCycleBarTrans.localPosition.z);
		HeatCycleBarTrans.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (1 , FocusShip.ShipHeatCycle / 200);

		SheildBarTrans.localScale = new Vector3 (SheildBarTrans.localScale.x,(BarHeight / 200) * FocusShip.ShipSheild, SheildBarTrans.localScale.z);
		float newSheildY = sheildY + (((FocusShip.ShipSheild / 200) - 1) * (BarHeight / 2));
		SheildBarTrans.localPosition = new Vector3(SheildBarTrans.localPosition.x, newSheildY , SheildBarTrans.localPosition.z);
		SheildBarTrans.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (1 , FocusShip.ShipSheild / 200);
		
		SheildCycleBarTrans.localScale = new Vector3 (SheildCycleBarTrans.localScale.x,(BarHeight / 200) * FocusShip.ShipSheildCycle, SheildCycleBarTrans.localScale.z);
		float newCycSheildY = sheildCycleY + (((FocusShip.ShipSheildCycle / 200) - 1) * (BarHeight / 2));
		SheildCycleBarTrans.localPosition = new Vector3(SheildCycleBarTrans.localPosition.x, newCycSheildY , SheildCycleBarTrans.localPosition.z);
		SheildCycleBarTrans.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (1 , FocusShip.ShipSheildCycle / 200);

		HullBarTrans.localScale = new Vector3 (HullBarTrans.localScale.x,(BarHeight / 200) * FocusShip.ShipHull, HullBarTrans.localScale.z);
		float newHullY = hullY + (((FocusShip.ShipHull / 200) - 1) * (BarHeight / 2));
		HullBarTrans.localPosition = new Vector3(HullBarTrans.localPosition.x, newHullY , HullBarTrans.localPosition.z);
		HullBarTrans.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (1 , FocusShip.ShipHull / 200);


		if (BossStatEnt != null)
		{
			float Trans = 0.20897f;
			BossIconObject.SetActive (true);
			
			Transform BossHullTrans = this.transform.Find ("BossMeter/Boss_Hull").transform;
			Transform BossSheildTrans = this.transform.Find ("BossMeter/Boss_Sheild").transform;

			BossIconObject.transform.Find ("Icon_Main").GetComponent<SpriteRenderer>().sprite = BossStatEnt.transform.Find("Main").GetComponent<SpriteRenderer>().sprite;
			BossIconObject.transform.Find ("Icon_Second").GetComponent<SpriteRenderer>().sprite = BossStatEnt.transform.Find("Second").GetComponent<SpriteRenderer>().sprite;
			BossIconObject.transform.Find ("Icon_Cockpit").GetComponent<SpriteRenderer>().sprite = BossStatEnt.transform.Find("Cockpit").GetComponent<SpriteRenderer>().sprite;
			Vector3 Newsize = new Vector3 (Trans / BossIconObject.transform.Find ("Icon_Main").GetComponent<SpriteRenderer>().sprite.bounds.size.x , -Trans / BossIconObject.transform.Find ("Icon_Main").GetComponent<SpriteRenderer>().sprite.bounds.size.y  , Trans / BossIconObject.transform.Find ("Icon_Main").GetComponent<SpriteRenderer>().sprite.bounds.size.z); //0.2059785 / size = scale
			BossIconObject.transform.Find ("Icon_Main").transform.localScale = Newsize;
			BossIconObject.transform.Find ("Icon_Second").transform.localScale = Newsize;
			BossIconObject.transform.Find ("Icon_Cockpit").transform.localScale = Newsize;
			BossIconObject.transform.Find ("Icon_Main").GetComponent<SpriteRenderer>().color = BossStatEnt.transform.Find("Main").GetComponent<SpriteRenderer>().color;
			BossIconObject.transform.Find ("Icon_Second").GetComponent<SpriteRenderer>().color = BossStatEnt.transform.Find("Second").GetComponent<SpriteRenderer>().color;
			BossIconObject.transform.Find ("Icon_Cockpit").GetComponent<SpriteRenderer>().color = BossStatEnt.transform.Find("Cockpit").GetComponent<SpriteRenderer>().color;
		
			if (BossStatEnt is DreadnoughtController)
			{
				BossHullTrans.localScale = new Vector3 (BossHullTrans.localScale.x,(BossBarHeight / BossStatEnt.GetComponent<DreadnoughtController>().BattlePhase[BossStatEnt.GetComponent<DreadnoughtController>().ActivePhase].PhaseHull) * BossStatEnt.ShipHull, BossHullTrans.localScale.z);
				float newBossHullX = BossHullX + (((BossStatEnt.ShipHull / BossStatEnt.GetComponent<DreadnoughtController>().BattlePhase[BossStatEnt.GetComponent<DreadnoughtController>().ActivePhase].PhaseHull) - 1) * (BossBarHeight / 2));
				BossHullTrans.localPosition = new Vector3(newBossHullX, BossHullTrans.localPosition.y , BossHullTrans.localPosition.z);
				BossHullTrans.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (1 , BossStatEnt.ShipHull / BossStatEnt.GetComponent<DreadnoughtController>().BattlePhase[BossStatEnt.GetComponent<DreadnoughtController>().ActivePhase].PhaseHull);
				
				BossSheildTrans.localScale = new Vector3 (BossSheildTrans.localScale.x,(BossBarHeight / 200) * BossStatEnt.ShipSheild, BossSheildTrans.localScale.z);
				float newBossSheildX = BossSheildX + (((BossStatEnt.ShipSheild / 200) - 1) * (BossBarHeight / 2));
				BossSheildTrans.localPosition = new Vector3(newBossSheildX,  BossSheildTrans.localPosition.y , BossSheildTrans.localPosition.z);
				BossSheildTrans.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (1 , FocusShip.ShipSheild / 200);
			}
			else if (BossStatEnt is FlagshipController)
			{
				BossHullTrans.localScale = new Vector3 (BossHullTrans.localScale.x,(BossBarHeight / BossStatEnt.GetComponent<FlagshipController>().BattlePhase[BossStatEnt.GetComponent<FlagshipController>().ActivePhase].PhaseHull) * BossStatEnt.ShipHull, BossHullTrans.localScale.z);
				float newBossHullX = BossHullX + (((BossStatEnt.ShipHull / BossStatEnt.GetComponent<FlagshipController>().BattlePhase[BossStatEnt.GetComponent<FlagshipController>().ActivePhase].PhaseHull) - 1) * (BossBarHeight / 2));
				BossHullTrans.localPosition = new Vector3(newBossHullX, BossHullTrans.localPosition.y , BossHullTrans.localPosition.z);
				BossHullTrans.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (1 , BossStatEnt.ShipHull / BossStatEnt.GetComponent<FlagshipController>().BattlePhase[BossStatEnt.GetComponent<FlagshipController>().ActivePhase].PhaseHull);
				
				BossSheildTrans.localScale = new Vector3 (BossSheildTrans.localScale.x,(BossBarHeight / 200) * BossStatEnt.ShipSheild, BossSheildTrans.localScale.z);
				float newBossSheildX = BossSheildX + (((BossStatEnt.ShipSheild / 200) - 1) * (BossBarHeight / 2));
				BossSheildTrans.localPosition = new Vector3(newBossSheildX,  BossSheildTrans.localPosition.y , BossSheildTrans.localPosition.z);
				BossSheildTrans.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (1 , FocusShip.ShipSheild / 200);
			}
			else if (BossStatEnt is FlagshipTwinSet)
			{
				//BossHullTrans.localScale = new Vector3 (BossHullTrans.localScale.x,(200 / BossBarHeight) * BossStatEnt.GetComponent<FlagshipTwinSet>().TwinShipHull, BossHullTrans.localScale.z);
				//float newBossHullX = BossHullX + (((BossStatEnt.GetComponent<FlagshipTwinSet>().TwinShipHull / 200) - 1) * (BossBarHeight / 2));
				//BossHullTrans.localPosition = new Vector3(newBossHullX, BossHullTrans.localPosition.y , BossHullTrans.localPosition.z);
				//BossHullTrans.GetComponent<MeshRenderer> ().material.mainTextureScale = new Vector2 (1 , BossStatEnt.GetComponent<FlagshipTwinSet>().TwinShipHull / 200);
			}
			
		}
		else
		{
			BossIconObject.SetActive (false);
		}

	}

	void PauseMenu()
	{
		if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("p")) && !Is_Win && !(FocusShip == null))
		{ 
			Time.timeScale = 0;
			Is_Paused = !Is_Paused;
			if (Is_Paused)
			{
				Cursor.visible = true;
				if (FocusShip != null)
				{
					this.FocusShip.GetComponent<Entity>().enabled = false;
				}
				GameObject.Find("BackGround").GetComponent<AudioSource>().Pause();
			}
			else
			{	
				Time.timeScale = 1;
				Cursor.visible = false;
				if (FocusShip != null)
				{
					this.FocusShip.GetComponent<Entity>().enabled = true;
					GameObject.Find("BackGround").GetComponent<AudioSource>().Play();
				}
			}
			PauseObj.SetActive(Is_Paused);
		}//Menu
	}
	
	void GameOverMenu()
	{
		if (FocusShip == null && !Ended)
		{ 
			Time.timeScale = 0;
			GameObject.Find ("Level_Controller").GetComponent<Level_Controller> ().TimeAdding = false;
			GameOverObj.SetActive(!Is_Paused);
			Cursor.visible = true;
			Ended = true;
		}
	}
	

}
