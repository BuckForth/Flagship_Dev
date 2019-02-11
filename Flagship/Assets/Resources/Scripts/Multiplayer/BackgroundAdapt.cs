using UnityEngine;
using System.Collections;

public class BackgroundAdapt : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		ShipController[] allShips = FindObjectsOfType<ShipController> ();
		foreach (ShipController ship in allShips)
		{
			if (ship.gameObject.GetComponent<NetworkView> ().isMine)
			{
				TimeBaseTrack[] backgroundLayers = GetComponentsInChildren<TimeBaseTrack> ();
				foreach (TimeBaseTrack backGroundLayer in backgroundLayers)
				{
					backGroundLayer.Player = ship.gameObject;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
}
