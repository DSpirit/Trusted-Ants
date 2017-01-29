using System;
using Assets.Scripts.Jobs;
using Assets.Scripts.Manager;
using Assets.Scripts.SettingClasses;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Ant.AgentTypes
{
    public class AgentFactory
    {
        private readonly Setting _settings = EnvironmentManager.Instance.Settings;
        public Agent CreateAgent(Groups group, AntType type)
        {
            Agent agent;
            switch (type)
            {
                case AntType.AdaptiveAnt:
                    agent = new AdpativeAgent();
                    break;
                case AntType.FreeriderAnt:
                    agent = new FreeriderAgent();
                    break;
                case AntType.EgoistAnt:
                    agent = new EgoistAgent();
                    break;
                case AntType.CunningAnt:
                    agent = new CunningAgent();
                    break;
                case AntType.SloppyAnt:
                    agent = new SloppyAgent();
                    break;
                case AntType.AltruisticAnt:
                    agent = new AltruisticAgent();
                    break;
                default: agent = new AdpativeAgent();
                    break;
            }
            agent.Id = Guid.NewGuid();
            agent.AntType = type;
            agent.Group = group;
            agent.Rank = (1 + _settings.MaximumRank)/2;//Random.Range(1, _settings.MaximumRank);
            SetBehaviour(agent);

            return agent;
        }

        private void SetBehaviour(Agent a)
        {
            var setting = _settings.AntTypeSettings.Find(n => n.AntType == a.AntType).BehaviourSetting;
            a.AntBehaviour = new AntBehaviour(a, setting);
        }
    }
}