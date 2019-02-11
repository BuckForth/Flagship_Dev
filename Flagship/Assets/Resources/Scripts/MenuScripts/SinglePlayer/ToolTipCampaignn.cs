using UnityEngine;
using System.Collections;

public class ToolTipCampaignn : MonoBehaviour 
{
	public Mission Mission ;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Mission == null)
		{
			this.transform.Find("Name").GetComponent<TextMesh>().text = "";
			this.transform.Find("Decription").GetComponent<TextMesh>().text = "";
			this.transform.Find("Decription").transform.localPosition = new Vector3(this.transform.Find("Decription").transform.localPosition.x, 0.40f ,this.transform.Find("Decription").transform.localPosition.z);
			this.gameObject.SetActive(false);
		}
		else
		{
			this.transform.Find("Name").GetComponent<TextMesh>().text = Mission.MissonName;
			this.transform.Find("Decription").GetComponent<TextMesh>().text = Mission.Breifing;
		}

	}
}
