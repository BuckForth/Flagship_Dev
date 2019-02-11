using UnityEngine;
using System.Collections;

public class SheildRun : MonoBehaviour 
{
	float SheildTime = 0;

	// Update is called once per frame
	void Update () 
	{
		if (SheildTime > 0)
		{
			SheildTime -= Time.deltaTime;
			int SheildFrame = 8 - Mathf.RoundToInt(SheildTime / 0.04f);
			if (SheildFrame > 8)
			{
				SheildFrame = 8;
			}else if (SheildFrame < 0)
			{
				SheildFrame = 0;
			}
			if (this.GetComponentInParent<ShipController>() != null)
			{
				this.GetComponent<SpriteRenderer>().sprite = this.GetComponentInParent<ShipController>().Sheildtype.SpriteArray[SheildFrame];
			}
			else if (this.GetComponentInParent<FlagshipController>() != null)
			{
				this.GetComponent<SpriteRenderer>().sprite = this.GetComponentInParent<FlagshipController>().Sheildtype.SpriteArray[SheildFrame];
			}
			else if (this.GetComponentInParent<FodderController>() != null)
			{
				this.GetComponent<SpriteRenderer>().sprite = this.GetComponentInParent<FodderController>().Sheildtype.SpriteArray[SheildFrame];
			}
		}
		else
		{
			SheildTime = 0;
		}

	}

	public void ActiveSheild ()
	{
		SheildTime = 0.32f;
	}
}
