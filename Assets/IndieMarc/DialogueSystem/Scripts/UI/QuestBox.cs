using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IndieMarc.DialogueSystem {

    public class QuestBox : UIPanel {

        public Text box_title;
        public Text quest_title;

        private float timer = 0f;

        private static QuestBox _instance;

        protected override void Awake()
        {
            base.Awake();
            _instance = this;
        }

        protected override void Update () {

            base.Update();

            if (IsVisible())
            {
                timer += Time.deltaTime;

                if (timer > 4f)
                {
                    Hide();
                }

            }
        }

        public void ShowBox(NarrativeQuest quest, string text)
        {
            box_title.text = text;
            quest_title.text = quest.title;
            timer = 0f;
            Show();
        }

        public static QuestBox Get()
        {
            return _instance;
        }
    }

}
