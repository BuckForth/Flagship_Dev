using UnityEngine;
using System.Collections;


public class Formation : ScriptableObject
{
	public ArrayLayout shipLayout;
	public TransformSet shipMoves;

	public TransformSet shipMovesByCenter;
	public bool ambush = false; //SETTING TO TRUE CAUSE ATTACK FROM BELOW

	public int[,] getShipArray()
	{
		return shipLayout.getArray ();
	}
}