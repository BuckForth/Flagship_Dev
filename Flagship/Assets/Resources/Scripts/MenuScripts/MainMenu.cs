using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour {
	public bool isQuit;
	public bool ChangeRooms;
	public Color StartColor;
	public Color HoverColor;
	public Color ClickColor;
	public int LoadRoom;

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
		//is this quit
		if (isQuit==true) {
			//quit the game
			Application.Quit();
		}
		else 
		{
			if (ChangeRooms)
			{
				//load level
				SceneManager.LoadScene(LoadRoom);
			}

		}
	}
	
	void Update()
	{

	}
}
