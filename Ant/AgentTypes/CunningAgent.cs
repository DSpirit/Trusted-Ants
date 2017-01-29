using System;
using Assets.Scripts.Jobs;
using Assets.Scripts.Manager;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Ant.AgentTypes
{
    [Serializable]
    public class CunningAgent : Agent
    {
        private bool ReachedTreshold;
        private readonly AntStatsObserver _observer;
        public CunningAgent()
        {
            _observer = Object.FindObjectOfType<AntStatsObserver>();
        }

        public override bool AcceptWorkingUnit(Guid submitterId)
        {
            if (TrustRating >= _observer.HighReputationThreshold)
            {
                ReachedTreshold = true;
                return false;
            }
            if (!ReachedTreshold)
            {
                 return true;
            }
            if (AntBehaviour.JobAcceptationRate * TrustRating < _observer.LowReputationThreshold)
            {
                ReachedTreshold = false;
                return true;
            }
            return false;
        }

        public override bool ProcessWorkingUnit(Job job)
        {
            // Oscillating behavior
            if (TrustRating >= _observer.HighReputationThreshold)
            {
                ReachedTreshold = true;
                // Only process my own jobs i.e. the job is not a distributed one
                return !job.name.Contains("Child");
            }
            return true;
        }
    }
}
