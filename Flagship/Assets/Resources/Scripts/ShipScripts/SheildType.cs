using UnityEngine;
using System.Collections;


public class SheildType : ScriptableObject
{
	public string Name;
	public int ItemID;

	public float MaxSheild;
	public Color SheildHue;
	public float ChargeRate;
	public float CycleRecharge;
	public float CycleEnergyCost;
	public float CycleHeatUse;
	public string[] Decription;
	public Sprite[] SpriteArray;

}
