using System;
using UnityEngine;
using System.Collections;

public class TalkingBehaviour : StateMachineBehaviour
{
    private AntAI _ant;
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
	    _ant = animator.GetComponent<AntAI>();
        _ant.TalkingSystem.Play();
        _ant.StateBubble.Talking();

	    if (_ant.Meeting)
	    {
            var meetingType = _ant.Agent.Meeting.MeetingType;
            if (_ant == _ant.Agent.Meeting.Submitter.Ant)
	        {
                switch (meetingType)
                {
                    case ActionType.Distributing:
                        animator.SetTrigger("Distribution");
                        break;
                    case ActionType.Verifying:
                        animator.SetTrigger("Verification");
                        break;
                }
            }
	    }
    }

   
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _ant.TalkingSystem.Stop();
        if (!_ant.Meeting)
        {
            animator.SetTrigger("FinishedMeeting");
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
