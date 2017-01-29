using UnityEngine;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Ant.AgentTypes;
using Assets.Scripts.Jobs;
using Assets.Scripts.Manager;

namespace Assets.Scripts
{
    public class VerificationBehaviour : StateMachineBehaviour
    {

        AntAI _ant;
        public Agent Agent;
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _ant = animator.GetComponent<AntAI>();
            _ant.StateBubble.Verifying();
            Agent = _ant.Agent;
            if (!Agent.Meeting) return;
            
            foreach (Job job in Agent.Jobs.GetDoneJobs())
            {
                var contract = job.JobContract;
                EnvironmentManager.Instance.NewsManager.JobsVerified(job, job.SuccessfullyFinished());
                
                if (contract.ContractFulfilled())
                {
                    contract.Debtor.IncreaseRank();
                }
                else if (contract.ContractFailed())
                {
                    contract.Debtor.DecreaseRank();
                }
                job.Ani.SetBool("Verified", true);
            }
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        //
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
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

}
