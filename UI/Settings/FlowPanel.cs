using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Assets.Scripts.Manager;
using Assets.Scripts.SettingClasses;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FlowPanel : MonoBehaviour, IPanel
{
    public Setting S;
    public FlowSetting Setting;
    public List<Slider> Sliders = new List<Slider>();
    public Dropdown Mode;
    public void Start()
    {
        S = EnvironmentManager.Instance.Settings;
        Sliders = GetComponentsInChildren<Slider>().ToList();
        Sliders.ForEach(n => n.gameObject.AddComponent<ShowTooltip>());
        Mode.value = 0;
        Mode.captionText.text = ((Difficulty) 0).ToString();
        Mode.RefreshShownValue();   
    }

    public void Update()
    {
        
    }

    public void SetMode(int val)
    {
        Setting = EnvironmentManager.Instance.Settings.FlowSettings.Find(n => (int) n.Mode == val);
        foreach (var slider in Sliders)
        {
            switch (slider.name)
            {
                case "Multiplier": slider.value = Setting.FlowMultiplier; break;
                case "Time": slider.value = Setting.MaxMissionTime; break;
                case "Enemies": slider.value = Setting.MaximumEnemies; break;
                case "Particles": slider.value = Setting.SoakingEffectivity; break;
                case "Consumption": slider.value = Setting.ManaConsumption; break;
                case "Amount": slider.value = Setting.SpawnAmount; break;
                case "Radius": slider.value = Setting.SpawnRadius; break;
                case "HP": slider.value = Setting.HealthIncrease; break;
                case "MP": slider.value = Setting.ManaIncrease; break;
            }
        }
        Mode.captionText.text = ((Difficulty) val).ToString();
        Mode.RefreshShownValue();
    }

    public void Save()
    {
        var f = S.FlowSettings.Find(n => n.Mode == Setting.Mode);
        f = Setting;
        EnvironmentManager.Instance.Settings = S;
    }

    // Slider Settings

    public void SetFlowMultiplier(float val)
    {
        Setting.FlowMultiplier = val;
    }

    public void SetMissionTime(float val)
    {
        Setting.MaxMissionTime = (int) val;
    }

    public void SetMaxEnemies(float val)
    {
        Setting.MaximumEnemies = (int)val;
    }

    public void SetParticleEffect(float val)
    {
        Setting.SoakingEffectivity = val;
    }

    public void SetParticleConsumption(float val)
    {
        Setting.ManaConsumption = val;
    }

    public void SetObstacleAmount(float val)
    {
        Setting.SpawnAmount = (int) val;
    }

    public void SetSpawnAmount(float val)
    {
        Setting.SpawnRadius = (int) val;
    }

    public void HPIncrease(float val)
    {
        Setting.HealthIncrease = val;
    }

    public void MPIncrease(float val)
    {
        Setting.ManaIncrease = val;
    }


    public void SetActive(bool active)
    {
        GameObject.FindObjectOfType<Assets.Scripts.UI.Settings.SettingsManager>().SetActive("FlowPanel");
    }
}
