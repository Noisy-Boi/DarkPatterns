using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IndieMarc.DialogueSystem
{
    
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Image))]
    public class ButtonIconUI : MonoBehaviour
    {
        public Sprite keyboard_icon;
        public Sprite controller_icon;
        
        private Image img;
        
        void Start()
        {
            img = GetComponent<Image>();
        }

        void Update()
        {
            img.sprite = IsUsingController() ? controller_icon : keyboard_icon;
        }

        public static bool IsUsingController()
        {
            //To do, change this function to return wherever controller icons should be displayed or keyboard icons
            return false;
        }
    }

}