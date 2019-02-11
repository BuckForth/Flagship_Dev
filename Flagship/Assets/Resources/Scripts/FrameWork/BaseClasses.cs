using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
public class TransformSet
{
	public bool Xcos = false;
	public float XSigma = 0f;
	public float XSinMulti = 0f;
	public float Xlin = 0f;
	public float Xsq = 0f;
	public float Xcu = 0f;
	
	public bool Ycos = false;
	public float YSigma = 0f;
	public float YSinMulti = 0f;
	public float Ylin = 0f;
	public float Ysq = 0f;
	public float Ycu = 0f;

	public TransformSet(TransformSet copySet)
	{
		Xcos = copySet.Xcos;
		XSigma = copySet.XSigma;
		XSinMulti = copySet.XSinMulti;
		Xlin = copySet.Xlin;
		Xsq = copySet.Xsq;
		Xcu = copySet.Xcu;

		Ycos = copySet.Ycos;
		YSigma = copySet.YSigma;
		YSinMulti = copySet.YSinMulti;
		Ylin = copySet.Ylin;
		Ysq = copySet.Ysq;
		Ycu = copySet.Ycu;
	}

	public void tMultiply (float val)
	{
		XSigma = XSigma * val;
		XSinMulti = XSinMulti * val;
		Xlin = Xlin * val;
		Xsq = Xsq * val;
		Xcu = Xcu * val;

		YSigma = YSigma * val;
		YSinMulti = YSinMulti * val;
		Ylin = Ylin * val;
		Ysq = Ysq * val;
		Ycu = Ycu * val;
	}

	public void tAdd(TransformSet addSet)
	{
		XSigma += addSet.XSigma;
		XSinMulti += addSet.XSinMulti;
		Xlin += addSet.Xlin;
		Xsq += addSet.Xsq;
		Xcu += addSet.Xcu;

		YSigma += addSet.YSigma;
		YSinMulti += addSet.YSinMulti;
		Ylin += addSet.Ylin;
		Ysq += addSet.Ysq;
		Ycu += addSet.Ycu;
	}
	
	public Vector3 GetPosition(Vector3 startPos, float objTime)
	{
		float xx;
		float yy;
		if (Xcos)
		{
			xx = (float)(startPos.x + (Math.Cos(XSigma * Math.PI * objTime) * XSinMulti) - (XSinMulti) + (Xlin * objTime) + (Xsq * objTime * objTime) + (Xcu * objTime * objTime * objTime));
		}
		else
		{
			xx = (float)(startPos.x + (Math.Sin(XSigma * Math.PI * objTime) * XSinMulti) + (Xlin * objTime) + (Xsq * objTime * objTime) + (Xcu * objTime * objTime * objTime));
		}
		
		if (Ycos)
		{
			yy = (float)(startPos.y + (Math.Cos(YSigma * Math.PI * objTime) * YSinMulti) + (Ylin * objTime) - (YSinMulti) + (Ysq * objTime * objTime) + (Ycu * objTime * objTime * objTime));
		}
		else
		{
			yy = (float)(startPos.y + (Math.Sin(YSigma * Math.PI * objTime) * YSinMulti) + (Ylin * objTime) + (Ysq * objTime * objTime) + (Ycu * objTime * objTime * objTime));
		}
		return new Vector3 (xx,yy,0);
	}
}

[Serializable]
public class TrasformStep
{
	public float timeInStep = 0.0f;
	public TransformSet transformset;
}

[Serializable]
public class TransformKey
{
	public TrasformStep[] movement;

	public Vector3 GetPosition(Vector3 startPos, float objTime)
	{
		Vector3 PosVector = startPos;
		float useTime = objTime % totalTime ();
		bool done = false;
		int ii = 0;
		while (!done)
		{
			if (useTime - movement [ii].timeInStep > 0)
			{
				PosVector += movement [ii].transformset.GetPosition (startPos, movement [ii].timeInStep);
				useTime -= movement [ii].timeInStep;
				ii++;
			}
			else
			{
				if (useTime <= 0)
				{
					done = true;
				}
				else
				{
					PosVector += movement [ii].transformset.GetPosition (startPos, useTime);
					useTime -= movement [ii].timeInStep;
					done = true;
				}
			}

		}
		return PosVector;
	}

	private float totalTime()
	{
		float rVal = 0;
		for (int ii = 0; ii < movement.Length; ii++)
		{
			rVal += movement [ii].timeInStep;
		}
		return rVal;
	}
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// 
/// 
/// 
/// 
/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



[Serializable]
public class WeightedSpawn
{
	public float Weight;
	public GameObject spawnedObject;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// 
/// 
/// 
/// 
/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



[Serializable]
public class WeightedSpawnAndTrans
{
	public float Weight;
	public GameObject spawnedObject;
	public TransformSet Trans;
}



/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// 
/// 
/// 
/// 
/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



[Serializable]
class PlayerShip
{
	public string shipname;
	public string shiptype;
	public string sheildtype;
	public string radiatortype;
	public string generatortype;
	public string mainweaponname;
	public string wingweaponname;
	
	public SerializeableColor maincolor;
	public SerializeableColor secondarycolor;
	public SerializeableColor cockpitcolor;

}



/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// 
/// 
/// 
/// 
/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



[Serializable]
public class SerializeableColor
{
	public byte Red;
	public byte Blue;
	public byte Green;
	public byte Alpha;
	
	public SerializeableColor (Color32 Storedcolor)
	{
		Red = Storedcolor.r;
		Green = Storedcolor.g;
		Blue = Storedcolor.b;
		Alpha = Storedcolor.a;
	}
	
	public Color32 GetColor()
	{
		return new Color32 (Red, Green, Blue, Alpha);
	}
	
}



/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// 
/// 
/// 
/// 
/// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



[Serializable]
public class PlayerData
{
	public int Credit;
	public int MultiWins;
	public int MultiLoses;
	public int FodderDestroyed;
	public int SingleMissionFin;
	
	public bool[] ShipHangarID = new bool[64];
	public bool[] WeapHangarID = new bool[64];
	public bool[] WingHangarID = new bool[64];
	public bool[] GenHangarID = new bool[64];
	public bool[] RadHangarID = new bool[64];
	public bool[] SheildHangarID = new bool[64];
	
}





/// Phase is a step for bossfights
/// 	-Contains stats and AI object
/// 		a part of a dreadnought fight
[Serializable]
public class Phase
{
	public float PhaseHull;
	public float PhaseSpeed;
	public Vector2 PhaseSize;
	public Dreadnought_AI PhaseAI; 
}


/// Phase is a step for bossfights
/// 	-Contains stats and AI object
/// 		a part of a dreadnought fight
[Serializable]
public class FlagPhase
{
	public float PhaseHull;
	public float PhaseSpeed;
	public Vector2 PhaseSize;
	public Flagship_AI PhaseAI; 
}






/// ShipSet acts as a container which holds all data of a single ship
/// 	-Used for entity constructors
/// 	-Cannot be saved to disk
[Serializable]
public class ShipSet
{
	public ShipType shiptype;
	public SheildType sheildtype;
	public RadiatorType radiatortype;
	public GeneratorType generatortype;
	public WeaponType mainweaponname;
	public WeaponType wingweaponname;
	public int scoreValue;
	public float shootDelay;
	public float DestroyTimer;
	public Color maincolor;
	public Color secondarycolor;
	public Color cockpitcolor;
}
	

[Serializable]
public class levelData
{
	public float LevelLength = 120;//In secs

}
	
[Serializable]
public class genData
{
	public int seed = 777;
	public int tileHeight = 4;
	public int minHeight = 0;
	public float tileScale = 5;
	public float tileMagnitude = 1;
	public float tileDepth = 1;

	private float tileSize = 4.6f;
	private int tileSeedX = 42;
	private int tileSeedY = 42;

	public float getSize()
	{
		return tileSize;
	}

	public void LoadSeed()
	{
		System.Random tileRand = new System.Random (seed);
		tileSeedX = tileRand.Next (-999999, 999999);
		tileSeedY = tileRand.Next (-999999, 999999);
		Debug.Log ("(Tile seeds) X: " + tileSeedX + " Y: " + tileSeedY);
	}

	public int getTileHeight(int x, int y)
	{
		int rVal = Noise (x + tileSeedX, y + tileSeedY, tileScale, tileMagnitude);
		if (rVal > tileHeight)
		{
			rVal = tileHeight;
		}
		if (rVal != 0 && minHeight != 0 && rVal < minHeight)
		{
			rVal = minHeight;
		}
		return ((int)rVal);
	}

	int Noise (int x, int y, float scale, float mag)
	{
		return (int)(Mathf.PerlinNoise(x/scale,y/scale)*mag); 
	}

}


[Serializable]
public class tileData
{
	public int tileHeight = 4;
	public int minHeight = 0;
	public float tileScale = 5;
	public float tileMagnitude = 1;
	public float tileDepth = 1;

	public int deconstructiveHeight = 50;
	public float deconstructiveScale = 5;
	public float deconstructivePower = 0.6f;

	public Texture[] mainTexture = new Texture[3];
	public Texture[] boarderTexture = new Texture[3];
	public Color[] colorSet = new Color[3];
	public Color[] colorSetB = new Color[3];

	private float tileSize = 4.6f;
	private int tileSeedX = 42;
	private int tileSeedY = 42;

	public float getSize()
	{
		return tileSize;
	}

	public int picDex(int xx, int yy, int seed)
	{
		System.Random picRand = new System.Random (seed);
		return ((xx * picRand.Next()) + (yy * picRand.Next())) % 10;
	}

	public void LoadSeed(int newSeed)
	{
		System.Random tileRand = new System.Random (newSeed);
		tileSeedX = tileRand.Next (-999999, 999999);
		tileSeedY = tileRand.Next (-999999, 999999);
		Debug.Log ("(Tile seeds) X: " + tileSeedX + " Y: " + tileSeedY);
	}

	public int getTileHeight(int x, int y)
	{
		int rVal = Noise (x + tileSeedX, y + tileSeedY, tileScale, tileMagnitude);
		if (rVal > tileHeight)
		{
			rVal = tileHeight;
		}
		if (rVal != 0 && minHeight != 0 && rVal < minHeight)
		{
			rVal = minHeight;
		}
		return ((int)rVal);
	}

	int Noise (int x, int y, float scale, float mag)
	{
		return (int)(Mathf.PerlinNoise(x/scale,y/scale)*mag); 
	}
}

[Serializable]
public enum PlacementKey
{
	Floor,
	Wall,
	Both
}

[Serializable]
public class Feature
{
	public PlacementKey placement;
	public GameObject varObject;
	public int sizeX;
	public int sizeY;
	public int quantity;
	public bool[] levelAvailable = new bool[3];
}

[System.Serializable]
public class ArrayLayout  {

	[System.Serializable]
	public struct rowData{
		public int[] row;
	}

	public int sizeY = 4;
	public int sizeX = 4;
	public rowData[] rows = new rowData[7]; //Grid of 7x7

	public int[,] getArray()
	{
		int[,] rVal = new int[sizeX, sizeY];
		for (int ii = 0; ii < sizeY; ii++)
		{
			for (int jj = 0; jj < sizeX; jj++)
			{
				rVal [jj, ii] = rows [ii].row [jj];
			}
		}
		return rVal;
	}
}

[System.Serializable]
public class NeuroNet
{
	public float[] inputValues;
	public NeuroNetNode[] inputLayer;
	public NeuroNetNode[] hiddenLayer;
	public NeuroNetNode[] hiddenLayer2;
	public float[] outputLayer;

	const String fileExt = ".nns";//Neuro Net Ship

	public NeuroNet copy()
	{
		NeuroNet rVal = new NeuroNet (inputLayer.Length,hiddenLayer.Length,outputLayer.Length);
		for (int ii = 0; ii < inputLayer.Length; ii++)
		{
			for (int jj = 0; jj < inputLayer [ii].layerMultiplyers.Length; jj++)
			{
				rVal.inputLayer [ii].layerMultiplyers [jj] = inputLayer [ii].layerMultiplyers [jj];
			}
		}
		for (int ii = 0; ii < hiddenLayer.Length; ii++)
		{
			for (int jj = 0; jj < hiddenLayer [ii].layerMultiplyers.Length; jj++)
			{
				rVal.hiddenLayer [ii].layerMultiplyers [jj] = hiddenLayer [ii].layerMultiplyers [jj];
			}
		}
		for (int ii = 0; ii < hiddenLayer2.Length; ii++)
		{
			for (int jj = 0; jj < hiddenLayer2 [ii].layerMultiplyers.Length; jj++)
			{
				rVal.hiddenLayer2 [ii].layerMultiplyers [jj] = hiddenLayer2 [ii].layerMultiplyers [jj];
			}
		}
		return rVal;
	}

	public void updateStep()
	{
		//clear
		foreach (NeuroNetNode node in hiddenLayer2)
		{
			node.input = 0f;
		}
		foreach (NeuroNetNode node in hiddenLayer)
		{
			node.input = 0f;
		}
		for (int ii = 0; ii < outputLayer.Length; ii++)
		{
			outputLayer[ii] = 0f;
		}
		//update
		for (int ii = 0; ii < inputValues.Length; ii++)
		{
			inputLayer [ii].input = inputValues [ii];
		}
		for(int ii = 0; ii < inputLayer.Length; ii++)
		{
			float[] layerOutput = inputLayer[ii].getOut ();
			for (int jj = 0; jj < layerOutput.Length; jj++)
			{
				hiddenLayer2 [jj].input += layerOutput [jj];
			}
		}
		for(int ii = 0; ii < hiddenLayer2.Length; ii++)
		{
			float[] layerOutput = hiddenLayer2[ii].getOut ();
			for (int jj = 0; jj < layerOutput.Length; jj++)
			{
				hiddenLayer [jj].input += layerOutput [jj];
			}
		}
		for(int ii = 0; ii < hiddenLayer.Length; ii++)
		{
			float[] layerOutput = hiddenLayer[ii].getOut ();
			for (int jj = 0; jj < layerOutput.Length; jj++)
			{
				outputLayer[jj] += layerOutput [jj];
			}
		}
	}

	public NeuroNet(int inputs, int hiddens, int outputs)
	{
		inputValues = new float[inputs];
		inputLayer = new NeuroNetNode[inputs];
		hiddenLayer = new NeuroNetNode[hiddens];
		hiddenLayer2 = new NeuroNetNode[hiddens];
		outputLayer = new float[outputs];
		for (int ii = 0; ii < hiddens; ii++)
		{
			hiddenLayer2 [ii] = new NeuroNetNode (hiddens);
		}
		for (int ii = 0; ii < hiddens; ii++)
		{
			hiddenLayer [ii] = new NeuroNetNode (outputs);
		}
		for (int ii = 0; ii < inputs; ii++)
		{
			inputLayer [ii] = new NeuroNetNode (hiddens);
		}
	}
	public void saveBrain(String fileName)
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream SavFile = File.Open(Application.persistentDataPath + "/" + "neuros" + "/" + fileName + fileExt , FileMode.OpenOrCreate);
		NeuroNet SaveData = new NeuroNet(inputLayer.Length,hiddenLayer.Length,outputLayer.Length);
		SaveData.inputValues = inputValues;
		SaveData.inputLayer = inputLayer;
		SaveData.hiddenLayer = hiddenLayer;
		SaveData.hiddenLayer2 = hiddenLayer2;
		SaveData.outputLayer = outputLayer;
		bf.Serialize (SavFile, SaveData);
		SavFile.Close();
		//Debug.Log ("Save Complete");
	}
	public void loadBrain(String fileName)
	{
		if (File.Exists (Application.persistentDataPath + "/" + "neuros" + "/" + fileName + fileExt)) 
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/" + "neuros" + "/" + fileName + fileExt , FileMode.Open);
			NeuroNet LoadShip = (NeuroNet)bf.Deserialize(file);
			inputValues = LoadShip.inputValues;
			inputLayer = LoadShip.inputLayer;
			hiddenLayer = LoadShip.hiddenLayer;
			hiddenLayer2 = LoadShip.hiddenLayer2;
			outputLayer = LoadShip.outputLayer;
			file.Close();
			//Debug.Log ("Load Complete");
		}
	}
}

[System.Serializable]
public class NeuroNetNode
{
	public float input = 0f;
	public float[] layerMultiplyers;
	public float[] getOut ()
	{
		if (input < 0f)
		{
			input = 0f;
		}
		if (input > 2f)
		{
			input = 2f;
		}
		float val = Mathf.Atan(Mathf.PI * (input));
		float[] rVal = new float[layerMultiplyers.Length];
		for (int ii = 0; ii < rVal.Length; ii++)
		{
			rVal [ii] = layerMultiplyers [ii] * val;
		}
		return rVal;
	}
	public NeuroNetNode(int nextLayerSize)
	{
		layerMultiplyers = new float[nextLayerSize];
		for (int ii = 0; ii < layerMultiplyers.Length; ii++)
		{
			layerMultiplyers [ii] = 0.5f;
		}
	}
}

[System.Serializable]
public class NetState
{
	public StateIn inputs;
	public StateOut outputs;
	const String fileExt = ".smp";
	public NetState()
	{
		inputs = new StateIn ();
		outputs = new StateOut ();
	}
	public void saveState(String fileName)
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream SavFile = File.Open(Application.persistentDataPath + "/" + "samples" + "/" + fileName + fileExt , FileMode.OpenOrCreate);
		NetState SaveData = new NetState();
		SaveData.inputs = inputs;
		SaveData.outputs = outputs;
		bf.Serialize (SavFile, SaveData);
		SavFile.Close();
		Debug.Log ("State: " + fileName + fileExt + " Saved!");
	}
	public static NetState loadState(String fileName)
	{
		NetState rVal = new NetState ();
		if (File.Exists (Application.persistentDataPath + "/" + "samples" + "/" + fileName + fileExt)) 
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/" + "samples" + "/" + fileName + fileExt, FileMode.Open);
			NetState loadState = (NetState)bf.Deserialize(file);
			rVal.inputs = loadState.inputs;
			rVal.outputs = loadState.outputs;
			file.Close();
		}
		return rVal;
	}
	[System.Serializable]
	public class StateIn
	{
		public float healthRatio;
		public float sheildRatio;
		public float tempRatio;
		public float powerRatio;

		public float xRatio;
		public float yRatio;

		public float xERatio;
		public float yERatio;

		public float xSRatio;
		public float ySRatio;

		public StateIn()
		{
			healthRatio = 0f;
			sheildRatio = 0f;
			tempRatio = 0f;
			powerRatio = 0f;

			xRatio = 0f;
			yRatio = 0f;

			xERatio = 0f;
			yERatio = 0f;

			xSRatio = 0f;
			ySRatio = 0f;
		}
	}

	[System.Serializable]
	public class StateOut
	{
		public float xTargRatio;
		public float yTargRatio;
		public bool main;
		public bool wing;

		public StateOut ()
		{
			xTargRatio = 0f;
			yTargRatio = 0f;
			main = false;
			wing = false;
		}
	}
}