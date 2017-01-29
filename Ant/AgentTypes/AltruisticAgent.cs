using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Ant.AgentTypes
{
    [Serializable]
    public class AltruisticAgent : Agent
    {
        public AltruisticAgent()
        {
        }

        public override bool AcceptWorkingUnit(Guid submitterId)
        {
            // Altruistic Agents accept every working unit
            return true;
        }
    }
}
