using UnityEngine;
using System.Collections;

public class EntitySpawner : MonoBehaviour 
{
	private GameObject Level_ControllerOBJ;
	public GameObject SpawnedEntity;
	public TransformSet movementKey;
	public float TriggerTime = 0.0f;
	public bool PauseWhileLiving = false;

	public bool Pausebackground = false;

	[HideInInspector]
	public bool isActiveNow = false;
	[HideInInspector]
	public GameObject HugEntity;
	// Use this for initialization
	void Start () 
	{
		Level_ControllerOBJ = GameObject.Find ("Level_Controller");	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (TriggerTime < Level_ControllerOBJ.GetComponent<Level_Controller>().LevelTime && !isActiveNow)
		{
			GameObject Entity_Spawned = (GameObject)Instantiate(SpawnedEntity);
			Entity_Spawned.transform.position = this.transform.position;
			Entity_Spawned.name = SpawnedEntity.name;
			Entity_Spawned.transform.SetParent (this.transform.parent);
			HugEntity = Entity_Spawned;
			if (PauseWhileLiving)
			{
				Level_ControllerOBJ.GetComponent<Level_Controller>().TimeAdding = false;
				if (Pausebackground)
				{
					GameObject.Find("BackGround/Ground_Layer").GetComponent<TimeBaseTrack>().MovingPicture = false;
				}
			}
			else
			{
				Destroy(this.gameObject);
			}
			isActiveNow = true;
		}
		if (isActiveNow && PauseWhileLiving && HugEntity == null)
		{
			Level_ControllerOBJ.GetComponent<Level_Controller>().TimeAdding = true;
			if (Pausebackground)
			{
				GameObject.Find("BackGround/Ground_Layer").GetComponent<TimeBaseTrack>().MovingPicture = true;
			}
			Destroy(this.gameObject);
		}
	}
}
