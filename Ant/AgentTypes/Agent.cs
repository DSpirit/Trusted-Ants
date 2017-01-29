using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Jobs;
using Assets.Scripts.Manager;
using Assets.Scripts.Norms;
using Assets.Scripts.Player;
using Assets.Scripts.SettingClasses;
using UnityEngine;
using Action = Assets.Scripts.Norms.Action;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Ant.AgentTypes
{
    public enum Level { Low, Medium, High }
    public abstract class Agent : ScriptableObject
    {
        // General Attributes
        public Guid Id;
        public Groups Group;
        public AntType AntType;
        public AntBehaviour AntBehaviour;
        
        public List<int> TrustRatings = new List<int>() { 1 };
        public List<Contract> Contracts = new List<Contract>();
        public List<Guid> Denials = new List<Guid>();
        public List<float> Speedup = new List<float>() { 1 }; // Speedup is set to 1, as this means agents accomplish work theirselves
        public float TrustRating = 0;
        public float Consistency = 1;
        public int Rank;
        public float Fitness;

        // Assigned Objects
        public JobSlots Jobs;
        public NavMeshAgent NavAgent;
        public WorkingUnitProcessor Targets;
        public AntMeeting Meeting;
        public AntStatistics Stats = new AntStatistics();
        public Food Meal;

        // Privates
        private readonly Setting _settings = EnvironmentManager.Instance.Settings;
        public AntAI Ant;

        protected Agent()
        {
            
        }

        public virtual bool AcceptWorkingUnit(Guid submitterId)
        {
            float submitterReputation = CalculateTrustRating(EnvironmentManager.Instance.Observer.GetTrustRatingsFromId(submitterId));

            //var wuOwnerTL = EnvironmentManager.Instance.Observer.GetTL(submitterReputation);
            //float aggTL = GetValueFromLevel(wuOwnerTL);


            return submitterReputation >= EnvironmentManager.Instance.Observer.LowReputationThreshold;
        }

        public virtual bool GetNormResult(Level normLevel)
        {
            return true;
        }

        public float GetValueFromLevel(Level val)
        {
            float value = (int)val;
            return (value/2) * AntBehaviour.JobAcceptationRate;
        }
        
        public Level AcceptanceThreshold(Level workload, Level reputation)
        {
            // Table Mapping from http://ieeexplore.ieee.org/document/6803242/

            if (reputation == Level.Low)
                return Level.Low;

            if (reputation == Level.Medium && workload == Level.Low)
                return Level.High;
            if (reputation == Level.Medium)
                return Level.Medium;

            if (reputation == Level.High && (int) workload < 2)
                return Level.High;
            return Level.Medium;
        }

        protected Level DistributionTrustThreshold(Level workload, Level reputation)
        {
            // http://ieeexplore.ieee.org/document/6114569/
            if (workload == Level.Medium)
                return Level.Low;

            if (workload == Level.Medium)
                return Level.Medium;

            if (workload == Level.High)
                return Level.Medium;
            return Level.Low;
        }

        public Level NormThreshold(Level reputation, Guid submitterId)
        {
            /* Level   | RepLow | RepMid | RepHigh 
             * IncLow    Low      Low      Low
             * IncMed    High     High     Medium
             * IncHigh   High     High     High
             */
            float incentive = GetCalculatedIncentive(submitterId);
            Level incentiveLevel = incentive >= 3 ? Level.High : Level.Medium;
            incentiveLevel = incentive < 1 ? Level.Low : incentiveLevel;
            // When no incentive can be expected, no interest exists
            if (incentiveLevel == Level.Low)
                return Level.Low;
            if (incentiveLevel == Level.High)
                return Level.High;
            return reputation == Level.High ? Level.Medium : Level.High;
        }


        public virtual bool ProcessWorkingUnit(Job job)
        {
            return true;
        }

        public virtual bool DistributeWorkingUnit()
        {
            // Should be implemented, but only when not in game mode...
            return true;
        }

        public void AdjustFitness(float value)
        {
            float incr = value > 1 ? 0.1f : -0.1f;
            float val = Fitness + incr;
            if (val > 1)
                Fitness = 1;
            else if (val < 0)
                Fitness = 0;
            else
                Fitness = val;
        }
        
        public bool RequestMeeting(Guid submitterId)
        {
            if (Denials.Any(n => n == submitterId))
            {
                return Denials.Count(n => n == submitterId) < 3;
            }
            return true;
        }

        public virtual void FindMeetingPartner(Collider[] radarColliders)
        {
            if (Meeting) return;
            //determines one of 0..* neighbour/s relevant for a communication
            foreach (Agent neighbourAnt in radarColliders.Where(h => h.tag == "Ant").Select(hitCollider => hitCollider.gameObject.GetComponent<AntAI>().Agent))
            {
                if (!neighbourAnt.Ant || neighbourAnt.Ant == Ant || neighbourAnt.Ant == Ant.Partner) continue;
                var antState = neighbourAnt.Ant.Ani.GetCurrentAnimatorStateInfo(0);
                if (!neighbourAnt.Meeting && (antState.IsName("Exploring") || antState.IsName("Working")))
                {
                    if (Jobs.JobForDistributionAvailable() && neighbourAnt.Jobs.GetPendingJobs().Count < _settings.Capacity && GlobalFunctions.ConsumerGroup(Group) == neighbourAnt.Group)
                    {
                        if (!RequestMeeting(Id)) return;
                        Ant.Partner = neighbourAnt.Ant;
                        neighbourAnt.Ant.Partner = Ant;
                        Meeting = EnvironmentManager.Instance.SpawnManager.SpawnMeeting(Ant, neighbourAnt.Ant, ActionType.Distributing);
                        Meeting.name = "Distribution Meeting";
                        neighbourAnt.Meeting = Meeting;
                        break;
                    }

                    if (Jobs.GetDoneJobs().Any() && neighbourAnt.Group == GlobalFunctions.VerificatorGroup(Group))
                    {
                        Ant.Partner = neighbourAnt.Ant;
                        neighbourAnt.Ant.Partner = Ant;
                        Meeting = EnvironmentManager.Instance.SpawnManager.SpawnMeeting(Ant, neighbourAnt.Ant, ActionType.Verifying);
                        Meeting.name = "Verification Meeting";
                        neighbourAnt.Meeting = Meeting;
                        break;
                    }

                }
            }
        }

        protected float GetCalculatedIncentive(Guid submitterId)
        {
            // Calculate incentive for job
            float incentive = GlobalFunctions.StandardIncentive;
            foreach (var norm in EnvironmentManager.Instance.Settings.Norms.Where(n => n.IsActive && n.PertinenceCondition == Tag.OnContractStart))
            {
                bool actionApplied = norm.Content == Action.Accept;
                foreach (var policy in norm.Policies)
                {
                    bool keptPolicy = GlobalFunctions.KeptPolicy(policy, submitterId);
                    // Add values for accepting
                    if (keptPolicy && actionApplied)
                    {
                        if (policy.RewardType == RewardType.Incentive)
                            incentive += policy.Reward;
                        if (policy.RewardType == RewardType.Sanction)
                            incentive -= policy.Reward;
                    }
                    // And the opposite for denying
                    else if (keptPolicy)
                    {
                        if (policy.RewardType == RewardType.Incentive)
                            incentive -= policy.Reward;
                        if (policy.RewardType == RewardType.Sanction)
                            incentive += policy.Reward;
                    }
                        
                }
            }
            return incentive;
        }

        public void IncreaseRank()
        {
            int newRank = Rank + 1;
            Rank = newRank <= _settings.MaximumRank ? newRank : _settings.MaximumRank;
            TrustRatings.Add(1);
            UpdateValues();
        }

        public void DecreaseRank()
        {
            int newRank = Rank - 1;
            Rank = newRank > 0 ? newRank : 0;
            TrustRatings.Add(-1);
            UpdateValues();
        }
        
        public void MoveToRandomPoint()
        {
            if (NavAgent.hasPath) return;

            NavAgent.SetDestination(GlobalFunctions.RandomPointOnNavmesh(_settings.WalkRadius));
        }

        public void ExitMeeting()
        {
            Meeting = null;
        }

        // Metrics Calculations

        public void UpdateValues()
        {
            TrustRating = CalculateTrustRating(TrustRatings);
            Consistency = CalculateConsistency(TrustRatings);
        }

        public float CalculateTrustRating(List<int> trustRatings)
        {
            return  (float)trustRatings.Sum() / trustRatings.Select(n => Mathf.Abs(n)).Sum();
        }

        public float CalculateConsistency(List<int> trustRatings)
        {
            float trustRating = CalculateTrustRating(trustRatings);
            if (!TrustRatings.Any())
            {
                return 1;
            }
            try
            {
                int reputationAbs = trustRatings.Sum(n => Math.Abs(n));
                float valuesBiggerZero = (float)trustRatings.Where(n => n > 0).Sum() / reputationAbs * Mathf.Pow(trustRating - 1, 2);
                float valuesSmallerZero = (float)trustRatings.Where(n => n < 0).Sum() / reputationAbs * Mathf.Pow(trustRating + 1, 2);
                return 1 - valuesBiggerZero + valuesSmallerZero;
            }
            catch 
            {
                return 1;
            }
        }

        // Visual Statements

        public void Talk()
        {
            //Ant.Talk();
        }

        public void LookAt(Transform atObject)
        {
            Ant.transform.LookAt(atObject);
        }

        public void LookForFood(Collider[] hitColliders)
        {
            var type = GlobalFunctions.AntTypeFoodMapping[AntType];
            try
            {
                var foodInArea = hitColliders.Select(n => n.GetComponent<Food>());
                var meal = foodInArea.First(n => n != null && n.Type == type && n.Activated);
                if (meal != null)
                    Meal = meal;
            }
            catch
            {
                
            }
        }
    }
}
