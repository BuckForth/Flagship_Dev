using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class PregameLobby : MonoBehaviour 
{
	public int playerCount = 0;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		PlayerInfoWidget[] widgets = FindObjectsOfType<PlayerInfoWidget> ();
		int count = 0;
		foreach (PlayerInfoWidget widget in widgets)
		{
			if (widget.gameObject.GetComponent<NetworkView> ().isMine)
			{
				widget.gameObject.transform.position = new Vector3 (-7.8f,3.15f - (1.8f * count),0f);
				widget.gameObject.name = "Player_" + (count + 1);
			}
		}
	}

	void OnDisconnectedFromServer ()
	{
		PlayerInfoWidget[] widgets = FindObjectsOfType<PlayerInfoWidget> ();
		foreach (PlayerInfoWidget widget in widgets)
		{
			if (widget.gameObject.GetComponent<NetworkView> ().isMine)
			{
				Destroy (widget.gameObject);
			}
		}
		SceneManager.LoadScene (4);
	}
}
