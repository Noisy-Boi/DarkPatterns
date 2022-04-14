using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class signin_complete : MonoBehaviour
{
    public bool signin1;
    public bool signin2;
    public bool signin3;
    public bool signin4;
    public bool signin5;
    public bool signin6;
    
    //public Button buttonlevelcomplete;
    // Start is called before the first frame update
    void Start()
    {
        bool signin1 = false;
        bool signin2 = false;
        bool signin3 = false;
        bool signin4 = false;
        bool signin5 = false;
        bool signin6 = false;
       
    }

    // Update is called once per frame
    void Update()
    {
        //if( sword = true){ Debug.Log("it works");}
    if(signin1 == true && signin2 == true && signin3 == true && signin4 == true && signin5 == true && signin6 == true){
     Debug.Log("YOU FOUND ALL THE ITEMS");
    //buttonlevelcomplete.gameObject.SetActive(true);
    
       //t SceneManager.LoadScene("ending");
     }
        
    }
}
