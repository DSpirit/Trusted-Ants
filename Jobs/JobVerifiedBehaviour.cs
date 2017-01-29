using UnityEngine;
using System.Collections;
using Assets.Scripts.Jobs;
using Assets.Scripts.Manager;

public class JobVerifiedBehaviour : StateMachineBehaviour {

    public Job Job;
    public MeshRenderer Food;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Job = animator.GetComponent<Job>();
        Job.gameObject.GetComponent<SphereCollider>().gameObject.SetActive(true);
        Food = animator.GetComponent<MeshRenderer>();
        Food.material = EnvironmentManager.Instance.SpawnManager.Materials.Find(n => n.name == "Orange");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!Job.JobContract.ContractClosed)
        {
            Job.JobContract.CalculateSpeedup();
        }
            
        animator.SetFloat("Speedup", Job.JobContract.Speedup);
        animator.SetBool("Closed", Job.JobContract.ContractClosed);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        try
        {
            Job.transform.SetParent(Job.JobsContainer.transform);
            if (animator.GetBool("Closed"))
            {
                Job.Owner.AdjustFitness(Job.JobContract.Speedup);
                Destroy(Job);
                Destroy(animator.gameObject, 1);
                animator.SetTrigger("Validated");

            }
        }
        catch { }
        
            
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
