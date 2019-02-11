using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class MultiPlayerController : MonoBehaviour {
	
	private const string typeName = "FlagShip";
	public GameObject playerPrefab;
	private const string gameName = "RoomName";
	public string fileName = "PlayerShip.fs";
//	private string RoomName = "";
	public int LobbyRoomID = 0;
	public GameObject LobbyObject;

	[HideInInspector]
	private HostData[] hostList;
	[HideInInspector]
	public HostData SelectedServer = null;



	public void RefreshHostList()
	{
		Debug.Log ("Refreshing List");
		MasterServer.RequestHostList(typeName);
		hostList = MasterServer.PollHostList();
	}
	 
	void DrawHostList()
	{
		MultiplayerLobbyJoin[] OldLobbys = FindObjectsOfType<MultiplayerLobbyJoin>() as MultiplayerLobbyJoin[];
		foreach (MultiplayerLobbyJoin OldLobby in OldLobbys) 
		{
			Destroy(OldLobby.gameObject);
		}
		if (hostList != null)
		{
			for (int i = 0; i < hostList.Length; i++)
			{
				Debug.Log ("Found Host");
				GameObject NewLobby = Instantiate(LobbyObject);

				NewLobby.GetComponent<MultiplayerLobbyJoin>().GameServer = hostList[i];
				NewLobby.transform.SetParent(this.transform.Find("RightPanel/LobbyList").transform);
				NewLobby.transform.localPosition = new Vector3(0f, 3 + (i * - 1.3f ) , 0f);
			}
		}
	}

	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
		{
			hostList = MasterServer.PollHostList();
			DrawHostList ();
		}
	}


	public void StartServer(string newGameName)
	{
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, newGameName);
	}
	
	void OnServerInitialized()
	{

		Debug.Log("Server Initializied");

		GameObject Player = Network.Instantiate (playerPrefab, new Vector3 (-7.8f, 3.15f - 1.8f, 0f), Quaternion.identity, 0) as GameObject;
		PlayerInfoWidget PlayerForm = Player.GetComponent<PlayerInfoWidget> ();

		if (File.Exists (Application.persistentDataPath + "/" + fileName)) 
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/" + fileName , FileMode.Open);
			PlayerShip LoadShip = (PlayerShip)bf.Deserialize(file);
			PlayerForm.shiptype = Resources.Load("ShipParts/ShipsTypes/" + LoadShip.shiptype) as ShipType;
			PlayerForm.sheildtype = Resources.Load("ShipParts/SheildTypes/" + LoadShip.sheildtype) as SheildType;
			PlayerForm.radiatortype = Resources.Load("ShipParts/RadiatorTypes/" + LoadShip.radiatortype) as RadiatorType;
			PlayerForm.generatortype = Resources.Load("ShipParts/GeneratorTypes/" + LoadShip.generatortype) as GeneratorType;
			PlayerForm.mainWeapon = Resources.Load("ShipParts/WeaponTypes/" + LoadShip.mainweaponname) as WeaponType;
			PlayerForm.wingWeapon = Resources.Load("ShipParts/WeaponTypes/" + LoadShip.wingweaponname) as WeaponType;
			PlayerForm.mainColor = LoadShip.maincolor.GetColor();
			PlayerForm.secondaryColor = LoadShip.secondarycolor.GetColor();
			PlayerForm.cockpitColor = LoadShip.cockpitcolor.GetColor();
			PlayerForm.shipName = LoadShip.shipname;
			file.Close();
			Debug.Log ("Load Complete");
		}
		else
		{
			Debug.Log ("No Saved File");
		}
		SceneManager.LoadScene(LobbyRoomID);
	}
	
	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}
	
	void OnConnectedToServer()
	{
		Debug.Log("Server Joined");
		GameObject Player = Network.Instantiate (playerPrefab, new Vector3 (-7.8f, 3.15f, 0f), Quaternion.identity, 0) as GameObject;
		PlayerInfoWidget PlayerForm = Player.GetComponent<PlayerInfoWidget> ();

		if (File.Exists (Application.persistentDataPath + "/" + fileName)) 
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/" + fileName , FileMode.Open);
			PlayerShip LoadShip = (PlayerShip)bf.Deserialize(file);
			PlayerForm.shiptype = Resources.Load("ShipParts/ShipsTypes/" + LoadShip.shiptype) as ShipType;
			PlayerForm.sheildtype = Resources.Load("ShipParts/SheildTypes/" + LoadShip.sheildtype) as SheildType;
			PlayerForm.radiatortype = Resources.Load("ShipParts/RadiatorTypes/" + LoadShip.radiatortype) as RadiatorType;
			PlayerForm.generatortype = Resources.Load("ShipParts/GeneratorTypes/" + LoadShip.generatortype) as GeneratorType;
			PlayerForm.mainWeapon = Resources.Load("ShipParts/WeaponTypes/" + LoadShip.mainweaponname) as WeaponType;
			PlayerForm.wingWeapon = Resources.Load("ShipParts/WeaponTypes/" + LoadShip.wingweaponname) as WeaponType;
			PlayerForm.mainColor = LoadShip.maincolor.GetColor();
			PlayerForm.secondaryColor = LoadShip.secondarycolor.GetColor();
			PlayerForm.cockpitColor = LoadShip.cockpitcolor.GetColor();
			PlayerForm.shipName = LoadShip.shipname;
			file.Close();
			Debug.Log ("Load Complete");
		}
		else
		{
			Debug.Log ("No Saved File");
		}
		SceneManager.LoadScene(LobbyRoomID);
	}

	void OnFailedToConnect()
	{

	}

	void OnFailedToConnectToMasterServer()
	{

	}
	
	// Use this for initialization
	void Start () 
	{
		RefreshHostList ();	
	}
	
	// Update is called once per frame
	void Update () 
	{

	}


}
