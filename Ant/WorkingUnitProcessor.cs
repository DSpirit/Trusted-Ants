using System;
using System.Linq;
using Assets.Scripts.Ant.AgentTypes;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Jobs;
using UnityEngine;

namespace Assets.Scripts.Ant
{
    [Serializable]
    public class WorkingUnitProcessor : MonoBehaviour
    {
        public Job CurrentWorkingUnitSet;
        public JobPart CurrentWorkingUnit;
        private JobSlots _slots;
        private NavMeshAgent _navAgent;
        private Agent _agent;


        public void Start()
        {
            _slots = GetComponent<JobSlots>();
            _navAgent = GetComponent<NavMeshAgent>();
            _agent = GetComponent<AntAI>().Agent;
        }

        public void Update()
        {
            
        }

        public void ProcessWorkingUnits()
        {
            Reset();

            // Check if there are pending jobs
            //if (!_slots.GetPendingJobs().Any(n => n.UnfinishedJobParts().Any())) return;
            // Assign Current Job
            try
            {
                CurrentWorkingUnitSet = _slots.GetPendingJobs().OrderBy(n => n.Ttl).First();
                bool willProcessJob = _agent.ProcessWorkingUnit(CurrentWorkingUnitSet);
                CurrentWorkingUnitSet.Ani.SetBool("IsActive", true);
                if (!willProcessJob)
                {
                    CurrentWorkingUnitSet.Ani.SetTrigger("Failed");
                    return;
                }
                var jobs = CurrentWorkingUnitSet.UnfinishedJobParts().OrderBy(n => n.Index);
                CurrentWorkingUnit = jobs.First();
                if (CurrentWorkingUnit == null) return;
                
                CurrentWorkingUnit.ProcessJobPart();
                _navAgent.SetDestination(CurrentWorkingUnit.Marker.transform.position);
            }
            catch (Exception ex)
            {
                //Debug.Log(ex.Message);
            }
            
        }

        //The function "OnTriggerEnter" is called when a collision happens.
        public void OnTriggerStay(Collider marker)
        {
            if (CurrentWorkingUnit == null) return;
            if (CurrentWorkingUnit.Id.ToString() != marker.name) return;
            CurrentWorkingUnit.FinishJobPart();
            CurrentWorkingUnit.Marker = null;
            CurrentWorkingUnit = null;   
        }

        private void Reset()
        {
            CurrentWorkingUnitSet = null;
            CurrentWorkingUnit = null;
        }

    }
}

