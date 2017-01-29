using System;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using Assets.Scripts;
using Assets.Scripts.Ant.AgentTypes;
using Assets.Scripts.Manager;
using UnityEngine.Assertions.Must;

public class DataManager : MonoBehaviour, IPanel
{
    public string StartTime;
    public void Start()
    {
        
        CultureInfo ci = new CultureInfo("de-DE");
        StartTime = DateTime.Now.ToString("HH-mm", ci);
    }

    public void Update()
    {

    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public void OnApplicationQuit()
    {
        GenerateStats();
    }

    private void GenerateStats()
    {
        string directory = GlobalFunctions.CreateDirectory("stats");

        SaveAgentData(directory);
        SaveJobData(directory);
        SaveContracts(directory);
    }

    private void SaveContracts(string directory)
    {
        string fileName = "Contracts.csv";
        string pathString = Path.Combine(directory, fileName);
        using (TextWriter tw = new StreamWriter(pathString))
        {
            string dataset;
            foreach (var s in EnvironmentManager.Instance.Contracts.Where(n => n.GotDistributed && n.ContractClosed))
            {
                dataset = string.Format("{0}\t{1}\t{2}\t{3:F}\t{4}",
                s.Creditor.AntType,
                s.ContractStartTime,
                s.ContractEndTime,
                s.Speedup,
                s.Result);
                tw.WriteLine(dataset);
            }
            tw.Close();
        }
    }

    private void SaveJobData(string directory)
    {
        string fileName = "Jobs.csv";

        string pathString = Path.Combine(directory, fileName);
        using (TextWriter tw = new StreamWriter(pathString))
        {
            var stats = Statistics.Instance().JobStatisticsList;
            string dataset;
            foreach (var s in stats)
            {
                foreach (var p in s.Parts)
                {
                    dataset = string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}",
                        s.Id,
                        (int)s.StartedAt,
                        (int)s.FinishedAt,
                        p.Id,
                        (int)p.StartTimestamp,
                        (int)p.FinishTimestamp,
                        p.Successfully);
                    tw.WriteLine(dataset);
                }
            }
            tw.Close();
        }
    }

    private static void SaveAgentData(string directory)
    {
        string fileName = "Agents.csv";
        string pathString = Path.Combine(directory, fileName);
        using (TextWriter tw = new StreamWriter(pathString))
        {
            foreach (var ant in EnvironmentManager.Instance.Ants)
            {
                Agent a = ant.Agent;
                float speedup = a.Contracts.Any() ? a.Contracts.Average(n => n.Speedup) : 1;
                string details = string.Format("{0}\t{1}\t{2}\t{3}\t{4:F}\t{5:F}\t{6:F1}\t{7}\t{8}\t{9}\t{10}\t{11}",
                    a.Id,
                    a.AntType,
                    a.Group,
                    a.Rank,
                    a.Consistency,
                    a.TrustRating,
                    speedup,
                    a.Stats.DistributedJobs.Count,
                    a.Stats.FailedJobs.Count,
                    a.Stats.FinishedJobs.Count,
                    a.Stats.VerifiedJobs.Count,
                    a.Fitness);
                tw.WriteLine(details);
            }
            tw.Close();
        }
    }
    
}
