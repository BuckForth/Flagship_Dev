using UnityEngine;
using System.Collections;

public class TimeDelayStart : MonoBehaviour 
{
	private GameObject Level_ControllerOBJ;
	public MonoBehaviour startScript;
	public float TriggerTime = 0.0f;

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
			startScript.enabled = true;
			this.enabled = false;
		}
	}
}
