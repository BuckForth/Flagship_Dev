using UnityEngine;
using System.Collections;


public class ShipType : ScriptableObject
{
	public string Name;
	public int ItemID;

	public float ShipSpeed;
	public float ShipHull;
	public float OverHeatThreshold;
	public float FreezeThreshold;
	public Vector2 Shipsize;
	public string[] Decription;
	public Sprite[] SpriteSet;
	public Sprite[] SpriteSecondSet;
	public Sprite[] SpriteCockpitSet;

	Sprite GetSprite (int SpriteIndex)
	{
		return SpriteSet[SpriteIndex];
	}
}
