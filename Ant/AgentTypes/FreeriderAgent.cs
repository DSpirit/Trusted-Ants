using System;

namespace Assets.Scripts.Ant.AgentTypes
{
    [Serializable]
    public class FreeriderAgent : Agent
    {
        public FreeriderAgent()
        {
        }

        public override bool AcceptWorkingUnit(Guid submitterId)
        {
            return false;
        }

        public override bool DistributeWorkingUnit()
        {
            return true;
        }
    }
}
