using UnityEngine;
using System.Collections;

public class TimeBaseTrack : MonoBehaviour {
	public GameObject Player;
	public float xbaseMove = 0.0f;
	public float xMid = -3.4f;
	public float Yspeed = 0.0f;
	public float Xspeed = 0.0f;
	public bool hasNorm = true;
	[HideInInspector]
	public float timestamp = 0.0f;
	[HideInInspector]
	public bool MovingPicture = true;
	//private Vector2 savedOffset;
	// Use this for initialization
	void Start () 
	{
		Vector2 offset = new Vector2 (0 , 0);
		GetComponent<Renderer>().sharedMaterial.SetTextureOffset ("_MainTex" , offset);
		//savedOffset = GetComponent<Renderer>().sharedMaterial.GetTextureOffset ("_MainTex");
		Update ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Player != null)
		{
			if (MovingPicture)
			{
				timestamp = GameObject.Find ("Level_Controller").GetComponent<Level_Controller> ().LevelTime;
			}
			float XMove = ((Player.transform.position.x - xMid) * xbaseMove) / (GetComponent<Renderer> ().sharedMaterial.GetTextureScale ("_MainTex").x * 50.0f) ;

			float xx = Mathf.Repeat (timestamp * (Xspeed * GetComponent<Renderer> ().sharedMaterial.GetTextureScale ("_MainTex").x) + XMove, 1);
			float yy = Mathf.Repeat (timestamp * (Yspeed * GetComponent<Renderer> ().sharedMaterial.GetTextureScale ("_MainTex").y), 1);
			Vector2 offset = new Vector2 (xx, yy);
			//this.transform.localScale = new Vector3(Quadsize,Quadsize,0);

			GetComponent<Renderer> ().sharedMaterial.SetTextureOffset ("_MainTex", offset);
			if (hasNorm)
			{
				GetComponent<Renderer> ().sharedMaterial.SetTextureOffset ("_BumpMap", offset);
			}
		}
	}
}
