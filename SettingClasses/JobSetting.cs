using System;

namespace Assets.Scripts.SettingClasses
{
    [Serializable]
    public class JobSetting
    {
        public int JobSpawnRate = 5;
        public float JobSpawnRadius = 200;
        public int MinimumJobParts = 1;
        public int MaximumJobParts = 3;
        public int MinimumTimeToLive = 300;
        public int MaximumTimeToLive = 600;
        public int HopCountLimit = 4;
        public float DeadAliveTime = 3;
    }
}
