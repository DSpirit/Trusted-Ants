using System;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Manager;
using Assets.Scripts.Norms;
using Random = UnityEngine.Random;

public class FightingBehaviour : StateMachineBehaviour {
    private AntAI _ant;
    private float _distance;
    private Vector3 _playerPos;
    public float RunAway = 50.0f;
    

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _ant = animator.GetComponent<AntAI>();
        _ant.StateBubble.Fighting();
        _playerPos = EnvironmentManager.Instance.Player.GetComponent<Rigidbody>().transform.position;

        // On Incentive go to player, on sanction run away!
        try
        {
            var rewardType = EnvironmentManager.Instance.GameManager.TaskPanel.ActiveTasks.Find(n => n.Target.Ant.Agent.Id == _ant.Agent.Id).Policy.RewardType;
            if (rewardType == RewardType.Incentive)
            {
                _ant.Nav.SetDestination(_playerPos);
            }
            else
            {
                _ant.transform.rotation = Quaternion.LookRotation(_ant.transform.position - _playerPos);
                Vector3 runTo = _ant.transform.position + _ant.transform.forward * RunAway;

                NavMeshHit hit;
                NavMesh.SamplePosition(runTo, out hit, 5, 1 << NavMesh.GetAreaFromName("Default"));

                _ant.Nav.SetDestination(hit.position);

            }
        }
        catch { }
        
        
        _ant.Agent.LookForFood(_ant.HitColliders);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If suiting has been thrown, go to it
        if (_ant.Agent.Meal != null)
        {
            _ant.Nav.SetDestination(_ant.Agent.Meal.transform.position);
            return;
        }

        _distance = Vector3.Distance(_playerPos, _ant.transform.FindChild("Body").position);
        if (_distance < 15f)
        {
            _ant.Nav.ResetPath();
            _ant.AttackingSystem.Play();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _ant.Agent.Meal = null;
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
