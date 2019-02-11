using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MultiplayerStartGame : MonoBehaviour {

	public Color StartColor;
	public Color HoverColor;
	public Color ClickColor;

	// Use this for initialization
	void Start () 
	{
		if (Network.isClient)
		{
			this.GetComponent<SpriteRenderer> ().color = Color.gray;
		}
	}

	void OnMouseOver()
	{
		if (Network.isServer)
		{
			this.GetComponent<SpriteRenderer> ().color = HoverColor;
			if (Input.GetButton ("Fire1"))
			{
				this.GetComponent<SpriteRenderer> ().color = ClickColor;
			}
		}
	}

	void OnMouseExit()
	{
		if (Network.isServer)
		{
			this.GetComponent<SpriteRenderer> ().color = StartColor;
		}
	}

	void OnMouseUp()
	{
		if (Network.isServer)
		{
			Network.Disconnect ();
		}
	}

	void Update()
	{
		if (Network.isClient)
		{
			this.GetComponent<SpriteRenderer> ().color = Color.gray;
		}
	}
}