using UnityEngine;
using System.Collections;

public class MultiplayerStartServer : MonoBehaviour {
	public Color StartColor;
	public Color HoverColor;
	public Color ClickColor;
//	private int LoadRoom = 6;
	private GameObject menuObj;

	// Use this for initialization
	void Start () 
	{

	}

	void OnMouseOver()
	{
		//change text color
		this.GetComponent<SpriteRenderer>().color = HoverColor;
		if (Input.GetButton ("Fire1"))
		{
			this.GetComponent<SpriteRenderer>().color = ClickColor;
		}
		
	}
	
	void OnMouseExit()
	{
		//change text color
		this.GetComponent<SpriteRenderer>().color = StartColor;
	}

	void OnMouseUp()
	{
		this.GetComponentInParent<MultiplayerNewLobby> ().StartNewServer ();
	}
	
	void Update()
	{
		
	}
}
