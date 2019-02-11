using UnityEngine;
using System.Collections;

public class NetworkLossReducer : MonoBehaviour 
{
	
	public string netName = "AceWing";
	public NetState[] sampleGroup;
	public int sampleSize = 230;

	public NeuroNet workingNet = new NeuroNet(10,8,4);
	public NeuroNet TrialNet = new NeuroNet(10,8,4);

	public int stepsPerFrame = 10;
	public float changeMax = 0.01f;
	public int Epoch = 0;
	// Use this for initialization
	void Start () 
	{
		workingNet = new NeuroNet(10,8,4);
		TrialNet = new NeuroNet(10,8,4);
		loadSamples ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		Epoch++;
		for (int ii = 0; ii < stepsPerFrame; ii++)
		{
			TrialNet = workingNet.copy ();
			Tweak (TrialNet);

			if (calculateLoss (TrialNet) < calculateLoss (workingNet))
			{
				workingNet = TrialNet.copy ();
			}
		}
		Debug.Log (calculateLoss (workingNet));

		workingNet.saveBrain (netName + Epoch);
	}

	public void loadSamples ()
	{
		sampleGroup = new NetState[sampleSize];
		for (int ii = 0; ii < sampleSize; ii++)
		{
			sampleGroup [ii] = NetState.loadState ("sample" + (ii + 1));
		}
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

	public float calculateLoss(NeuroNet nNetwork)
	{
		float totalLoss = 0f;
		for (int ii = 0; ii < sampleGroup.Length; ii++)
		{
			NetState iState = sampleGroup [ii];
			nNetwork.inputValues = new float[]{iState.inputs.healthRatio,iState.inputs.sheildRatio,iState.inputs.tempRatio,iState.inputs.powerRatio,iState.inputs.xRatio,iState.inputs.yRatio,iState.inputs.xERatio,iState.inputs.yERatio,iState.inputs.xSRatio,iState.inputs.ySRatio};
			nNetwork.updateStep ();
			float stateLoss = 0f;
			stateLoss += Mathf.Pow(Mathf.Abs (iState.outputs.xTargRatio - nNetwork.outputLayer[0])*2f,2f);
			stateLoss += Mathf.Pow(Mathf.Abs (iState.outputs.yTargRatio - nNetwork.outputLayer[1])*2f,2f);
			if ((iState.outputs.main && (nNetwork.outputLayer [2] < 0f)) || (!iState.outputs.main && (nNetwork.outputLayer [2] > 0f)))
			{
				stateLoss += 1f;
			}
			if ((iState.outputs.wing && (nNetwork.outputLayer [2] < 0f)) || (!iState.outputs.wing && (nNetwork.outputLayer [2] > 0f)))
			{
				stateLoss += 1f;
			}

			stateLoss = stateLoss / 4f;
			totalLoss += stateLoss;
		}
		return totalLoss / ((float)sampleGroup.Length);
	}
}
