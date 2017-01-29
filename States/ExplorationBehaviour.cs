using System;
using UnityEngine;

public class ExplorationBehaviour : StateMachineBehaviour
{
    private AntAI _ant;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _ant = animator.GetComponent<AntAI>();
        //_ant.GetComponent<Animation>().Play("fireant_walk");
        _ant.StateBubble.Exploring();
        _ant.MoveToRandomPoint();
        //animator.ResetTrigger("FoundPartner");
        animator.ResetTrigger("Leave");
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (_ant.Agent.Meeting != null)
        //{
        //    animator.SetTrigger("FoundPartner");
        //}
    }
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
