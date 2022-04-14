using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class signin1_bool : MonoBehaviour
{
    
    public GameObject signin1;
    public GameObject signin_complete;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
   void OnMouseDown(){

   //GameObject varGameObject = GameObject.Find("sphere");   
   //then disable or enable script/component

    Debug.Log("itworks");
   signin_complete.GetComponent<signin_complete>().signin1 = true;
  
    //Destroy (unicorn.gameObject,1f);
   //Destroy (unicorn2.gameObject,1f);
    //Destroy (GameObject.FindWithTag("flower"));
    //Destroy(this.GetComponent<MeshRenderer>());
   // unicorntext.text = "✓";
   // unicorntext.color = new Color32(0,255,0,255);// rang is 0 - 255
  }
    }
