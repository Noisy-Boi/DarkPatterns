using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dialogue Choice options
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace IndieMarc.DialogueSystem
{

    public class DialogueChoice : MonoBehaviour
    {
        [Tooltip("How this dialogue will be triggered")]
        public GameObject trigger;

        [Tooltip("How long before it can trigger again (seconds)")]
        public float trigger_cooldown = 1f;

        private DialogueChoiceItem[] choices;

        private static List<DialogueChoice> choice_event_list = new List<DialogueChoice>();
        private List<NarrativeCondition> conditions = new List<NarrativeCondition>();

        private DialogueActor actor_trigger;
        private DialogueActor actor_player_trigger;
        private float timer = 0f;

        void Awake()
        {
            choice_event_list.Add(this);
            choices = GetComponentsInChildren<DialogueChoiceItem>();
            conditions.AddRange(GetComponents<NarrativeCondition>());
			
			if (trigger != null && trigger.GetComponent<DialogueActor>())
            {
                actor_trigger = trigger.GetComponent<DialogueActor>();
            }
        }

        void OnDestroy()
        {
            choice_event_list.Remove(this);
        }

        void Start()
        {
            
        }

        void Update()
        {
            if (NarrativeManager.Get().IsPaused())
                return;

            if (IsRunning())
            {
                DialogueActor player = actor_player_trigger;

                //Check for cancel
                if (player != null)
                {
                    float dist = (player.transform.position - actor_trigger.transform.position).magnitude;
                    if (dist > actor_trigger.trigger_radius)
                    {
                        NarrativeManager.Get().CancelDiagChoice();
                    }
                }
            }

            else
            {
                timer += Time.deltaTime;
                if (timer > 0f)
                {
                    //Trigger cinematic with actor
                    if (actor_trigger != null && actor_trigger.CanBeTalkedChoices())
                    {
                        GameObject player = NarrativeManager.Get().GetNearestPlayer(actor_trigger.transform.position, actor_trigger.trigger_radius, true);
                        bool accept_pressed = Input.GetKeyDown(NarrativeManager.Get().talk_button);
                        
                        if (player != null && accept_pressed)
                        {
                            float dist = (player.transform.position - actor_trigger.transform.position).magnitude;
                            if (dist < actor_trigger.trigger_radius)
                            {
                                if (AreConditionsMet())
                                    Trigger(player);
                            }
                        }
                    }
                }

            }
        }
        
        public void Trigger(GameObject triggerer)
        {
            if (triggerer == null && actor_player_trigger != null)
                triggerer = actor_player_trigger.gameObject;

            timer = -trigger_cooldown;
            actor_player_trigger = triggerer.GetComponent<DialogueActor>();

            NarrativeManager.Get().StartDiagChoice(this, actor_player_trigger);
        }

        public bool IsRunning()
        {
            return NarrativeManager.Get().GetCurrentChoice() == this;
        }

        public bool AreConditionsMet(GameObject triggerer = null)
        {
            bool are_met = true;
            foreach (NarrativeCondition cond in conditions)
            {
                if (!cond.IsMet(triggerer))
                {
                    are_met = false;
                }
            }
            return are_met;
        }

        public void RunChoiceEffect(int choice_id)
        {
            if (choice_id >= 0 && choice_id < choices.Length)
            {
                DialogueChoiceItem item = choices[choice_id];

                if (item.go_to != null)
                {
                    if (item.go_to.GetComponent<NarrativeEvent>())
                    {
                        GameObject player = actor_player_trigger ? actor_player_trigger.gameObject : null;
                        item.go_to.GetComponent<NarrativeEvent>().RunIfConditionsMet(player);
                    }

                    else if (item.go_to.GetComponent<DialogueEvent>())
                    {
                        GameObject player = actor_player_trigger ? actor_player_trigger.gameObject : null;
                        item.go_to.GetComponent<DialogueEvent>().TriggerSkipConditions(player);
                    }

                    else if (item.go_to.GetComponent<DialogueChoice>())
                    {
                        GameObject player = actor_player_trigger ? actor_player_trigger.gameObject : null;
                        item.go_to.GetComponent<DialogueChoice>().Trigger(player);
                    }
                }
            }
        }

        public DialogueChoiceItem[] GetChoices()
        {
            return choices;
        }

        public static DialogueChoice[] GetAllOf(DialogueActor actor)
        {
            List<DialogueChoice> choice_list = new List<DialogueChoice>();
            foreach (DialogueChoice choice in choice_event_list)
            {
                if (choice.actor_trigger == actor)
                    choice_list.Add(choice);
            }
            return choice_list.ToArray();
        }

        public static DialogueChoice[] GetAll()
        {
            return choice_event_list.ToArray();
        }
    }

}