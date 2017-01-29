using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Jobs;
using Assets.Scripts.Manager;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Ant.AgentTypes
{
    public class SloppyAgent : Agent
    {
        public SloppyAgent()
        {
        }

        public override bool AcceptWorkingUnit(Guid submitterId)
        {
            float reputation = CalculateTrustRating(EnvironmentManager.Instance.Observer.GetTrustRatingsFromId(submitterId));
            return reputation > AntBehaviour.JobAcceptationRate;
        }
    }
}
