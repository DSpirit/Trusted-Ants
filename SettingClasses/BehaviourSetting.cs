using System;
using System.Collections.Generic;

namespace Assets.Scripts.SettingClasses
{
    [Serializable]
    public class BehaviourSetting
    {
        public SortOrder JobTtlSortOrder;
        public float JobAcceptationRateMin = 0.1f;
        public float JobAcceptationRateMax = 1f;
        public float JobFinishRateMin;
        public float JobFinishRateMax;

        public bool WillFinishJob = true;
        public float ReputationRelevanceMin = 0.1f;
        public float ReputationRelevanceMax = 1f;
        public AntTypeBehaviour Behaviour;
        public int WorkingThresholdLow = 1;
        public int WorkingThresholdHigh = 20;
        public float CapacityThresholdMin = 0.1f;
        public float CapacityThresholdMax = 1f;

        public List<AntTypeBehaviour> AcceptsAntTypeBehaviours = new List<AntTypeBehaviour>();

        public BehaviourSetting()
        {
            JobTtlSortOrder = SortOrder.Descending;
            Behaviour = AntTypeBehaviour.WellBehaving;
        }
    }

    public enum SortOrder
    {
        Ascending, Descending
    }

    public enum AntTypeBehaviour
    {
        WellBehaving, BadBehaving
    }
}
