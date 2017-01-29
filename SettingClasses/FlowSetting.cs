using System;
using UnityEngine;

namespace Assets.Scripts.SettingClasses
{
    public enum Difficulty
    {
        Easy, Medium, Hard
    }
    [Serializable]
    public class FlowSetting
    {
        public Difficulty Mode;

        [Range(1, 5)]
        public float FlowMultiplier = 3.0f;
        [Range(60, 300)]
        public int MaxMissionTime = 250;

        [Range(0.01f, 0.25f)]
        public float SoakingEffectivity = 0.15f;
        [Range(0.01f, 0.25f)]
        public float HealthIncrease = 0.2f;
        [Range(0.01f, 0.25f)]
        public float ManaIncrease = 0.2f;
        [Range(0.05f, 0.5f)]
        public float ManaConsumption = 0.4f;
        [Range(1,3)]
        public int MaximumEnemies = 3;
        [Range(1,50)]
        public int SpawnAmount = 10;
        public int SpawnRadius = 200;
    }
}
