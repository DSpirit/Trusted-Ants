using System;
using System.Linq;
using Assets.Scripts.Ant.AgentTypes;
using Assets.Scripts.Jobs;
using Assets.Scripts.Manager;
using UnityEngine;

namespace Assets.Scripts.Norms
{
    [Serializable]
    public class Contract
    {
        public Job ParentJob;
        public Job CurrentJob;
        public Agent Creditor;
        public Agent Debtor;
        public Norm BusinessNorm;
        public bool GotDistributed;
        public float ContractStartTime;
        public float ContractEndTime;
        public float CommunicationTime;
        public float EstimatedEffort;
        public bool ContractClosed;
        public float Speedup;
        public bool Result;
        
        public Contract(Job newJob)
        {
            Debtor = newJob.Owner;
            ParentJob = null;
            CurrentJob = newJob;
            EstimatedEffort = CurrentJob.EstimatedEffort;
            ContractStartTime = Time.time;
        }

        public void Initialize(Agent debtor, Job copyJob)
        {
            GotDistributed = true;
            Creditor = Debtor;
            Debtor = debtor;
            ParentJob = CurrentJob;
            CurrentJob = copyJob;
            ContractStartTime = Time.time;
        }

        public bool ContractFulfilled()
        {
            return CurrentJob.SuccessfullyFinished();
        }

        public bool ContractFailed()
        {
            bool failed = false;
            try
            {
                failed = CurrentJob.Exceeded() || ParentJob.Exceeded();
            }
            catch { }
            if (failed)
            {
                Speedup = 0;
                CloseContract();
            }
            return failed;
        }
       
        public void CloseContract()
        {
            if (Creditor == null)
            {
                Debtor.Speedup.Add(Speedup);
                EnvironmentManager.Instance.Observer.Speedup[Debtor.AntType].Add(Speedup);
            }
            else
            {
                Creditor.Speedup.Add(Speedup);
                EnvironmentManager.Instance.Observer.Speedup[Creditor.AntType].Add(Speedup);
            }
            ContractClosed = true;
            ContractEndTime = Time.time;
        }


        public void CalculateSpeedup()
        {
            if (ContractClosed) return;
            // No speedup achieved
            if (!GotDistributed)
            {
                Speedup = 1;
                CloseContract();
                // Speedup can only be measured when distributed, so no speedup will be added here
                //EnvironmentManager.Instance.Observer.Speedup[Debtor.AntType].Add(Speedup);
                return;
            }

            if (ContractFailed())
            {
                Speedup = 0;
                CloseContract();
                return;
            }

            if (CurrentJob.SuccessfullyFinished())
            {
                Result = true;
            }

            // Communication time is automatically integrated due to not stopping the time during conversations
            if (ParentJob.SuccessfullyFinished() && CurrentJob.SuccessfullyFinished())
            {
                Speedup = ParentJob.EstimatedEffort / GetLatestJobTimestamp();
                Speedup = Speedup >= 1 ? Speedup : 1;
                CloseContract();
            }
        }

        private float GetLatestJobTimestamp()
        {
            float maximumParent = 0;
            float maximumChild = 0;
            maximumParent = ParentJob.JobParts.Max(n => n.FinishTimeStamp);
            maximumChild = CurrentJob.JobParts.Max(n => n.FinishTimeStamp);

            // This value represents the whole timespan between contract start and finish of the last jobpart
            float max = (maximumChild > maximumParent ? maximumChild : maximumParent) - ContractStartTime;

            return max;
        }

        

        
    }

    
}