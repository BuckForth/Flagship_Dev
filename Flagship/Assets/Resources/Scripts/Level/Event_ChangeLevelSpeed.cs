using UnityEngine;
using System.Collections;

public class Event_ChangeLevelSpeed : MonoBehaviour 
{
	private GameObject Level_ControllerOBJ;
	public float TriggerTime = 0.0f;
	public float NewSpeed = 0.5f;
	
	// Use this for initialization
	void Start () 
	{
				Level_ControllerOBJ = GameObject.Find ("Level_Controller");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (TriggerTime < Level_ControllerOBJ.GetComponent<Level_Controller>().LevelTime)
		{
			Level_ControllerOBJ.GetComponent<Level_Controller>().timeScale = NewSpeed;
			Destroy(this.gameObject);
		}
		
	}
	
}
