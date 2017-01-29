using UnityEngine;
using System.Collections;
using Assets.Scripts.Jobs;
using Assets.Scripts.Manager;

public class JobFinishedBehaviour : StateMachineBehaviour {

    public Job Job;
    public MeshRenderer Food;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsActive", false);
        Job = animator.GetComponent<Job>();
        Food = animator.GetComponent<MeshRenderer>();
        Food.material = EnvironmentManager.Instance.SpawnManager.Materials.Find(n => n.name == "Green"); ;
        Job.FinishedTimeStamp = Time.time;
        Job.Owner.Stats.FinishedJobs.Add(Job.Id);
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
