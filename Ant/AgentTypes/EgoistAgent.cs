using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Jobs;

namespace Assets.Scripts.Ant.AgentTypes
{
    [Serializable]
    public class EgoistAgent : Agent
    {
        public EgoistAgent()
        {
            
        }

        public override bool ProcessWorkingUnit(Job job)
        {
            // Only process my own jobs i.e. the job is not a distributed one
            return (!job.name.Contains("Child"));
        }

        public override bool DistributeWorkingUnit()
        {
            return true;
        }
    }
}
