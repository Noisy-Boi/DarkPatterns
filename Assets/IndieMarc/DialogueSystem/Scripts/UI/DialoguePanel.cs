using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IndieMarc.DialogueSystem
{
    public class DialoguePanel : UIPanel
    {
        public Image portrait;
        public Text title;
        public Text text;

        private static DialoguePanel _instance;

        protected override void Awake()
        {
            base.Awake();
            _instance = this;
        }

        protected override void Start()
        {
            base.Start();
            Hide();
        }

        protected override void Update()
        {
            base.Update();

        }

        public void Set(Sprite portrait, string title, string text)
        {
            this.portrait.sprite = portrait;
            this.title.text = title;
            this.text.text = text;
        }

        public static DialoguePanel Get()
        {
            return _instance;
        }
    }

}