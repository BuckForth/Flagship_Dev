using UnityEngine;
using System.Collections;

public class TrainingBot : MonoBehaviour 
{
	public GameObject redShip;
	public GameObject blueShip;

	public GameObject activeRed;
	public GameObject activeBlue;

	public float changeMax = 0.05f;

	public float roundTime = 0f;
	public float roundLength = 10f;

	public TextMesh timerLable;
	public TextMesh redLable;
	public TextMesh blueLable;
	public TextMesh epochLable;

	public float timeVAl = 50f;
	public float hurtVAl = 500f;
	public float damgVAl = 700f;

	public string fileName = "new";

	public int epoch = 0;
	// Use this for initialization
	void Start () 
	{
		NewRound ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		roundTime += Time.deltaTime;
		//Update Gui
		timerLable.text = ("Time: \n" + (Mathf.Round(1000f * roundTime))/1000f);

		if (roundTime > roundLength)
		{
			EndRound ();
		}
	}
	void NewRound()
	{

		activeRed = Instantiate (redShip);
		activeBlue = Instantiate (blueShip);
		NeuroShipController blue = activeBlue.GetComponent<NeuroShipController> ();
		NeuroShipController red = activeRed.GetComponent<NeuroShipController> ();
		blue.brain.loadBrain (("epoch" + epoch));
		red.brain.loadBrain (("epoch" + epoch));
		blue.Player = activeRed;
		red.Player = activeBlue;

		Tweak (red.brain);

		epoch++;
		epochLable.text = "Epoch:\n" + epoch;
		roundTime = 0f;
	}

	void Tweak(NeuroNet nNetwork)
	{
		foreach (NeuroNetNode node in nNetwork.hiddenLayer)
		{
			for (int ii = 0; ii < node.layerMultiplyers.Length; ii++)
			{
				node.layerMultiplyers [ii] += (Random.value * changeMax)-(changeMax / 2f);
			}
		}
		foreach (NeuroNetNode node in nNetwork.hiddenLayer2)
		{
			for (int ii = 0; ii < node.layerMultiplyers.Length; ii++)
			{
				node.layerMultiplyers [ii] += (Random.value * (changeMax/2f))-(changeMax / 4f);
			}
		}
		foreach (NeuroNetNode node in nNetwork.inputLayer)
		{
			for (int ii = 0; ii < node.layerMultiplyers.Length; ii++)
			{
				node.layerMultiplyers [ii] += (Random.value * (changeMax/8f))-(changeMax / 16f);
			}
		}
	}

	public void EndRound()
	{
		float scoreBlue = 0f;
		float scoreRed = 0f;
		NeuroShipController blue = activeBlue.GetComponent<NeuroShipController> ();
		NeuroShipController red = activeRed.GetComponent<NeuroShipController> ();
		scoreBlue = (roundTime * timeVAl * (1 - (blue.ShipHull/blue.Shiptype.ShipHull))) + ((blue.ShipHull/blue.Shiptype.ShipHull) * hurtVAl) + ((1f-(red.ShipHull/red.Shiptype.ShipHull)) * damgVAl) + (200f - blue.ShipHeat) + (red.hits * 200) + (( blue.transform.position.y - red.transform.position.y ) * 50);
		scoreRed = (roundTime * timeVAl) + ((red.ShipHull/red.Shiptype.ShipHull) * hurtVAl) + ((1f-(blue.ShipHull/blue.Shiptype.ShipHull)) * damgVAl) + (200f - red.ShipHeat) + (blue.hits * 200)  + (( blue.transform.position.y - red.transform.position.y ) * 50);

		if (scoreBlue > scoreRed)
		{
			blue.brain.saveBrain ("epoch" + epoch);
			fileName = ("epoch" + epoch);
			redLable.text = "Score:\n" + scoreRed;
			blueLable.text = "Score:\n" + scoreBlue;
		}
		else
		{
			red.brain.saveBrain ("epoch" + epoch);
			fileName = ("epoch" + epoch);
			redLable.text = "Score:\n" + scoreRed;
			blueLable.text = "Score:\n" + scoreBlue;
		}
		Destroy (activeBlue);
		Destroy (activeRed);
		NewRound ();
	}
}
