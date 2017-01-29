using System;
using System.Collections.Generic;
using Assets.Scripts.Ant.AgentTypes;
using Assets.Scripts.Manager;
using Assets.Scripts.SettingClasses;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Ant
{
    [Serializable]
    public class AntBehaviour
    {
        public SortOrder JobTtlSortOrder = SortOrder.Descending;
        public float JobAcceptationRate = 0.5f;
        public float JobProcessingRate = 1.0f;
        public float ReputationRelevance = 0.5f;
        public AntTypeBehaviour Behaviour = AntTypeBehaviour.WellBehaving;
        public List<AntTypeBehaviour> AcceptsAntTypeBehaviours = new List<AntTypeBehaviour>(); 

        
        public int WorkingThresholdLow;
        public int WorkingThresholdHigh;
        public float WorkloadThreshold;

        public AntBehaviour(Agent ant, BehaviourSetting setting)
        {
            SetBehaviour(setting);
        }

        private void SetBehaviour(BehaviourSetting s)
        {
            JobTtlSortOrder = s.JobTtlSortOrder;
            JobAcceptationRate = s.JobAcceptationRateMax;
            JobProcessingRate = Random.Range(s.JobFinishRateMin, s.JobFinishRateMax);
            ReputationRelevance = Random.Range(s.ReputationRelevanceMin, s.ReputationRelevanceMax);
            // TODO: Necessary? Only CUN!
            WorkingThresholdLow = s.WorkingThresholdLow;
            WorkingThresholdHigh = s.WorkingThresholdHigh;
            // TODO: 
            WorkloadThreshold = Random.Range(WorkingThresholdLow, WorkingThresholdHigh);
            Behaviour = s.Behaviour;
            AcceptsAntTypeBehaviours = s.AcceptsAntTypeBehaviours;

        }
    }
}
