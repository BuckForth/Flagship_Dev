using UnityEngine;
using System.Collections;

public class TimeDelayDestroy : MonoBehaviour {

	public bool isActive = false;
	public float Timer = 0.0f;

	void Update () 
	{
		if(isActive)
		{
			if (Timer <= 0)
			{
				Destroy(this.gameObject);
			}
			Timer = Timer - Time.deltaTime;
		}
	}
}
