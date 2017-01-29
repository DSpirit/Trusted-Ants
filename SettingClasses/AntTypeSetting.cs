using System;
using Assets.Scripts.Ant;

namespace Assets.Scripts.SettingClasses
{
    [Serializable]
    public class AntTypeSetting
    {
        public AntType AntType;
        public DistributionSetting DistributionSetting = new DistributionSetting();
        public BehaviourSetting BehaviourSetting = new BehaviourSetting();
    }
}
