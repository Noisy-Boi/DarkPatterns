using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class trigger_convo : MonoBehaviour

{
    public GameObject canvasObject;

   void Start () {

   }
    
    

     
  void OnMouseDown(){
    
      canvasObject.SetActive(true); 
     //Debug.Log("Clicked");
            
    }

   
       
}

