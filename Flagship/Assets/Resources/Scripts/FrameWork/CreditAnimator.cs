using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class CreditAnimator : MonoBehaviour 
{
	public ShipType shipType;
	public string fileName;



	public TransformSet[] movements;
	public float[] MoveTimes;
	public int MoveRepeatFrom = 0;



	private Color32 mainColor;
	private Color32 secondaryColor;
	private Color32 cockpitColor;

	[HideInInspector]
	public Vector3 startPos;
	[HideInInspector]
	public float Move_timer = 0;
	[HideInInspector]
	public int Move_order = 0;
	
	void Start () 
	{
		startPos = this.transform.position;
		LoadShip();
	}
	
	void Update () 
	{	

		Move_timer += Time.deltaTime / 2;
		if (Move_timer > MoveTimes[Move_order])
		{
			Move_timer = 0;
			Move_order++;
			startPos = this.transform.position;
			if (Move_order == MoveTimes.Length)
			{
				Move_order = MoveRepeatFrom;
			}
		}
		Move (movements [Move_order]);

		UpdateColor ();
	}

	void UpdateColor()
	{
		this.transform.Find ("Main").GetComponent<SpriteRenderer> ().color = mainColor;
		this.transform.Find ("Second").GetComponent<SpriteRenderer> ().color = secondaryColor;
		this.transform.Find ("Cockpit").GetComponent<SpriteRenderer> ().color = cockpitColor;
	}

	void Move(TransformSet movement)
	{
		Vector3 OldPos = this.transform.position;
		this.transform.position = movement.GetPosition(startPos,Move_timer);
		RotateShip (OldPos , this.transform.position);
	}

	void LoadShip()
	{
		if (File.Exists (Application.persistentDataPath + "/" + fileName)) 
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/" + fileName , FileMode.Open);
			PlayerShip LoadShip = (PlayerShip)bf.Deserialize(file);
			shipType = Resources.Load("ShipParts/ShipsTypes/" + LoadShip.shiptype) as ShipType;
			mainColor = LoadShip.maincolor.GetColor();
			secondaryColor = LoadShip.secondarycolor.GetColor();
			cockpitColor = LoadShip.cockpitcolor.GetColor();
			file.Close();
			Debug.Log ("Load Complete");
		}
		else
		{
			Debug.Log ("No Saved File");
		}
	}

	public void RotateShip(Vector3 OldLoc , Vector3 NewLoc)
	{
		float ShipRotation = ((OldLoc.x - NewLoc.x) * 16f) / Time.deltaTime;
		int NewSprite = 2;
		if (ShipRotation < -4) 
		{
			NewSprite = 3;
		}
		if (ShipRotation < -8) 
		{
			NewSprite = 4;
		}
		if (ShipRotation > 4) 
		{
			NewSprite = 1;
		}
		if (ShipRotation > 8) 
		{
			NewSprite = 0;
		}
		this.transform.Find ("Main").GetComponent<SpriteRenderer> ().sprite = shipType.SpriteSet[NewSprite];
		this.transform.Find ("Second").GetComponent<SpriteRenderer> ().sprite = shipType.SpriteSecondSet[NewSprite];
		this.transform.Find ("Cockpit").GetComponent<SpriteRenderer> ().sprite = shipType.SpriteCockpitSet[NewSprite];
	}

}