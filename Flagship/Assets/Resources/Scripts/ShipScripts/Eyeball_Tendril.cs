using UnityEngine;
using System.Collections;

public class Eyeball_Tendril : MonoBehaviour 
{
	public float distance;
	public float speed;
	public float offset;
	public float destroyDelay;
	public Color darkened;
	public GameObject Explosion;
	public Eyeball_Claw handGO;
	private float startX;
	private float personalTime = 0;
	private bool isAlive = true;


	void Start () 
	{
		startX = transform.position.x;
	}
	

	void Update () 
	{
		if (isAlive)
		{ 
			personalTime += Time.deltaTime;
			if (handGO.isAlive)
			{
				transform.position = new Vector3 (startX + (distance * Mathf.Sin (((speed * personalTime) / Mathf.PI) + (offset / Mathf.PI))), transform.position.y);
			}
			if (!handGO.isAlive && (personalTime - handGO.getTime ()) > destroyDelay)
			{
				ShipExplode ();
			}
		}
	}

	void ShipExplode()
	{
		if (Explosion != null)
		{
			GameObject Explosion_Spawned = (GameObject)Instantiate(Explosion);
			Explosion_Spawned.transform.position = this.transform.position;
		}
		transform.GetComponent<SpriteRenderer> ().color = darkened;
		this.isAlive = false;
	}

}
