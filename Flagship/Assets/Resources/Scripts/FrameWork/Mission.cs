using UnityEngine;
using System.Collections;

public class Mission : ScriptableObject
{
	public string MissonName;
	[TextArea(3,10)]
	public string Breifing;
	public int BreifingLength;
	public int LoadLevel;
}
