using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class pop_attack : StateMachineBehaviour
{
     
   private float rand;
  // private Enemy hurt;
   
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
	
	//hurt = animator.GetComponent<Enemy>();
    rand = Random.Range(0, 2);
	
	if (rand == 0)
	{
		animator.SetBool("Walking", false);
	}
	else
	{
		animator.SetBool("Walking", true);
	}
	}
/*
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Enemy.onHit != null)
		{
		animator.SetTrigger("Hurt");
		}
		
		if (Enemy.hp <= 0f)
		{
		animator.SetTrigger("Death");
		}
    }
*/
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
