using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region : MonoBehaviour {

	public string regionName;

	void OnTriggerEnter(Collider other)
	{
		HUD.Instance.EnterRegion (regionName);
	}
}
