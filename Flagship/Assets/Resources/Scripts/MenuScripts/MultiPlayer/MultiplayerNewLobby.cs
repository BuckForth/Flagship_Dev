using UnityEngine;
using UnityEngine.UI; 
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MultiplayerNewLobby : MonoBehaviour {
	public Color StartColor;
	public Color HoverColor;
	public Color ClickColor;
//	private int LoadRoom = 6;
	private string serverName = "New Lobby";
	private GameObject menuObj;

	// Use this for initialization
	void Start () 
	{
		menuObj = this.transform.Find ("Menu").gameObject;
		menuObj.SetActive (false);
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
		menuObj.SetActive (true);
	}

	public void UpdateServerName ()
	{
		serverName = this.transform.Find ("Menu/ServerName/InputField").GetComponent<InputField>().text;
		UpdateNameBoxPlaceHolder ();
	}

	public void UpdateNameBoxPlaceHolder ()
	{
		this.transform.Find ("Menu/ServerName/InputField").GetComponent<InputField>().text = serverName;
	}
	 
	public void StartNewServer()
	{
		this.GetComponentInParent<MultiPlayerController> ().StartServer (serverName);
	}
	void Update()
	{
		
	}
}
