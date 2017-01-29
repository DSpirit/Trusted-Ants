using UnityEngine;
using System.Collections;
using Assets.Scripts.Jobs;
using Assets.Scripts.Manager;

public class JobProcessingBehaviour : StateMachineBehaviour {

    public Job Job;
    public MeshRenderer Food;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Job = animator.GetComponent<Job>();
        Food = animator.GetComponent<MeshRenderer>();
        Food.material = EnvironmentManager.Instance.SpawnManager.Materials.Find(n => n.name == "Yellow");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Job.Ttl < 1)
        {
            animator.SetTrigger("Failed");
        }
        else if (Job.SuccessfullyFinished())
        {
            animator.SetTrigger("Finished");
        }
    }

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
