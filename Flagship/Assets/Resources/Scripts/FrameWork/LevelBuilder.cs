using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class LevelBuilder : MonoBehaviour
{
	public int levelSeed = 777;
	private int tileQuality = 10;
	private float Yslight = 0.0025f;
	private float Xslight = 0.004f;
	public int enemyStart = 0;
	public int enemyEnd = 0;
	public tileData tileInfo;
	public Feature[] featureList;
	public GameObject[] shipTypes;
	public Formation[] shipFormations;
	public int shipGapMin = 1;
	public int shipGapMax = 5;
	public int levelLength = 100;

	public Material tileMat;

	private int[,] tileHeights;
	private bool[,] featureMap;
	private System.Random levelRand;

	private List<Vector3> newVertices = new List<Vector3>();
	private List<int> newTriangles = new List<int>();
	private List<Vector2> newUV = new List<Vector2>();
	private Mesh mesh;
	private float xUnit = (1f/6f);
	private float yUnit = (1f/3f);

	private int squareCount;

	// Use this for initialization
	void Start ()
	{
		levelRand = new System.Random (levelSeed);
		tileInfo.LoadSeed (levelRand.Next());
		BuildTiles ();
	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown("s"))
		{
			SaveAsset();
		}
	}

	void BuildTiles()
	{
		tilePassOne ();
		for (int ii = 0; ii < tileQuality; ii++)
		{
			tilePassTwo ();
		}
		buildObject ();
		setFeatures ();
	}

	//Loads the seed and sets heights
	void tilePassOne()
	{
		int tileWidth = 12;
		int tileLength = levelLength;

		tileHeights = new int[tileWidth, tileLength];

		for (int xx = 0; xx < tileWidth; xx++)
		{
			for (int yy = 0; yy < tileLength; yy++)
			{
				tileHeights [xx, yy] = tileInfo.getTileHeight(xx,yy);
			}
		}
	}

	//Cleans up any odds and ends
	void tilePassTwo()
	{
		for (int layer = 3; layer >= 0; layer--)
		{
			for (int xx = 0; xx < tileHeights.GetLength (0); xx++)
			{
				for (int yy = 0; yy < tileHeights.GetLength (1); yy++)
				{
					if (tileHeights [xx, yy] == layer + 1)
					{
						if (!keepAble(layer , xx, yy))
						{
							tileHeights [xx, yy] = tileHeights [xx, yy] - 1;
						}
					}
				}
			}
		}
	}

	bool  hasTile(int layer, int xx, int yy)
	{
		bool rVal = false;
		if (tileInfo.getTileHeight(xx,yy) > layer)
		{
			if (xx >= 0 && yy >= 0 && xx < tileHeights.GetLength(0) && yy < tileHeights.GetLength(1) )
			{
				if (tileHeights[xx, yy] > layer)
				{
					rVal = true;
				}
			}
			else
			{
				rVal = true;
			}
		}
		return rVal;
	}

	int surroundCount (int layer, int xx, int yy)
	{
		int rVal = neighbourCount (layer, xx, yy);
		if (hasTile (layer, xx - 1, yy + 1))
		{
			rVal++;
		}
		if (hasTile (layer, xx + 1, yy + 1))
		{
			rVal++;
		}
		if (hasTile (layer, xx - 1, yy - 1))
		{
			rVal++;
		}
		if (hasTile (layer, xx + 1, yy - 1))
		{
			rVal++;
		}
		return rVal;
	}

	int neighbourCount (int layer, int xx, int yy)
	{
		int rVal = 0;
		if (hasTile (layer, xx - 1, yy))
		{
			rVal++;
		}
		if (hasTile (layer, xx + 1, yy))
		{
			rVal++;
		}
		if (hasTile (layer, xx, yy - 1))
		{
			rVal++;
		}
		if (hasTile (layer, xx, yy + 1))
		{
			rVal++;
		}
		return rVal;
	}

	bool keepAble (int layer, int xx, int yy)
	{
		bool keep = false;
		if (neighbourCount(layer, xx, yy) > 1)
		{
			if (hasTile (layer, xx - 1, yy) && hasTile (layer, xx, yy + 1))
			{
				keep = true;
			}
			else if (hasTile (layer, xx, yy + 1) && hasTile (layer, xx + 1, yy))
			{
				keep = true;
			}
			else if (hasTile (layer, xx + 1, yy) && hasTile (layer, xx, yy - 1))
			{
				keep = true;
			}
			else if (hasTile (layer, xx, yy - 1) && hasTile (layer, xx - 1, yy))
			{
				keep = true;
			}

			if (!hasTile (layer, xx - 1, yy - 1) && !hasTile (layer, xx + 1, yy + 1))
			{
				keep = false;
			}
			else if(!hasTile (layer, xx + 1, yy - 1) && !hasTile (layer, xx - 1, yy + 1))
			{
				keep = false;
			}
		}
		return keep;
	}

	void buildObject ()
	{
		GameObject tileObj = new GameObject ("Tile");
		tileObj.AddComponent<TileController> ();
		tileObj.GetComponent<TileController> ().Player = GameObject.Find ("Ship_Player");
		for (int layer = 0; layer < 3; layer++)
		{
			Material buildMat = new Material (tileMat);
			buildMat.mainTexture = tileInfo.mainTexture [layer];
			buildMat.color = tileInfo.colorSet [layer];

			GameObject tilesLow = new GameObject ("TileLayer_" + layer);
			tilesLow.AddComponent<MeshRenderer> ();
			tilesLow.AddComponent<MeshFilter> ();
			mesh = tilesLow.GetComponent<MeshFilter> ().mesh;
			tilesLow.GetComponent<MeshRenderer> ().material = buildMat;
			///////////
			Material buildMatB = new Material (tileMat);
			buildMatB.mainTexture = tileInfo.boarderTexture [layer];
			buildMatB.color = tileInfo.colorSetB [layer];

			GameObject tileB = new GameObject ("Border_" + layer);
			tileB.AddComponent<MeshRenderer> ();
			tileB.AddComponent<MeshFilter> ();
			tileB.GetComponent<MeshFilter> ().mesh = mesh;
			tileB.GetComponent<MeshRenderer> ().material = buildMatB;
			tileB.transform.SetParent(tilesLow.transform);
				
			for (int xx = 0; xx < tileHeights.GetLength (0); xx++)
			{
				for (int yy = 0; yy < tileHeights.GetLength (1); yy++)
				{
					if (tileHeights [xx, yy] > layer)
					{
						if (hasTile (layer, xx, yy - 1 ) && hasTile (layer, xx, yy + 1) && hasTile (layer, xx - 1, yy) && hasTile (layer, xx + 1, yy))
						{
							//Do inner corner checks here!!!!!!!!!!!!!!!!!!!!!!!
							if (!hasTile (layer, xx - 1, yy - 1))
							{
								GenSquare(xx,yy, new Vector2(4,2)); //Try for inner corner1
							}
							else if (!hasTile (layer, xx + 1, yy - 1))
							{
								GenSquare(xx,yy, new Vector2(3,2)); //Try for inner corner1
							}
							else if (!hasTile (layer, xx + 1, yy + 1))
							{
								GenSquare(xx,yy, new Vector2(3,1)); //Try for inner corner1
							}
							else if (!hasTile (layer, xx - 1, yy + 1))
							{
								GenSquare(xx,yy, new Vector2(4,1)); //Try for inner corner1
							}
							else
							{
								int dex = tileInfo.picDex(xx, yy, levelSeed);
								if (dex == 0)
								{
									GenSquare(xx,yy, new Vector2(3,0)); //Try for mid
								}
								else if(dex == 1)
								{
									GenSquare(xx,yy, new Vector2(4,0)); //Try for mid
								}
								else if(dex == 2)
								{
									GenSquare(xx,yy, new Vector2(5,0)); //Try for mid
								}
								else if(dex == 3)
								{
									GenSquare(xx,yy, new Vector2(5,1)); //Try for mid
								}
								else if(dex == 4)
								{
									GenSquare(xx,yy, new Vector2(5,2)); //Try for mid
								}
								else
								{
									GenSquare(xx,yy, new Vector2(1,1)); //Try for mid
								}
							}
						}
						else if (hasTile (layer, xx, yy - 1) && hasTile (layer, xx, yy + 1) && hasTile (layer, xx - 1, yy))
						{
							if (!hasTile (layer, xx - 1, yy - 1))
							{
								GenSquare(xx,yy, new Vector2(2,0)); // corner instead
							}
							else if (!hasTile (layer, xx - 1, yy + 1))
							{
								GenSquare(xx,yy, new Vector2(2,2)); // corner instead
							}
							else
							{
								GenSquare(xx,yy, new Vector2(2,1)); //Try for vert side |=>
							}
						}	

						else if (hasTile (layer, xx, yy - 1) && hasTile (layer, xx, yy + 1) && hasTile (layer, xx + 1, yy))
						{
							if (!hasTile (layer, xx + 1, yy - 1))
							{
								GenSquare(xx,yy, new Vector2(0,0)); // corner instead
							}
							else if (!hasTile (layer, xx + 1, yy + 1))
							{
								GenSquare(xx,yy, new Vector2(0,2)); // corner instead
							}
							else
							{
								GenSquare(xx,yy, new Vector2(0,1)); //Try for vert side
							}
						}
						else if (hasTile (layer, xx, yy + 1) && hasTile (layer, xx - 1, yy) && hasTile (layer, xx + 1, yy))
						{
							if (!hasTile (layer, xx - 1, yy + 1))
							{
								GenSquare(xx,yy, new Vector2(2,0)); // corner instead
							}
							else if (!hasTile (layer, xx + 1, yy + 1))
							{
								GenSquare(xx,yy, new Vector2(0,0)); // corner instead
							}
							else
							{
								GenSquare(xx,yy, new Vector2(1,0)); //Try for Horz side v_v
							}
						}
						else if (hasTile (layer, xx, yy - 1) && hasTile (layer, xx - 1, yy) && hasTile (layer, xx + 1, yy))
						{
							if (!hasTile (layer, xx - 1, yy - 1))
							{
								GenSquare(xx,yy, new Vector2(2,2)); // corner instead
							}
							else if (!hasTile (layer, xx + 1, yy - 1))
							{
								GenSquare(xx,yy, new Vector2(0,2)); // corner instead
							}
							else
							{
								GenSquare(xx,yy, new Vector2(1,2)); //Try for Horz side
							}
						}

						else if (hasTile (layer, xx - 1, yy) && hasTile (layer, xx, yy + 1) && hasTile (layer, xx - 1, yy + 1))
						{
							GenSquare(xx,yy, new Vector2(2,0));// corner1
						}
						else if (hasTile (layer, xx, yy + 1) && hasTile (layer, xx + 1, yy) && hasTile (layer, xx + 1, yy + 1))
						{
							GenSquare(xx,yy, new Vector2(0,0));// corner2
						}
						else if (hasTile (layer, xx + 1, yy) && hasTile (layer, xx, yy - 1) && hasTile (layer, xx + 1, yy - 1))
						{
							GenSquare(xx,yy, new Vector2(0,2));// corner3
						}
						else if (hasTile (layer, xx, yy - 1) && hasTile (layer, xx - 1, yy) && hasTile (layer, xx - 1, yy - 1))
						{
							GenSquare(xx,yy, new Vector2(2,2));// corner4
						}

					}
				}
			}
			UpdateMesh();
			tilesLow.transform.position = new Vector3 (tilesLow.transform.position.x, tilesLow.transform.position.y, tilesLow.transform.position.z - (layer * .125f));
			tilesLow.transform.SetParent (tileObj.transform);
			tileB.transform.localPosition = new Vector3 (tileB.transform.localPosition.x, tileB.transform.localPosition.y, tileB.transform.localPosition.z - (layer * .125f) - 0.025f);
		}
	}

	void UpdateMesh () 
	{
		mesh.Clear ();
		mesh.vertices = newVertices.ToArray();
		mesh.triangles = newTriangles.ToArray();
		mesh.uv = newUV.ToArray();
		;
		mesh.RecalculateNormals ();

		squareCount=0;
		newVertices.Clear();
		newTriangles.Clear();
		newUV.Clear();

	}

	void GenSquare(int x, int y, Vector2 texture)
	{
		newVertices.Add( new Vector3 ((x * tileInfo.getSize()) - (tileHeights.GetLength (0) * tileInfo.getSize())/2 -3.4f,( y * tileInfo.getSize()) -15f , 0 ));
		newVertices.Add( new Vector3 ((x * tileInfo.getSize() + tileInfo.getSize()) - (tileHeights.GetLength (0) * tileInfo.getSize())/2 -3.4f, (y * tileInfo.getSize()) -15f , 0 ));
		newVertices.Add( new Vector3 ((x * tileInfo.getSize() + tileInfo.getSize()) - (tileHeights.GetLength (0) * tileInfo.getSize())/2 -3.4f, (y * tileInfo.getSize() - tileInfo.getSize()) -15f , 0 ));
		newVertices.Add( new Vector3 ((x * tileInfo.getSize())  - (tileHeights.GetLength (0) * tileInfo.getSize())/2 -3.4f,( y * tileInfo.getSize() - tileInfo.getSize()) -15f, 0 ));

		newTriangles.Add(squareCount*4);
		newTriangles.Add((squareCount*4)+1);
		newTriangles.Add((squareCount*4)+3);
		newTriangles.Add((squareCount*4)+1);
		newTriangles.Add((squareCount*4)+2);
		newTriangles.Add((squareCount*4)+3);

		newUV.Add(new Vector2 (xUnit * texture.x + Xslight, yUnit * texture.y + yUnit - Yslight));
		newUV.Add(new Vector2 (xUnit * texture.x + xUnit - Xslight, yUnit * texture.y + yUnit - Yslight));
		newUV.Add(new Vector2 (xUnit * texture.x + xUnit - Xslight, yUnit * texture.y + Yslight));
		newUV.Add(new Vector2 (xUnit * texture.x + Xslight, yUnit * texture.y + Yslight));

		squareCount++;
	}


	void setFeatures ()
	{
		System.Random featRand = new System.Random (levelSeed);
		featureMap = new bool[tileHeights.GetLength (0),tileHeights.GetLength (1)];

		for (int ii = 0; ii < featureList.Length; ii++)
		{
			bool canPlace = true;
			int amount = featureList [ii].quantity;
			while (canPlace && amount > 0) 
			{
				canPlace = setObject(featureList [ii], featRand);
				amount--;
			}
		}
	}

	bool setObject(Feature feature, System.Random rand)
	{
		bool rVal = false;
		int[,] availableHeight = new int[tileHeights.GetLength (0),tileHeights.GetLength (1)];
		for (int ii = 0; ii < tileHeights.GetLength (0); ii++)
		{
			for (int jj = 0; jj < tileHeights.GetLength (1); jj++)
			{
				if (tileHeights [ii, jj] != 0 && feature.levelAvailable [tileHeights [ii, jj] - 1])
				{
					
					availableHeight [ii, jj] = tileHeights [ii, jj];
				}
				else
				{
					availableHeight [ii, jj] = 0;
				}
			}
		}
		Vector2[] allSpots = new Vector2[0];
		for (int xx = 0; xx < tileHeights.GetLength (0); xx++)
		{
			for (int yy = 0; yy < tileHeights.GetLength (1); yy++)
			{
				if (tileHeights [xx, yy] > 0 && surroundCount (tileHeights [xx, yy] - 1, xx, yy) == 8 && feature.levelAvailable[tileHeights [xx, yy] - 1])
				{
					bool open = true;

					for (int ii = 0; ii < feature.sizeX; ii++)
					{
						for (int jj = 0; jj < feature.sizeY; jj++)
						{
							if (surroundCount (tileHeights [xx, yy] - 1, xx+ii, yy+jj) != 8)
							{
								open = false;
							}
							if (!(ii + xx >= tileHeights.GetLength(0) || jj + yy >= tileHeights.GetLength(1)) && tileHeights [xx, yy] < tileHeights [ii + xx, jj + xx])
							{
								open = false;
							}
							if (!(ii + xx >= tileHeights.GetLength(0) || jj + yy >= tileHeights.GetLength(1)) &&  featureMap [xx + ii, yy + jj]) 
							{
								open = false;
							}
						}
					}
					if (xx == 0 || xx == tileHeights.GetLength (0)-1 || yy < enemyStart || yy > tileHeights.GetLength (1) - enemyEnd)
					{
						open = false;
					}
					if (open)
					{
						Vector2 newXY = new Vector2 ((float)xx, (float)yy);
						Vector2[] updateList = new Vector2[allSpots.Length + 1];
						for (int kk = 0; kk < allSpots.Length; kk++)
						{
							updateList[kk] = allSpots [kk];
						}
						updateList[allSpots.Length] = newXY;
						allSpots = updateList;
					}
				}
			}
		}
		Debug.Log (allSpots.Length);
		if (allSpots.Length > 0)
		{
			GameObject targetLayer = GameObject.Find("Tile");
			GameObject newFeat = Instantiate (feature.varObject);
			newFeat.name = feature.varObject.name;
			newFeat.transform.SetParent (targetLayer.transform);
			Vector2 spot = allSpots [rand.Next() % allSpots.Length];
			float tileSize = tileInfo.getSize ();
			Vector3 location = new Vector3 ((spot.x * tileSize) - (tileHeights.GetLength (0) * tileSize /2f) - (tileSize/4f), (spot.y * tileSize) - (4 * tileSize) + (tileSize/4f));
			newFeat.transform.localPosition = location;
			rVal = true;
			//Set featureMap.
			for (int ii = 0; ii < feature.sizeX; ii++)
			{
				for (int jj = 0; jj < feature.sizeY; jj++)
				{
					featureMap[(int)spot.x + ii, (int)spot.y + jj] = true;
				}
			}
		}

		return rVal;
	}
		

	void SaveAsset()
	{
		//Mesh m1 = obj1.GetComponent<MeshFilter>().mesh;
		//AssetDatabase.CreateAsset(m1, "Assets/GenTiles/Tile" + levelSeed + "_" + tileInfo.tileScale + "_" + tileInfo.tileMagnitude  + ".asset"); // saves to "assets/"
		//AssetDatabase.SaveAssets(); // not needed?
	}

}

