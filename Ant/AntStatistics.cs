using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Ant
{
    public class AntStatistics
    {
        public List<Guid> FinishedJobs = new List<Guid>();
        public List<Guid> VerifiedJobs = new List<Guid>();
        public List<Guid> DistributedJobs = new List<Guid>();
        public List<Guid> FailedJobs = new List<Guid>();  
    }
}
