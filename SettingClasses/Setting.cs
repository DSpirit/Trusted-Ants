using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Ant;
using Assets.Scripts.Norms;

namespace Assets.Scripts.SettingClasses
{
    [Serializable]
    public class Setting
    {
        public bool NavmeshPriorityCollision = true;
        public int Capacity = 3;
        public int MaximumRank = 20;
        public float SpawnRadius = 500;
        public float RefreshRate = 3f;
        public float WalkRadius = 500;
        public float SearchRadius = 40;
        public JobSetting JobSetting = new JobSetting();        
        public int GraphUpdateInterval = 5;
        public int MeetingTimeToLive = 20;
        public List<AntTypeSetting> AntTypeSettings = new List<AntTypeSetting>();
        public List<string> HighScore = new List<string>();
        public List<Norm> Norms = new List<Norm>();
        public Difficulty Mode = Difficulty.Hard;
        public List<FlowSetting> FlowSettings = new List<FlowSetting>();
        public FlowSetting Flow()
        {
            return FlowSettings.Find(n => n.Mode == Mode);
        }

        public Setting()
        {
            InitFlowSettings();
            InitAntTypeSettings();
        }

        public void InitFlowSettings()
        {
            FlowSettings.Clear();
            // Load all Roles to List
            foreach (FlowSetting setting in from Difficulty mode in Enum.GetValues(typeof(Difficulty)) select new FlowSetting { Mode = mode })
            {
                FlowSettings.Add(setting);
            }
        }

        public void InitAntTypeSettings()
        {
            AntTypeSettings.Clear();
            // Load all Roles to List
            foreach (AntTypeSetting setting in from AntType role in Enum.GetValues(typeof(AntType)) select new AntTypeSetting {AntType = role})
            {
                AntTypeSettings.Add(setting);
            }
        }
    }
}
