using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAgro : MonoBehaviour
{
 
 	[SerializeField]
 	Transform player;
	
	[SerializeField]
 	float agroRange;
		
	[SerializeField]
	float moveSpeed;
	
	
	Rigidbody2D rb2d;
 
 
 // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //distance to player
		float distToPlayer = Vector2.Distance(transform.position, player.position);
		print("distToPlayer = " + distToPlayer);
		
		if(distToPlayer < agroRange)
		{
			//Code to chase Player
			ChasePlayer();
		}
		else
		{
			//Stop chasing code
			StopChase();
		}
    }
	
	void ChasePlayer()
	{
	if(transform.position.x < player.position.x)
	{
		// enemy to the left, move right
		rb2d.velocity = new Vector2(moveSpeed, 0);
	}
	else if (transform.position.x > player.position.x)
	{
		//enemy right, moves left
		rb2d.velocity = new Vector2(-moveSpeed, 0);
	}
	
	}
	
	void StopChase()
	{}
}
