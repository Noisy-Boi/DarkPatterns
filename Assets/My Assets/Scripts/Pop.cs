using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pop : MonoBehaviour
{
    
	public float speed;
	public float attackRange;
	private Rigidbody2D rb;
	public bool attack;
	
	public Animator animator;
	
	private Transform target;
	
	public Transform player;

	public bool isFlipped = false;
	
	// Start is called before the first frame update
    void Start()
    {

	
		//declare the rigid body
		rb = this.GetComponent<Rigidbody2D>();
		//find what the hell we're chasing after
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
		
		animator = GetComponent<Animator>();
		animator.SetBool("Attack", attack);
    }

    // Update is called once per frame
    void Update()
    {
		
      //attack when I'm in the same horizontal range
	  if(Mathf.Abs(target.position.y - rb.position.y) <= attackRange)
	  {
	  Attack();
	  }
	  else
	  {
	Move();
	  }
	  
	  //speed for walking, stop moving when attacking
	  if(attack == true)
	  {
	  speed = 0f;
	  }
	  else
	  {
	  speed = 0.5f  ;
	  }
	  
	  }
	  
	  
	  public void Move()
	  {
	//move to player
	attack = false;
	animator.SetBool("Attack", false);
		if(attack == false)
		{
		Vector2 direction = target.position - transform.position;
	  	transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
		LookAtPlayer();
		}
	  }
	  
	  public void Attack()
	  {
	  //attack when I'm in the same horizontal range
		animator.SetBool("Attack", true);
		attack = true;
	  }
	  
	  public void LookAtPlayer()
	{
		Vector3 flipped = transform.localScale;
		flipped.z *= -1f;

		if (transform.position.x > player.position.x && isFlipped)
		{
			transform.localScale = flipped;
			transform.Rotate(0f, 180f, 0f);
			isFlipped = false;
		}
		else if (transform.position.x < player.position.x && !isFlipped)
		{
			transform.localScale = flipped;
			transform.Rotate(0f, 180f, 0f);
			isFlipped = true;
		}
	}
	  
	  }