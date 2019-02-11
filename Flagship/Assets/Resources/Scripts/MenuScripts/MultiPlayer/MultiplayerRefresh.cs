using UnityEngine;
using System.Collections;

public class MultiplayerRefresh : MonoBehaviour 
{
	
	public Color StartColor;
	public Color HoverColor;
	public Color ClickColor;
	
	
	// Use this for initialization
	void Start () 
	{
		Update ();
	}
	
	void OnMouseUp ()
	{
		GameObject.Find ("LobbyManager").GetComponent<MultiPlayerController> ().RefreshHostList ();
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
	
	
	void Update()
	{

	}
}
