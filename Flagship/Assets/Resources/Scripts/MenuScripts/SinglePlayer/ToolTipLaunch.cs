using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ToolTipLaunch : MonoBehaviour {
	public GameObject ToolTip;

	public GameObject LoadCover;


	
	
	
	// Use this for initialization
	void Start () 
	{
		
	}

	
	void OnMouseUp()
	{

		GameObject LoadScreen = Instantiate(LoadCover);
		LoadScreen.transform.localPosition = new Vector3(0f, 0f , 0f);

		SceneManager.LoadScene(ToolTip.GetComponent<ToolTipCampaignn> ().Mission.LoadLevel);

;
	}




}
