using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actor for dialogues
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace IndieMarc.DialogueSystem
{
    public class DialogueActor : MonoBehaviour
    {
        [Tooltip("Can the player interact with this actor?")]
        public bool active = true;
        [Tooltip("Is the player character")]
        public bool is_player = false;

        [Header("Portrait")]
        public string title;
        [Tooltip("Portrait in gameplay mode")]
        public Sprite portrait;
        [Tooltip("Portrait in Zoomed-in mode")]
        public Sprite portrait_zoomed;
        [Tooltip("Use an gameobject prefab instead of a sprite")]
        public GameObject portrait_zoomed_prefab;

        [Header("Chat Bubble")]
        public float bubble_size = 1f;
        public Vector2 bubble_offset;
        public float bubble_z = 0f;
        public bool bubble_flip = false;

        [Header("Trigger")]
        [Tooltip("How far from the player can this actor be triggered?")]
        public float trigger_radius = 2f;

        private CinematicActorPortrait portrait_instance;

        private DialogueChoice[] choices_list;
        private DialogueEvent[] events_list;

        private static List<DialogueActor> actor_list = new List<DialogueActor>();

        private void Awake()
        {
            actor_list.Add(this);
        }

        private void OnDestroy()
        {
            actor_list.Remove(this);
        }

        void Start()
        {
            choices_list = DialogueChoice.GetAllOf(this);
            events_list = DialogueEvent.GetAllOf(this);

            if (portrait_zoomed_prefab)
            {
                GameObject portrait = Instantiate(portrait_zoomed_prefab);
                portrait_instance = portrait.GetComponent<CinematicActorPortrait>();
                portrait.SetActive(false);
            }

            if (NarrativeManager.Get() != null)
            {
                GameObject tbtn = Instantiate(NarrativeManager.Get().talk_button_prefab, transform.position, Quaternion.identity);
                tbtn.GetComponent<ButtonDisplayTalk>().actor = this;
            }
        }

        public bool CanBeTalked(GameObject triggerer = null)
        {
            return CanBeTalkedEvent(triggerer) || CanBeTalkedChoices(triggerer);
        }

        public bool CanBeTalkedEvent(GameObject triggerer=null)
        {
            if (NarrativeManager.Get() == null)
                return false;

            if (!gameObject.activeSelf || NarrativeManager.Get().IsRunning() || !NarrativeManager.Get().CanInteract())
                return false;

            if (is_player)
                return false;

            bool can_be_talked = false;
            foreach (DialogueEvent evt in events_list)
            {
                if (evt.AreConditionsMet(triggerer))
                    can_be_talked = active;
            }

            return can_be_talked;
        }

        public bool CanBeTalkedChoices(GameObject triggerer = null)
        {
            if (NarrativeManager.Get() == null)
                return false;

            if (!gameObject.activeSelf || NarrativeManager.Get().IsRunning() || !NarrativeManager.Get().CanInteract())
                return false;

            if (is_player)
                return false;

            bool can_be_talked = false;
            foreach (DialogueChoice choice in choices_list)
            {
                if (choice.AreConditionsMet(triggerer))
                    can_be_talked = active && !is_player;
            }

            return can_be_talked;
        }

        public CinematicActorPortrait GetPortrait()
        {
            return portrait_instance;
        }

        public static DialogueActor GetPlayerActor()
        {
            foreach (DialogueActor actor in actor_list)
            {
                if (actor.is_player)
                    return actor;
            }
            return null;
        }

    }

}