using System;
using System.Collections.Generic;
using Assets.Scripts.Ant;

namespace Assets.Scripts.SettingClasses
{
    [Serializable]
    public class AntDistributionSetting
    {
        public Groups AntGroup;
        public int Count;
        public AntDistributionSetting(Groups group, int count)
        {
            AntGroup = group;
            Count = count;
        }
    }

    [Serializable]
    public class DistributionSetting
    {
        public List<AntDistributionSetting> FixedCount = new List<AntDistributionSetting>();
        public List<AntDistributionSetting> RandomCount = new List<AntDistributionSetting>();

        public DistributionSetting()
        {
            foreach (Groups g in Enum.GetValues(typeof(Groups)))
            {
                FixedCount.Add(new AntDistributionSetting(g, 6));
                RandomCount.Add(new AntDistributionSetting(g, 2));
            }
        }
    }
}
