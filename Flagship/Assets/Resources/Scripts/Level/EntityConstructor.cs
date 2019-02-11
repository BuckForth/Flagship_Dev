using UnityEngine;
using System.Collections;

public class EntityConstructor : MonoBehaviour 
{
	private GameObject Level_ControllerOBJ;
	public GameObject objectBase;
	public bool isFlagship = false;
	public float TriggerTime = 0.0f;
	public bool PauseWhileLiving = false;
 	
	public bool Pausebackground = false;
	public ShipSet shipInfo;
	public TransformKey movement;

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
			GameObject Entity_Spawned;
			if (isFlagship)
			{

			}
			else
			{
				GameObject ship = GameObject.Instantiate (objectBase);
				FodderController shipcont = ship.GetComponent<FodderController> ();
				shipcont.Shiptype = shipInfo.shiptype;
				shipcont.Sheildtype = shipInfo.sheildtype;
				shipcont.Radiatortype = shipInfo.radiatortype;
				shipcont.Generatortype = shipInfo.generatortype;
				if (shipInfo.mainweaponname != null)
				{
					shipcont.MainWeapon = shipInfo.mainweaponname;
				}
				if (shipInfo.wingweaponname != null)
				{
					shipcont.WingWeapon = shipInfo.wingweaponname;
				}
				shipcont.ScoreValue = shipInfo.scoreValue;
				shipcont.MainColor = shipInfo.maincolor;
				shipcont.SecondaryColor = shipInfo.secondarycolor;
				shipcont.CockpitColor = shipInfo.cockpitcolor;
				shipcont.LifeSpan = shipInfo.DestroyTimer;
				shipcont.ShootDelay = shipInfo.shootDelay;
				shipcont.MovementKey = movement;
				Entity_Spawned = ship;
				Entity_Spawned.transform.position = this.transform.position;
				HugEntity = Entity_Spawned;
			}
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
