using UnityEngine;
using System.Collections;

public class HangerMenuJump : MonoBehaviour {

	public string OpenMenu;


	// Use this for initialization
	void Start () 
	{
	
	}

	void OnMouseEnter()
	{

	}

	void OnMouseExit()
	{

	}

	void OnMouseDown()
	{
		//is this quit
		if (this.transform.Find(OpenMenu).gameObject.activeSelf == false) 
		{
			HangerMenuJump[] HangerPanels = FindObjectsOfType<HangerMenuJump>() as HangerMenuJump[];
			foreach (HangerMenuJump Panel in HangerPanels) 
			{
				Panel.transform.Find(Panel.OpenMenu).gameObject.SetActive(false);
			}
			this.transform.Find(OpenMenu).gameObject.SetActive(true);
		}

	}
}
