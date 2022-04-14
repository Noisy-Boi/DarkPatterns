using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roger_walk : StateMachineBehaviour
{
 	
	public float speed = 0.5f;
	public float attackRange = 5f;
	private Rigidbody2D rb;
	private int rand;
	
	Transform player;
	
	Boss_flip boss;
	
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
     
	 		//declare the rigid body
		rb = animator.GetComponent<Rigidbody2D>();
		//find what the hell we're chasing after
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
	 	boss = animator.GetComponent<Boss_flip>();
    }

 
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       boss.LookAtPlayer();
	  	
		//move towards player
		Vector2 target = new Vector2(player.position.x, rb.position.x );
		Vector2 newPos = Vector2.MoveTowards(rb.position, target,  speed * Time.deltaTime);
		rb.MovePosition(newPos);
		
		if(Mathf.Abs(player.position.y - rb.position.y) <= attackRange)
		//if (Vector2.Distance(player.position.y, rb.position.y) <= attackRange)
		{ 
		animator.SetTrigger("Attack");
		}
		
	}

 override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }
	
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
