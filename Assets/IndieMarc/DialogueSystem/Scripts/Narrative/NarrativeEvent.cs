using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IndieMarc.TopDown;

/// <summary>
/// Script for narrative event (triggering quests or other)
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace IndieMarc.DialogueSystem
{

    public enum NarrativeEventType 
    {
        Manual = -1,
        AutoTrigger = 0,
        AtStart = 1,
        EnterRegion = 2,
        LeaveRegion = 3,
        UseLever = 6,
        CharacterDead = 10,
        DialogueStart = 20,
        DialogueEnd = 21,
    }
    
    public class NarrativeEvent : MonoBehaviour
    {
        [Tooltip("Optional: Only use if you plan to save the NarrativeData, so it can reference to it.")]
        public string event_id;
        [Tooltip("How this event will be triggered")]
        public NarrativeEventType trigger_type;
        [Tooltip("Which object will trigger this event")]
        public GameObject trigger_target;
        [Tooltip("Number of times it can trigger. Put 0 for Infinity")]
        public int trigger_limit = 1; //0 means infinite

        [Header("Comment, no effect")]
        [TextArea(3, 5)]
        public string comment;

        private int trigger_count = 0;
        private float timer = 0f;
        private GameObject last_triggerer;

        private static List<NarrativeEvent> evt_list = new List<NarrativeEvent>();

        private List<NarrativeCondition> conditions = new List<NarrativeCondition>();
        private List<NarrativeEffect> effects = new List<NarrativeEffect>();
        private List<NarrativeChild> childs = new List<NarrativeChild>();

        private List<NarrativeEffect> execList = new List<NarrativeEffect>();
        private List<NarrativeChild> childExecList = new List<NarrativeChild>();

        void Awake()
        {
            evt_list.Add(this);
            conditions.AddRange(GetComponents<NarrativeCondition>());
            effects.AddRange(GetComponents<NarrativeEffect>());

            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child_obj = transform.GetChild(i).gameObject;
                if (child_obj.GetComponent<NarrativeEffect>())
                {
                    NarrativeChild child = new NarrativeChild();
                    child.game_obj = child_obj;
                    child.conditions.AddRange(child_obj.GetComponents<NarrativeCondition>());
                    child.effects.AddRange(child_obj.GetComponents<NarrativeEffect>());
                    childs.Add(child);
                }
            }
        }

        void OnDestroy()
        {
            evt_list.Remove(this);
        }

        void Start()
        {
            if (trigger_type == NarrativeEventType.EnterRegion)
            {
                trigger_target.GetComponent<Region>().OnEnterRegion += OnTriggerEvent;
            }
            if (trigger_type == NarrativeEventType.LeaveRegion)
            {
                trigger_target.GetComponent<Region>().OnExitRegion += OnTriggerEvent;
            }

            if (trigger_type == NarrativeEventType.AtStart)
            {
                RunIfConditionsMet();
            }

            if (trigger_type == NarrativeEventType.DialogueStart)
            {
                if (trigger_target.GetComponent<DialogueEvent>())
                {
                    trigger_target.GetComponent<DialogueEvent>().OnStart += OnTriggerEvent;
                }
                else if (trigger_target.GetComponent<DialogueMessage>())
                {
                    trigger_target.GetComponent<DialogueMessage>().OnStart += OnTriggerEvent;
                }
            }

            if (trigger_type == NarrativeEventType.DialogueEnd)
            {
                if (trigger_target.GetComponent<DialogueEvent>())
                {
                    trigger_target.GetComponent<DialogueEvent>().OnEnd += OnTriggerEvent;
                }
                else if (trigger_target.GetComponent<DialogueMessage>())
                {
                    trigger_target.GetComponent<DialogueMessage>().OnEnd += OnTriggerEvent;
                }
            }
            
            if (trigger_type == NarrativeEventType.UseLever)
            {
                if (trigger_target.GetComponent<Lever>())
                {
                    trigger_target.GetComponent<Lever>().OnTriggerLever += OnTriggerEvent;
                }
            }

            if (trigger_type == NarrativeEventType.CharacterDead)
            {
                if (trigger_target.GetComponent<PlayerCharacter>())
                {
                    trigger_target.GetComponent<PlayerCharacter>().onDeath += OnTriggerEvent;
                }
            }
        }

        void Update()
        {
            if (NarrativeManager.Get().IsPaused())
                return;

            timer += Time.deltaTime;

            if (execList.Count > 0)
            {
                if (timer >= 0f)
                {
                    NarrativeEffect effect = execList[0];
                    execList.RemoveAt(0);

                    if (effect.enabled)
                    {
                        effect.Trigger(last_triggerer);
                        timer = -effect.GetWaitTime();
                    }

                    CheckIfEnd();
                }
            }
            else if (childExecList.Count > 0)
            {
                //Still running
                if (timer >= 0f)
                {
                    NarrativeChild child = childExecList[0];
                    childExecList.RemoveAt(0);

                    float wait = child.RunIfMet(last_triggerer);
                    timer = -wait;

                    CheckIfEnd();
                }
            }
            else
            {
                if (trigger_type == NarrativeEventType.AutoTrigger && AreConditionsMet())
                {
                    Run();
                }
            }
        }

        public void AddConditions(NarrativeCondition[] group_conditions)
        {
            conditions.AddRange(group_conditions);
        }

        public void OnTriggerEvent(GameObject triggerer)
        {
            last_triggerer = triggerer;
            RunIfConditionsMet(triggerer);
        }

        public void OnTriggerEvent()
        {
            last_triggerer = null;
            RunIfConditionsMet();
        }
        
        public void RunIfConditionsMet(GameObject triggerer = null)
        {
            if (AreConditionsMet(triggerer))
            {
                Run(triggerer);
            }
        }

        public bool AreConditionsMet(GameObject triggerer = null)
        {
            bool conditions_met = true;
            foreach (NarrativeCondition condition in conditions)
            {
                if (condition.enabled && !condition.IsMet(triggerer))
                {
                    conditions_met = false;
                }
            }

            int game_trigger_count = NarrativeData.Get().GetTriggerCount(event_id);
            bool below_max = (trigger_limit == 0 || game_trigger_count < trigger_limit)
                && (trigger_limit == 0 || trigger_count < trigger_limit);

            return (conditions_met && below_max);
        }

        public void Run(GameObject triggerer = null)
        {
            //Increment
            int cur_val = NarrativeData.Get().GetTriggerCount(event_id);
            NarrativeData.Get().SetTriggerCount(event_id, cur_val + 1);
            last_triggerer = triggerer;
            trigger_count++;

            //Add to exec list
            execList.AddRange(effects);
            childExecList.AddRange(childs);

        }
        
        private void CheckIfEnd()
        {
            if (execList.Count == 0 && childExecList.Count == 0)
            {
                //Ended event

            }
        }

        //Call if you want to reset the trigger count of all events
        public static void ResetAll()
        {
            foreach (NarrativeEvent evt in GetAll())
            {
                evt.trigger_count = 0;
                NarrativeData.Get().SetTriggerCount(evt.event_id, 0);
            }
        }

        public static NarrativeEvent[] GetAll()
        {
            return evt_list.ToArray();
        }
    }

    public class NarrativeChild
    {
        public GameObject game_obj;
        public List<NarrativeCondition> conditions = new List<NarrativeCondition>();
        public List<NarrativeEffect> effects = new List<NarrativeEffect>();

        public bool AreConditionsMet(GameObject triggerer = null)
        {
            bool conditions_met = true;
            foreach (NarrativeCondition condition in conditions)
            {
                if (condition.enabled && !condition.IsMet(triggerer))
                {
                    conditions_met = false;
                }
            }
            return (conditions_met);
        }

        public float RunIfMet(GameObject triggerer = null)
        {
            float wait_time = 0f;
            if (AreConditionsMet(triggerer))
            {
                foreach (NarrativeEffect effect in effects)
                {
                    effect.Trigger(triggerer);
                    wait_time = effect.GetWaitTime();
                }
            }
            return wait_time;
        }
    }
}