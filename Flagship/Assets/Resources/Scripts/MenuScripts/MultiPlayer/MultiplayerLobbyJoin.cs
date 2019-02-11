using UnityEngine;
using System.Collections;

public class MultiplayerLobbyJoin : MonoBehaviour {

	public HostData GameServer;
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
		JoinServer (GameServer);
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


	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}

	void Update()
	{
		this.transform.Find ("Title").GetComponent<TextMesh> ().text = GameServer.gameName;
	}
}
