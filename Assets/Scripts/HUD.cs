using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour {
	public static HUD Instance;
	void Awake()
	{Instance = this;}

	public Text regionLabel;

	string currentRegionName;

	// Use this for initialization
	void Start () {
		
	}
	
	public void EnterRegion(string regionName)
	{
		currentRegionName = regionName;
		regionLabel.text = currentRegionName;
	}
}
