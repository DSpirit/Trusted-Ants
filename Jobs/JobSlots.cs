using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Manager;
using Assets.Scripts.SettingClasses;
using UnityEngine;

namespace Assets.Scripts.Jobs
{
    public class JobSlots : MonoBehaviour
    {
        public AntAI Owner;
        public GameObject FinishedJobs;
        public GameObject EnqueuedJobs;

        // Use this for initialization
        void Start ()
        {
            Owner = gameObject.GetComponent<AntAI>();
            FinishedJobs = transform.FindChild("DoneSlot").gameObject;
            EnqueuedJobs = transform.FindChild("PendingSlot").gameObject;
        }
	
        // Update is called once per frame
        void Update ()
        {
	
        }

        public void ReorderAll()
        {
            for (int i = 0; i < EnqueuedJobs.transform.childCount; i++)
            {
                Transform child = EnqueuedJobs.transform.GetChild(i);
                child.localPosition = new Vector3(0.0f, i * child.localScale.y, 0.0f);
            }

            for (int i = 0; i < FinishedJobs.transform.childCount; i++)
            {
                Transform child = FinishedJobs.transform.GetChild(i);
                child.localPosition = new Vector3(0.0f, i * child.localScale.y, 0.0f);
            }
        }

        public void Reorder(string slotName)
        {
            Transform slot = gameObject.transform.Find(slotName);
            for (int i = 0; i < slot.childCount; i++)
            {
                Transform child = slot.GetChild(i);
                child.localPosition = new Vector3(0.0f, i * child.localScale.y, 0.0f);
            }
        }

        public List<Job> GetDoneJobs()
        {
            return FinishedJobs.GetComponentsInChildren<Job>().ToList();
        }

        public bool HasFreeSlot()
        {
            return GetPendingJobs().Count < EnvironmentManager.Instance.Settings.Capacity;
        }


        public List<Job> GetPendingJobs()
        {
            return EnqueuedJobs.GetComponentsInChildren<Job>().ToList();
        }

        // Are there Jobs whose HopCount Limit hasn't been exceeded yet?
        public bool JobForDistributionAvailable()
        {
            return (GetPendingJobs().Any(job => job.HopCount < EnvironmentManager.Instance.Settings.JobSetting.HopCountLimit && job.UnfinishedJobParts().Count > 1));
        }

        public Job GetJobForDistribution()
        {
            var jobTtlSortOrder = Owner.Agent.AntBehaviour.JobTtlSortOrder;
            var jobs = GetPendingJobs().OrderByDescending(n => n.Ttl);
            if (jobTtlSortOrder == SortOrder.Ascending)
                jobs = jobs.OrderBy(n => n.Ttl);

            return jobs.Where(job => job.HopCount < EnvironmentManager.Instance.Settings.JobSetting.HopCountLimit).FirstOrDefault(job => job.UnfinishedJobParts().Count > 1);
        }
    }
}
