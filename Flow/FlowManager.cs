using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Ant.AgentTypes;
using UnityEngine;
using Assets.Scripts.Manager;
using Assets.Scripts.Player;
using Assets.Scripts.SettingClasses;
using FuzzyLogic;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FlowManager : MonoBehaviour
{
    [Range(0, 1)]
    public float FlowLevel = 0.1f;
    public FlowSetting Flow;
    public GameObject ObstacleGameObject;
    public GameManager GameManager;

    public int MissionsFulfilled;
    public int MissionsTotal;

    public int TotalThrows;
    public int TotalHits;

    public float SoakingParticlesHit;
    public float SoakingParticlesEmitted;


    [Range(0, 1)]
    public float MissionEfficiency;
    [Range(0, 1)]
    public float ThrowAccuracy;
    [Range(0, 1)]
    public float SoakingAccuracy;

    private float NavAgentSpeed;
    public Text FlowMultiplier;

    // Use this for initialization
    void Start ()
	{
	    Flow = EnvironmentManager.Instance.Settings.Flow();
	    GameManager = EnvironmentManager.Instance.GameManager;
        InvokeRepeating("UpdateFlow", 10, 10);
	    NavAgentSpeed = 30f;
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (!GameManager.Player) return;
	    UpdateStats();
    }

    void UpdateStats()
    {
        if (TotalThrows > 0)
        {
            ThrowAccuracy = (float)TotalHits / TotalThrows;
        }
        if (SoakingParticlesHit > 0)
        {
            SoakingAccuracy = SoakingParticlesHit / SoakingParticlesEmitted;
        }
        if (MissionsTotal > 0)
        {
            MissionEfficiency = (float) MissionsFulfilled/MissionsTotal;
        }
    }

    public void UpdateFlow()
    {
        if (!GameManager.Player) return;
        try
        {
            FlowLevel = (ThrowAccuracy + SoakingAccuracy + MissionEfficiency)/3;
        }

        catch { }
        AdjustFlow();
    }

    private void AdjustFlow()
    {
        if (!EnvironmentManager.Instance.Player) return;
        // Adjust Ant Behavior
        EnvironmentManager.Instance.Ants.ForEach(n => n.Nav.speed = NavAgentSpeed * (1 + FlowLevel));

        // Adjust Player Properties
        EnvironmentManager.Instance.Player.AdjustFlowValues();

        // Update Multiplier
        Flow.FlowMultiplier = GetMultiplier();
        FlowMultiplier.text = String.Format("x {0}", Mathf.RoundToInt(Flow.FlowMultiplier));
    }

    public void InitializeTask(Task task)
    { 
        int amount = GetObstacleAmount();
        task.Obstacles = new GameObject[amount];
        for (int i = 0; i < amount; i++)
        {
            task.Obstacles[i] = (GameObject)Instantiate(ObstacleGameObject, GlobalFunctions.RandomPointOnNavmesh(Flow.SpawnRadius), Quaternion.identity);
            task.Obstacles[i].AddComponent<Obstacle>();
        }
        task.TTL = GetTaskLivingTime();
        MissionsTotal++;
    }

    public void SetGameSpeed(float value)
    {
        Time.timeScale = value;
    }

    public float GetMultiplier()
    {
        /*******************************************************************
         *  MissionAccuracy | SoakingAccuracy | Thowing Accuracy | Decision
         *  High                x                x               | Very High
         *  x                   High            High             | Very High
         *  Medium              x               High             | High
         *  Medium              High            x                | High
         *  Medium              x               Med              | Medium
         *  Medium              Med             x               | Medium
         *  Low                 x               x                | Low
         ******************************************************************/
         /*****************************************************************
          * Parameter   | Low   | Med        | High
          * Mission     | 0-0.4 | 0.3-0.8    | 0.75 - 1
          * Soaking     | 0-0.2 | 0.15-0.6   | 0.5 - 1
          * Throwing    | 0-0.3 | 0.2-0.8    | 0.75 - 1
          *****************************************************************/
          /****************************************************************
           * Multiplier Low: 1-3, Multiplier Med: 2-4, Multipler Hi: 3-5, Multiplier Very High: 4-6
           * **************************************************************/
        

        bool missionHigh = FuzzyCompare.InSameBoundary(MissionEfficiency, 1, 0.25f, 1, 1);
        if (missionHigh)
            return Random.Range(4, 6);

        bool soakingHi = FuzzyCompare.InSameBoundary(SoakingAccuracy, 1, 0.5f, 1, 1);
        bool throwingHi = FuzzyCompare.InSameBoundary(ThrowAccuracy, 1, 0.75f, 1, 1);
        if (soakingHi && throwingHi)
            return Random.Range(4, 6);
        
        bool missionMed = FuzzyCompare.InSameBoundary(MissionEfficiency, 0.8, 0.5f, 1, 1);
        if (missionMed && (soakingHi || throwingHi))
            return Random.Range(3, 5);

        bool soakingMed = FuzzyCompare.InSameBoundary(SoakingAccuracy, 6, 0.15f, 1, 1);
        bool throwingMed = FuzzyCompare.InSameBoundary(ThrowAccuracy, 8, 0.6f, 1, 1);

        if (missionMed && (throwingMed || soakingMed))
            return Random.Range(2, 4);
        return Random.Range(1, 3);
    }


    public int GetObstacleAmount()
    {
        return (int) (FlowLevel*Flow.SpawnAmount*Flow.FlowMultiplier);
    }

    public float GetTaskLivingTime()
    {
        return Random.Range(60, Flow.MaxMissionTime * (1 - FlowLevel));
    }
}
