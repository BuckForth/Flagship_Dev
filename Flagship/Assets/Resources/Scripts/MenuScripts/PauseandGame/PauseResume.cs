using UnityEngine;
using System.Collections;

public class PauseResume : MonoBehaviour {
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
		this.GetComponent<SpriteRenderer>().color = StartColor;
		GameObject.Find ("Hud").GetComponent<HudController> ().Is_Paused = !GameObject.Find ("Hud").GetComponent<HudController> ().Is_Paused;
		GameObject.Find ("Hud").GetComponent<HudController> ().PauseObj.SetActive(GameObject.Find ("Hud").GetComponent<HudController> ().Is_Paused);
		if (GameObject.Find ("Hud").GetComponent<HudController> ().Is_Paused == true)
		{
			Cursor.visible = true;
			GameObject.Find ("Hud").GetComponent<HudController> ().FocusShip.GetComponent<Entity>().enabled = false;
			Time.timeScale = 0;
		}
		else
		{	
			Cursor.visible = false;
			GameObject.Find ("Hud").GetComponent<HudController> ().FocusShip.GetComponent<Entity>().enabled = true;
			Time.timeScale = 1;
		}
	}
	
	void Update()
	{

	}
}
