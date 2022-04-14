using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pop_health : MonoBehaviour
{
	public float popHealth = 3;
	public float damage = 1;
	
	public bool isInvulnerable = false;
	
	private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
		/* if (popHealth >= 1)
		{
		TakeDamage();
		}*/
		
		
	
        if (popHealth <= 0)
		{
		anim.SetTrigger("Death");
		Destroy(gameObject);
		}
    }
	
	public void TakeDamage(float damage)
	{
		if (isInvulnerable)
			return;
			
		popHealth -= damage;
		anim.SetTrigger("Hurt");

	}
}