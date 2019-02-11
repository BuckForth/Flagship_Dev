using UnityEngine;
using System.Collections;

public class Magnetic : MonoBehaviour {
	public float strength = 0;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Rigidbody thisBody = this.GetComponent<Rigidbody>();
		Rigidbody[] bodys = FindObjectsOfType<Rigidbody>() as Rigidbody[];
		foreach (Rigidbody body in bodys) 
		{
			if (body.gameObject.GetComponent<Magnetic>() != null && body != thisBody && body.gameObject.layer != thisBody.gameObject.layer )
			{
				body.AddRelativeForce((body.transform.position - thisBody.transform.position).normalized * (-strength / (body.transform.position - thisBody.transform.position).magnitude));
			}
			if (body.gameObject.GetComponent<Magnetic>() == null && body != thisBody && body.gameObject.layer != thisBody.gameObject.layer )
			{
				body.AddRelativeForce((body.transform.position - thisBody.transform.position).normalized * (strength / (body.transform.position - thisBody.transform.position).magnitude));
			}
		}
	}
}
