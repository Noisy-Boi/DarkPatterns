using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IndieMarc.DialogueSystem
{

    public class DialogueChoicePanel : UIPanel
    {
        public Text choice_text1;
        public Text choice_text2;
        public Text choice_text3;
        public Text choice_text4;

        private DialogueChoice current_choices = null;
        private DialogueActor current_player = null;
        private float timer = 0f;
        private int[] button_map = new int[4];

        private static DialogueChoicePanel _instance;

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

            if (NarrativeManager.Get() == null)
                return;

            if (NarrativeManager.Get().IsPaused())
                return;

            if (current_choices == null)
                return;
            
            timer += Time.deltaTime;
            if (timer < 1f)
                return;
            
            bool diag1 = Input.GetKeyDown(NarrativeManager.Get().choice1_button);
            bool diag2 = Input.GetKeyDown(NarrativeManager.Get().choice2_button);
            bool diag3 = Input.GetKeyDown(NarrativeManager.Get().choice3_button);
            bool diag4 = Input.GetKeyDown(NarrativeManager.Get().choice4_button);

            if (diag1)
                SelectChoice(button_map[0]);
            if (diag2)
                SelectChoice(button_map[1]);
            if (diag3)
                SelectChoice(button_map[2]);
            if (diag4)
                SelectChoice(button_map[3]);
            if (Input.GetKeyDown(KeyCode.Return))
                CancelChoice();
        }

        public void Show(DialogueChoice cinematic_choice, DialogueActor player_trigger)
        {
            if (cinematic_choice == null)
                return;

            current_choices = cinematic_choice;
            current_player = player_trigger;

            DialogueChoiceItem[] choices = cinematic_choice.GetChoices();
            int button_map_index = 0;
            for (int i = 0; i < 4; i++)
            {
                button_map[i] = -1;
                if (choices.Length >= (i + 1) && choices[i].AreConditionsMet())
                {
                    button_map[button_map_index] = i;
                    button_map_index++;
                }
            }

            choice_text1.gameObject.SetActive(button_map[0] >= 0);
            choice_text2.gameObject.SetActive(button_map[1] >= 0);
            choice_text3.gameObject.SetActive(button_map[2] >= 0);
            choice_text4.gameObject.SetActive(button_map[3] >= 0);

            if (button_map[0] >= 0)
                choice_text1.text = choices[button_map[0]].text;
            if (button_map[1] >= 0)
                choice_text2.text = choices[button_map[1]].text;
            if (button_map[2] >= 0)
                choice_text3.text = choices[button_map[2]].text;
            if (button_map[3] >= 0)
                choice_text4.text = choices[button_map[3]].text;


            Show();
            timer = 0f;
        }

        public void SelectChoice(int choice)
        {
            if (current_choices == null)
                return;

            if (choice < 0)
                return;

            if (choice < current_choices.GetChoices().Length)
            {
                NarrativeManager.Get().DoDiagChoice(choice);
            }
        }

        public void CancelChoice()
        {
            NarrativeManager.Get().CancelDiagChoice();
        }

        public void AfterSelectChoice()
        {
            current_choices = null;
            Hide();
        }

        public bool IsOpenedFor(DialogueActor player)
        {
            return IsVisible() && (current_player == player);
        }

        public static DialogueChoicePanel Get()
        {
            return _instance;
        }
    }

}