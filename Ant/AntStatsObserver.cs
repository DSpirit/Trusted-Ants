using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Ant.AgentTypes;
using Assets.Scripts.Jobs;
using Assets.Scripts.Manager;
using Assets.Scripts.Norms;
using Assets.Scripts.SettingClasses;
using UnityEngine;

namespace Assets.Scripts.Ant
{
    public class AntStatsObserver : MonoBehaviour
    {
        public List<Agent> Ants = new List<Agent>();
        public float AverageRatings;
        public float AverageRatingsWellBehaving;
        public float AverageRatingsBadBehaving;
        public float AverageConsistency;
        public float AverageRank;
        public float AverageRankWellBehaving;
        public float AverageRankBadBehaving;
        public float LowReputationThreshold;
        public float MediumReputationThreshold;
        public float HighReputationThreshold;
        public Dictionary<AntType, List<float>> Speedup = new Dictionary<AntType, List<float>>();
        public List<Job> Jobs = new List<Job>();
        public List<Contract> Contracts = new List<Contract>();
        public GameObject Hive;


        public void Start()
        {
            InvokeRepeating("UpdateAverageRank", 5, 5);
            InvokeRepeating("UpdateAverageReputation", 5, 5);

            foreach (AntType type in Enum.GetValues(typeof(AntType)))
            {
                Speedup.Add(type, new List<float>());
                Speedup[type].Add(1.0f);
            }
        }

        public void Update()
        {
            if (Hive.transform.childCount > Ants.Count)
                Ants = Hive.GetComponentsInChildren<AntAI>().Select(n => n.Agent).ToList();
            Jobs = EnvironmentManager.Instance.Jobs;
        }

        public List<int> GetTrustRatingsFromId(Guid agentId)
        {
            return EnvironmentManager.Instance.Ants.Find(n => n.Agent.Id == agentId).Agent.TrustRatings;
        }

        public Level WorkloadTotal()
        {
            Level wl = Level.Low;
            float workload = Jobs.Count / Ants.Count;
            if (workload >= 0.34f)
                wl = Level.Medium;
            if (workload >= 0.67f)
                wl = Level.High;
            return wl;
        }

        private void UpdateAverageRank()
            {
                if (!Ants.Any()) return;
                try
                {
                    AverageRank = Ants.Sum(n => n.Rank) / Ants.Count;
                    AverageRankWellBehaving = GetWellBehavingAnts().Sum(m => m.Rank) / GetWellBehavingAnts().Count;
                    AverageRankBadBehaving = GetBadBehavingAnts().Sum(m => m.Rank) / GetBadBehavingAnts().Count;
                }
                catch { }
            
            }

        private void UpdateAverageReputation()
        {
            if (!Ants.Any()) return;
            try
            {
                AverageRatings = Ants.Average(n => n.TrustRating);

                // Thresholds
                LowReputationThreshold = AverageRatings * 0.3f;
                MediumReputationThreshold = AverageRatings;
                HighReputationThreshold = AverageRatings * 0.67f;
            }
            catch { }
            
        }

        public Level GetTL(float reputation)
        {
            Level lv = Level.Low;
            if (reputation > LowReputationThreshold)
                lv = Level.Medium;
            if (reputation > HighReputationThreshold)
                lv = Level.High;
            return lv;
        }

        // Helper
        private List<Agent> GetWellBehavingAnts()
        {
            return Ants.Where(n => n.AntBehaviour.Behaviour.Equals(AntTypeBehaviour.WellBehaving)).ToList();
        }

        private List<Agent> GetBadBehavingAnts()
        {
            return Ants.Where(n => n.AntBehaviour.Behaviour.Equals(AntTypeBehaviour.BadBehaving)).ToList();
        }
    }
}