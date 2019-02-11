using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class CreditController : MonoBehaviour 
{
	public int Level;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
	public void ExitToMenu()
	{
		SceneManager.LoadScene(Level);
	}
}
