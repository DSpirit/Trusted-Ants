using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts.Ant.AgentTypes;
using Assets.Scripts.Manager;
using Assets.Scripts.Norms;

public class DistributionBehaviour : StateMachineBehaviour {

    AntAI _ant;
    public Agent Agent;
    private readonly NewsManager _news = EnvironmentManager.Instance.NewsManager;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _ant = animator.GetComponent<AntAI>();
        _ant.StateBubble.Distributing();
        Agent = _ant.Agent;
        // Distribute only once
        if (!Agent.Meeting) return;
        

        // Check behaviour before distribution
        var partner = Agent.Meeting.Worker;
        if (!Agent.DistributeWorkingUnit())
        {
            _news.NotDistributed(Agent.Meeting);
            return;
        }
        var jobToCopy = Agent.Jobs.GetJobForDistribution();

        // Consider norms
        if (EnvironmentManager.Instance.NormManager.Norms.Any(n => n.IsActive))
        {
            if (partner.AntType != AntType.AltruisticAnt && partner.AntType != AntType.EgoistAnt)
            {
                Level tt = partner.AcceptanceThreshold(EnvironmentManager.Instance.Observer.WorkloadTotal(), EnvironmentManager.Instance.Observer.GetTL(partner.TrustRating));
                Level normLevel = partner.NormThreshold(tt, Agent.Id);
                if (!partner.GetNormResult(normLevel))
                {
                    EnvironmentManager.Instance.Observer.Speedup[Agent.AntType].Add(0);
                    return;
                }
            }
        }

        if (!partner.AcceptWorkingUnit(Agent.Id))
        {
            EnvironmentManager.Instance.Observer.Speedup[Agent.AntType].Add(0);
            GlobalFunctions.CheckNorm(Agent, partner, Tag.OnContractStart, Action.Deny);
            partner.DecreaseRank();
            _news.NotAccepted(Agent.Meeting);
            return;
        }
        
        if (jobToCopy != null)
        {
            var distributingJob = jobToCopy.CloneJob(partner);
            jobToCopy.name = string.Format("Parent Splitted Job {0}", jobToCopy.HopCount);
            distributingJob.name = string.Format("Child Splitted Job {0}", jobToCopy.HopCount);
            Transform neighbourPendingSlot = partner.Jobs.EnqueuedJobs.transform;
            distributingJob.gameObject.transform.SetParent(neighbourPendingSlot, false);
            Agent.Jobs.ReorderAll();
            partner.Jobs.ReorderAll();
            Agent.Stats.DistributedJobs.Add(jobToCopy.Id);
            _news.JobDistributed(Agent.Meeting, jobToCopy);
            partner.IncreaseRank();
            GlobalFunctions.CheckNorm(Agent, partner, Tag.OnContractStart, Action.Accept);
        }

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _ant.Agent.Meeting.Finished();
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //

    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
