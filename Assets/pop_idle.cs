using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class pop_idle : StateMachineBehaviour
{
	
	public float timer;
	public float minTime;
	public float maxTime;
	public float attackRange;
	
	Transform player;
	private Rigidbody2D rb;
	
	//Boss_flip boss;
	
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
		//declare the rigid body
		rb = animator.GetComponent<Rigidbody2D>();
	
     timer = Random.Range(minTime, maxTime);
	// boss = animator.GetComponent<Boss_flip>();
	 player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

   
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
	
	animator.GetComponent<Boss_flip>().LookAtPlayer();

	if(Mathf.Abs(player.position.y - rb.position.y) <= attackRange)
		//if (Vector2.Distance(player.position.y, rb.position.y) <= attackRange)
		{ 
		animator.SetTrigger("Attack");
		}
	
               if (timer <= 0)
	   {
	   	animator.SetBool("Walking", true);
	   }
	   else
	   {
	   	timer -= Time.deltaTime;
	   }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
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
