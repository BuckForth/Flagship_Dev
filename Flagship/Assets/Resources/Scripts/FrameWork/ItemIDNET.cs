using UnityEngine;
using System.Collections;

public class ItemIDNET : ScriptableObject
{
	public ShipType[] ShipID = new ShipType[64];
	public SheildType[] SheildID = new SheildType[64];
	public RadiatorType[] RadiatorID = new RadiatorType[64];
	public GeneratorType[] GeneratorID = new GeneratorType[64];
	public WeaponType[] WeaponID = new WeaponType[128];
}
