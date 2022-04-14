using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class dialogue_convo : MonoBehaviour
{
    // Start is called before the first frame update
    public Image mc;
    public Image navvy;
    public Image mc_dissaproval;
    public Image roger;
    public Image mc_fade;
    public Image navvy_fade;
    public Image Roger_fade;
    public Text text1;
    public Text text2;
    public Text text3;
    public Text text4;
    public Text text5;
    public Text text6;
    public Text text7;
    public Button previous;
    public Button next;

    public float currentimagevalue = 0.0f;

    


    

    void Start()
    {
     
        mc.enabled = false;
        mc_dissaproval.enabled = false;
        navvy.enabled = false;
        roger.enabled = false;
        text1.enabled = false;
        text2.enabled = false;
        text3.enabled = false;
        text4.enabled = false;
        text5.enabled = false;
        text6.enabled = false;
        text7.enabled = false;

        mc_fade.enabled = false;
        navvy_fade.enabled = false;
        Roger_fade.enabled = false;
       

        //imageslider.onValueChanged.AddListener(delegate{
            //Debug.Log(imageslider.value);
           // currentimagevalue = imageslider.value;
        //});



        //previous.onClick.AddListener(previmage);
        previous.onClick.AddListener(()=>{
            currentimagevalue = currentimagevalue -1;
            if(currentimagevalue < 0){
            //if(currentimagevalue <= -1){
                //currentimagevalue = 0;
                currentimagevalue = 5;
            }
        });

        next.onClick.AddListener(()=>{
            currentimagevalue = currentimagevalue +1;
            if(currentimagevalue >= 6){
                currentimagevalue = 0;
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        // if(currentimagevalue == 1.0f){
        //     chameleon.enabled = true;
        // }

        // if(currentimagevalue == 2.0f){
        //     chimpanzee.enabled = true;
        // }

        //Debug.Log(imageslider.value);

        if(currentimagevalue == 1.0f){
        mc.enabled = false;
        navvy.enabled = true;
        roger.enabled = false;
        text1.enabled = true;
        text2.enabled = false;
        text3.enabled = false;
        text4.enabled = false;
        text5.enabled = false;
        text6.enabled = false;
        text7.enabled = false;

        mc_fade.enabled = false;
        navvy_fade.enabled = false;
        Roger_fade.enabled = false;
            

     
        }else if(currentimagevalue == 2.0f){
        mc.enabled = false;
        mc_dissaproval.enabled = true;
        navvy.enabled = false;
        roger.enabled = false;
        text1.enabled = false;
        text2.enabled = true;
        text3.enabled = false;
        text4.enabled = false;
        text5.enabled = false;
        text6.enabled = false;
        text7.enabled = false;

        mc_fade.enabled = false;
        navvy_fade.enabled = true;
        Roger_fade.enabled = false;

        }else if(currentimagevalue == 3.0f){
        mc.enabled = false;
        mc_dissaproval.enabled = false;
        navvy.enabled = true;
        roger.enabled = false;
        text1.enabled = false;
        text2.enabled = false;
        text3.enabled = true;
        text4.enabled = false;
        text5.enabled = false;
        text6.enabled = false;
        text7.enabled = false;

        mc_fade.enabled = true;
        navvy_fade.enabled = false;
        Roger_fade.enabled = false;

        }else if(currentimagevalue == 4.0f){
        mc.enabled = false;
        mc_dissaproval.enabled = true;
        navvy.enabled = false;
        roger.enabled = false;
        text1.enabled = false;
        text2.enabled = false;
        text3.enabled = false;
        text4.enabled = true;
        text5.enabled = false;
        text6.enabled = false;
        text7.enabled = false;

        mc_fade.enabled = false;
        navvy_fade.enabled = true;
        Roger_fade.enabled = false;

        }else if (currentimagevalue == 5.0f){
       
      GetComponent<dialogue_convo>().enabled = false;
      GameObject.Find("Conversation").SetActive(false);
        }
        else{
            //default condition
        mc.enabled = false;
        mc_dissaproval.enabled = false;
        navvy.enabled = false;
        roger.enabled = false;
        text1.enabled = false;
        text2.enabled = false;
        text3.enabled = false;
        text4.enabled = false;
        text5.enabled = false;
        text6.enabled = false;
        text7.enabled = false;

        mc_fade.enabled = false;
        navvy_fade.enabled = false;
        Roger_fade.enabled = false;
        }
    }
}
