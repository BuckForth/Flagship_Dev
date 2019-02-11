using UnityEngine;
using System.Collections;

public class LevelScroll : MonoBehaviour 
{
	public float Speed = 0.0f;
	public bool IsMoving = true;
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 Oldpos = this.transform.position;
		if (IsMoving) 
		{
			transform.position = new Vector3(0 ,Oldpos.y + (Time.deltaTime * -Speed) ,0f);
		}
	}
}
