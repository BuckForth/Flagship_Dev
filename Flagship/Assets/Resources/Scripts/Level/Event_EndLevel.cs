using UnityEngine;
using System.Collections;

public class Event_EndLevel : MonoBehaviour 
{
	private GameObject Level_ControllerOBJ;
	public float TriggerTime = 0.0f;
	
	[HideInInspector]
	public bool isactive = false;
	
	// Use this for initialization
	void Start () 
	{
				Level_ControllerOBJ = GameObject.Find ("Level_Controller");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (TriggerTime < Level_ControllerOBJ.GetComponent<Level_Controller>().LevelTime && !isactive)
		{
			GameObject.Find("Hud").GetComponent<HudController>().ActiveateWin();
			Destroy(this.gameObject);
		}
	}
	
}
