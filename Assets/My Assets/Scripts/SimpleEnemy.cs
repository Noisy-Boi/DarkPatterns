using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    
	public float speed;
	public float startDist;
	public float stopDist;
	private Rigidbody2D rb;
	public bool attack;
	
	public Animator animator;
	
	private Transform target;
	
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

      //agro - only attack if im close enough
	  if(Vector2.Distance(transform.position, target.position) < startDist)
	  {
	 animator.SetBool("Attack", true);
		Attack();
	  }
	  else
	  {
	  animator.SetBool("Attack", false);
	  Chill();
	  }

	// stop moving if it gets too close
	if(Vector2.Distance(transform.position, target.position) < stopDist){
	 transform.position = Vector2.MoveTowards(transform.position, target.position, speed * 0);

	 }
	 
    }
	
	void Attack()
	{
		Vector3 direction = target.position - transform.position;
	  	transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;
		rb.rotation = angle;
		
		
	}
	
	void Chill()
	{
	
	rb.rotation = 0;
	}
}
