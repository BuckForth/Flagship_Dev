using UnityEngine;
using System.Collections;

public class HudAdapt : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		ShipController[] allShips = FindObjectsOfType<ShipController> ();
		foreach (ShipController ship in allShips)
		{
			if (ship.gameObject.GetComponent<NetworkView> ().isMine)
			{
				GetComponent<HudController> ().FocusShip = ship;
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
