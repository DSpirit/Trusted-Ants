using UnityEngine;
using System.Collections;
using Assets.Scripts.Jobs;
using Assets.Scripts.Manager;
using Assets.Scripts.Player;

public class JobFailedBehaviour : StateMachineBehaviour
{

    public Job Job;
    public MeshRenderer Food;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsActive", false);
        Job = animator.GetComponent<Job>();
        Food = animator.GetComponent<MeshRenderer>();
        Food.material = EnvironmentManager.Instance.SpawnManager.Materials.Find(n => n.name == "Red");
        Job.Ttl = -1;
        Job.Owner.Stats.FailedJobs.Add(Job.Id);
        foreach (var part in Job.JobParts)
        {
            part.FinishJobPart();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
