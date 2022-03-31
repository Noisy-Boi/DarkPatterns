using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Events to start a dialogue
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace IndieMarc.DialogueSystem
{
    public class DialogueEvent : MonoBehaviour
    {
        public bool is_enabled = true;
        [Tooltip("Optional: Only use if you plan to save the NarrativeData, so it can reference to it.")]
        public string event_id;

        [Header("Trigger")]
        [Tooltip("How this dialogue will be triggered")]
        public GameObject trigger; //If not set, will start as soon as conditions are true
        [Tooltip("Number of times it can trigger. Put 0 for Infinity")]
        public int trigger_limit = 1;  //0 means infinite
        [Tooltip("How long before it can trigger again (seconds)")]
        public float trigger_cooldown = 1f;

        [Header("Type")]
        [Tooltip("If true will start automatically, otherwise need to press the talk button")]
        public bool auto_start = true; //Don't need to press a button to start the dialogue
        [Tooltip("Will trigger even if another dialogue is already running")]
        public bool priority = false;
        [Tooltip("Will use the Zoomed-in dialogue mode")]
        public bool zoomed_in = false;
        [Tooltip("Will lock camera to target during dialogue")]
        public GameObject camera_lock_on;

        public UnityAction OnStart;
        public UnityAction OnEnd;

        private DialogueActor actor_trigger;
        private float duration = 0f;
        private float timer = 0f;
        private int trigger_count = 0;

        private List<DialogueMessage> dialogues = new List<DialogueMessage>();
        private List<NarrativeCondition> conditions = new List<NarrativeCondition>();
        private List<NarrativeEffect> effects = new List<NarrativeEffect>();

        private static List<DialogueEvent> event_list = new List<DialogueEvent>();

        private void Awake()
        {
            event_list.Add(this);
            conditions.AddRange(GetComponents<NarrativeCondition>());
            effects.AddRange(GetComponents<NarrativeEffect>());

            if (trigger != null)
            {
                actor_trigger = trigger.GetComponent<DialogueActor>();
            }
            if (trigger != null && trigger.GetComponent<Region>())
            {
                trigger.GetComponent<Region>().OnEnterRegion += (GameObject triggerer) => { Trigger(triggerer); };
            }
        }

        private void OnDestroy()
        {
            event_list.Remove(this);
        }

        void Start()
        {
            trigger_count = NarrativeData.Get().GetTriggerCount(event_id);
            
            //Load dialogues
            duration = 0f;
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                if (child && child.activeSelf && child.GetComponent<DialogueMessage>())
                {
                    DialogueMessage dialogue = child.GetComponent<DialogueMessage>();
                    dialogues.Add(dialogue);
                    duration += dialogue.duration + dialogue.pause;
                }
            }
        }

        void Update()
        {
            if (NarrativeManager.Get().IsPaused())
                return;

            timer += Time.deltaTime;

            if (timer > 0.25f)
            {
                timer = 0f;

                //Trigger cinematic with actor
                if (actor_trigger != null && actor_trigger.active)
                {
                    GameObject player = NarrativeManager.Get().GetNearestPlayer(actor_trigger.transform.position, actor_trigger.trigger_radius);

                    //Start auto
                    if (player != null && auto_start && AreConditionsMet())
                        Trigger(player);
                    
                }
                
            }

            bool accept_pressed = Input.GetKeyDown(NarrativeManager.Get().talk_button);
            if (actor_trigger != null && actor_trigger.active && accept_pressed)
            {
                GameObject player = NarrativeManager.Get().GetNearestPlayer(actor_trigger.transform.position, actor_trigger.trigger_radius);
                
                //Start with button
                if (player != null && !auto_start && actor_trigger.CanBeTalkedEvent())
                {
                    if (AreConditionsMet())
                        Trigger(player);
                }
            }
        }

        public void Trigger(GameObject triggerer = null)
        {
            if (NarrativeManager.Get().GetCurrent() != this)
            {
                NarrativeManager manager = NarrativeManager.Get();
                if (manager.GetCurrent() != this && !manager.IsInCinematicQueue(this))
                {
                    if (AreConditionsMet(triggerer) && timer >= 0f)
                    {
                        trigger_count++;
                        timer = -trigger_cooldown;
                        NarrativeData.Get().SetTriggerCount(event_id, trigger_count);

                        if (priority)
                            manager.StartEvent(this);
                        else
                            manager.AddCinematicToQueue(this);
                    }
                }
            }
        }

        public void TriggerSkipConditions(GameObject triggerer = null)
        {
            NarrativeManager manager = NarrativeManager.Get();
            if (manager.GetCurrent() != this)
            {
                trigger_count++;
                timer = -trigger_cooldown;
                NarrativeData.Get().SetTriggerCount(event_id, trigger_count);

                if (priority)
                    manager.StartEvent(this);
                else
                    manager.AddCinematicToQueue(this);
            }
        }

        public bool AreConditionsMet(GameObject triggerer = null)
        {
            bool are_met = is_enabled && (trigger_count < trigger_limit || trigger_limit <= 0);
            foreach (NarrativeCondition cond in conditions)
            {
                if (!cond.IsMet(triggerer))
                {
                    are_met = false;
                }
            }
            return are_met;
        }

        public void TriggerEffects()
        {
            foreach (NarrativeEffect effect in effects)
            {
                if (effect.enabled)
                    effect.Trigger(gameObject);
            }
        }

        public bool CanBeTriggered()
        {
            bool can_trigger = is_enabled && (trigger_count < trigger_limit || trigger_limit <= 0);
            return can_trigger;
        }

        public int GetTriggerCount()
        {
            return trigger_count;
        }
        
        public float GetDuration()
        {
            return duration;
        }

        public DialogueMessage[] GetDialogues()
        {
            return dialogues.ToArray();
        }

        //Call if you want to reset the trigger count of all events
        public static void ResetAll()
        {
            foreach (DialogueEvent evt in GetAll())
            {
                evt.trigger_count = 0;
                NarrativeData.Get().SetTriggerCount(evt.event_id, 0);
            }
        }

        public static DialogueEvent[] GetAllOf(DialogueActor actor)
        {
            List<DialogueEvent> evt_list = new List<DialogueEvent>();
            foreach (DialogueEvent evt in event_list)
            {
                if (evt.actor_trigger == actor && !evt.auto_start)
                    evt_list.Add(evt);
            }
            return evt_list.ToArray();
        }

        public static DialogueEvent[] GetAll()
        {
            return event_list.ToArray();
        }
    }

}