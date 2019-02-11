using UnityEngine;
using System.Collections;

public class Event_DisplayMessage : MonoBehaviour 
{
	private GameObject Level_ControllerOBJ;
	public float TriggerTime = 0.0f;
	public float MessageTime = 0.5f;
	[TextArea(3,10)]
	public string Message;
	[HideInInspector]
	public float timercount = 0.0f;
	[HideInInspector]
	public bool isactive = false;
	
	// Use this for initialization
	void Start () 
	{
		timercount = 0.0f;
		Level_ControllerOBJ = GameObject.Find ("Level_Controller");

	}
	
	// Update is called once per frame
	void Update () 
	{
		if (TriggerTime < Level_ControllerOBJ.GetComponent<Level_Controller>().LevelTime && !isactive)
		{
			GameObject.Find("Hud").GetComponent<HudController>().MessageObject.SetActive(true);
			GameObject.Find("Hud").GetComponent<HudController>().MessageObject.GetComponentInChildren<TextMesh>().text = Message;
			isactive = true;

		}

		if (isactive)
		{
			timercount += Time.deltaTime;
		}

		if (timercount > MessageTime)
		{
			GameObject.Find("Hud").GetComponent<HudController>().MessageObject.SetActive(false);
			Destroy(this.gameObject);
		}

	}
	
}
