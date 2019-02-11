using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PlayerInfoWidget : MonoBehaviour 
{
	public bool isHost = false;
	public string shipName;
	public ShipType shiptype;
	public SheildType sheildtype;
	public RadiatorType radiatortype;
	public GeneratorType generatortype;
	public WeaponType mainWeapon;
	public WeaponType wingWeapon;

	public Color32 mainColor;
	public Color32 secondaryColor;
	public Color32 cockpitColor;

	public float updateTime = 0.25f;
	private float count = 0f;
	void Awake ()
	{
		DontDestroyOnLoad (this);
	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		count += Time.deltaTime;
		transform.Find ("ShipName").GetComponent<TextMesh> ().text = shipName;
		transform.Find ("ShipType").GetComponent<TextMesh> ().text = shiptype.Name;
		transform.Find ("MainWeapon").GetComponent<TextMesh> ().text = mainWeapon.Name;
		transform.Find ("WingWeapon").GetComponent<TextMesh> ().text = wingWeapon.Name;

		transform.Find ("Main").GetComponent<SpriteRenderer> ().sprite = shiptype.SpriteSet[2];
		transform.Find ("Second").GetComponent<SpriteRenderer> ().sprite = shiptype.SpriteSecondSet[2];
		transform.Find ("Cockpit").GetComponent<SpriteRenderer> ().sprite = shiptype.SpriteCockpitSet[2];

		transform.Find ("Main").GetComponent<SpriteRenderer> ().color = mainColor;
		transform.Find ("Second").GetComponent<SpriteRenderer> ().color = secondaryColor;
		transform.Find ("Cockpit").GetComponent<SpriteRenderer> ().color = cockpitColor;


		if (GetComponent<NetworkView> ().isMine && count >= updateTime)
		{
			count -= updateTime;
			sendShipStats ();
		}

	}


	void sendShipStats ()
	{
		GetComponent<NetworkView>().RPC("reciveShipStats", RPCMode.OthersBuffered, shipName);
	}

	[RPC] void reciveShipStats (string nameOfShip)
	{
		shipName = nameOfShip;
	}


	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncPosition = Vector3.zero;
		Vector3 syncMain = new Vector3 (mainColor.r / 255f, mainColor.g  / 255f, mainColor.b / 255f);
		Vector3 syncSecondary = new Vector3 (secondaryColor.r / 255f, secondaryColor.g / 255f , secondaryColor.b / 255f);
		Vector3 syncCockpit = new Vector3 (cockpitColor.r / 255f, cockpitColor.g / 255f , cockpitColor.b / 255f);

		int syncShip = shiptype.ItemID;
		int syncSheild = sheildtype.ItemID;
		int syncRad = radiatortype.ItemID;
		int syncGen = generatortype.ItemID;
		int syncMainWeap = mainWeapon.ItemID;
		int syncWingWeap = wingWeapon.ItemID;

		if (stream.isWriting)
		{
			stream.Serialize(ref syncMain);
			stream.Serialize(ref syncSecondary);
			stream.Serialize(ref syncCockpit);

			stream.Serialize(ref syncShip);
			stream.Serialize(ref syncSheild);
			stream.Serialize(ref syncRad);
			stream.Serialize(ref syncGen);
			stream.Serialize(ref syncMainWeap);
			stream.Serialize(ref syncWingWeap);
			syncPosition = transform.position;
			stream.Serialize(ref syncPosition);
		}
		else
		{
			ItemIDNET targetlist = Resources.Load ("Scripts/FrameWork/MasterID") as ItemIDNET;

			stream.Serialize(ref syncMain);
			stream.Serialize(ref syncSecondary);
			stream.Serialize(ref syncCockpit);
			mainColor = new Color(syncMain.x, syncMain.y, syncMain.z, 1f);
			secondaryColor = new Color(syncSecondary.x, syncSecondary.y, syncSecondary.z, 1f);
			cockpitColor = new Color(syncCockpit.x, syncCockpit.y, syncCockpit.z, 1f);

			stream.Serialize(ref syncShip);
			stream.Serialize(ref syncSheild);
			stream.Serialize(ref syncRad);
			stream.Serialize(ref syncGen);
			stream.Serialize(ref syncMainWeap);
			stream.Serialize(ref syncWingWeap);

			shiptype = targetlist.ShipID [syncShip];
			sheildtype = targetlist.SheildID [syncSheild];
			radiatortype = targetlist.RadiatorID [syncRad];
			generatortype = targetlist.GeneratorID [syncGen];
			mainWeapon = targetlist.WeaponID [syncMainWeap];
			wingWeapon = targetlist.WeaponID [syncWingWeap];

			stream.Serialize(ref syncPosition);
			transform.position = syncPosition;
		}
	}

}
