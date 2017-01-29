using UnityEngine;
using System.Collections;
using Assets.Scripts.Jobs;
using Assets.Scripts.Manager;

public class JobWaitingBehaviour : StateMachineBehaviour
{
    private Job _job;
    public MeshRenderer Food;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _job = animator.GetComponent<Job>();
        Food = animator.GetComponent<MeshRenderer>();
        Food.material = EnvironmentManager.Instance.SpawnManager.Materials.Find(n => n.name == "Purple");
        if (animator.GetBool("Verified"))
        {
            _job.Owner.Stats.VerifiedJobs.Add(_job.Id);
            animator.SetTrigger("Validated");
        }
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

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
