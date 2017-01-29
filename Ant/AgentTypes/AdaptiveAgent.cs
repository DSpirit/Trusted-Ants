using System;
using System.Linq;
using Assets.Scripts.Jobs;
using Assets.Scripts.Manager;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Ant.AgentTypes
{
    [Serializable]
    public class AdpativeAgent : Agent
    {
        private readonly AntStatsObserver _stats;

        public AdpativeAgent()
        {
            _stats = Object.FindObjectOfType<AntStatsObserver>();
        }

        public override bool GetNormResult(Level normLevel)
        {
            if (normLevel == Level.Low)
                return false;
            return true;
        }
    }
}
