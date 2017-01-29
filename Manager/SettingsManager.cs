using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Globalization;
using System.Linq;
using Assets.Scripts.Ant;
using Assets.Scripts.Manager;
using Assets.Scripts.SettingClasses;

public class SettingsManager : MonoBehaviour
{
    public Setting GeneralSettings = new Setting();
	
	//Stores the panel due to inactive game objects can't be found
	public GameObject settings;

	//Stores all the input fields
	public InputField spawnRadius;
	public InputField capacity;
	public Toggle prioOn;
	public InputField maxRank;
	public InputField refreshRate;
	public InputField walkRadius;
	public InputField searchRadius;
	public InputField jobSpawnRadius;
	public InputField minParts;
	public InputField maxParts;
	public InputField graphUpdate;
	public Text helpText;

    public AntType CurrentAntType;
	
	public void Start ()
	{
        InitDefaultSettings();
        GetGeneralSettings();
        InitUISettings();
		Seed();
	}

    public void InitDefaultSettings()
    {
        Setting set = EnvironmentManager.Instance.Settings;
        if (set == null) return;
        GeneralSettings = set;
    }

    public void GetGeneralSettings()
    {
        
    }
	
	public void reset ()
	{
		Seed();
	}


    public void InitUISettings()
    {
        Debug.Log("Loading Settings...");

        // General
        //capacity.text = GeneralSettings.Capacity.ToString();
        //prioOn.isOn = GeneralSettings.NavmeshPriorityCollision;
        //maxRank.text = GeneralSettings.MaximumRank.ToString();
        //refreshRate.text = GeneralSettings.RefreshRate.ToString(CultureInfo.InvariantCulture);
        //walkRadius.text = GeneralSettings.WalkRadius.ToString(CultureInfo.InvariantCulture);
        //searchRadius.text = GeneralSettings.SearchRadius.ToString(CultureInfo.InvariantCulture);
        //jobSpawnRadius.text = GeneralSettings.JobSetting.JobSpawnRadius.ToString(CultureInfo.InvariantCulture);
        //minParts.text = GeneralSettings.JobSetting.MinimumJobParts.ToString();
        //maxParts.text = GeneralSettings.JobSetting.MaximumJobParts.ToString();
        //graphUpdate.text = GeneralSettings.GraphUpdateInterval.ToString();
        
        Debug.Log("Finished Loading!");
    }

	
	public void Seed()
	{
	    //spawnRadius.text = GeneralSettings.SpawnRadius.ToString("F");
	    //capacity.text = GeneralSettings.Capacity.ToString();
	    //maxRank.text = GeneralSettings.MaximumRank.ToString();
	    //refreshRate.text = GeneralSettings.RefreshRate.ToString("F");
	    //walkRadius.text = GeneralSettings.WalkRadius.ToString("F");
     //   searchRadius.text = GeneralSettings.SearchRadius.ToString("F");
     //   jobSpawnRadius.text = GeneralSettings.JobSetting.JobSpawnRadius.ToString("F");
     //   minParts.text = GeneralSettings.JobSetting.MinimumJobParts.ToString();
     //   maxParts.text = GeneralSettings.JobSetting.MaximumJobParts.ToString();
	    //graphUpdate.text = GeneralSettings.GraphUpdateInterval.ToString();
		Debug.Log ("Finished Loading");
	}
    
    
    // Update Settings Model
    private void UpdateGeneralSettings()
    {
        GeneralSettings.Capacity = int.Parse(capacity.text);
        GeneralSettings.NavmeshPriorityCollision = prioOn.isOn;
        GeneralSettings.MaximumRank = int.Parse(maxRank.text);
        GeneralSettings.RefreshRate = float.Parse(refreshRate.text);
        GeneralSettings.WalkRadius = float.Parse(walkRadius.text);
        GeneralSettings.SearchRadius = float.Parse(searchRadius.text);
        GeneralSettings.JobSetting.JobSpawnRadius = float.Parse(jobSpawnRadius.text);
        GeneralSettings.JobSetting.MinimumJobParts = int.Parse(minParts.text);
        GeneralSettings.JobSetting.MaximumJobParts = int.Parse(maxParts.text);
        GeneralSettings.GraphUpdateInterval = int.Parse(graphUpdate.text);
    }
}
