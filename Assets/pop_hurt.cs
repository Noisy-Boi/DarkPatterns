using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pop_hurt : StateMachineBehaviour
{

	private float rand;

  
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<pop_health>().isInvulnerable = true;
		
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

    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   	{
       
    }

   
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
   
   animator.GetComponent<pop_health>().isInvulnerable = true;
   
    }

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
