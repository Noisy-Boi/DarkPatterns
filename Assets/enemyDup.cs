using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyDup : MonoBehaviour
{
	public GameObject duplicate;
	public GameObject enemy;
	Vector3 deathspot;
//	Enemy Enemy;
//	int enemyhp;
	
	void Start()
	{
		deathspot = enemy.transform.localPosition;	}
	
    void Update()
    {
		
        if (enemy == null)
		{for (int i = 0; i < 3; i++){
		Instantiate(duplicate, deathspot, Quaternion.identity);
		Debug.Log("Duplicated");
		
		Destroy(gameObject);

		}}
        
    }
}
