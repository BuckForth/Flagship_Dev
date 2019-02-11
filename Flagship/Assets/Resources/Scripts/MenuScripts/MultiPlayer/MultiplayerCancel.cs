using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MultiplayerCancel : MonoBehaviour {

	public Color StartColor;
	public Color HoverColor;
	public Color ClickColor;

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
		Network.Disconnect ();
	}

	void Update()
	{

	}
}