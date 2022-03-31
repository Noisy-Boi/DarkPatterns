using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using IndieMarc.TopDown;

/// <summary>
/// Main manager script for dialogues and quests
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace IndieMarc.DialogueSystem
{
    public class NarrativeManager : MonoBehaviour
    {
        public KeyCode talk_button = KeyCode.Return;
        public KeyCode choice1_button = KeyCode.Alpha1;
        public KeyCode choice2_button = KeyCode.Alpha2;
        public KeyCode choice3_button = KeyCode.Alpha3;
        public KeyCode choice4_button = KeyCode.Alpha4;

        public GameObject talk_bubble_prefab;
        public GameObject talk_button_prefab;
        public Sprite default_portrait;
        public Sprite default_portrait_zoomed;

        [HideInInspector] public UnityAction<DialogueEvent> OnDialogueStart;
        [HideInInspector] public UnityAction<DialogueEvent> OnDialogueEnd;

        [HideInInspector]
        public NarrativeData dialogue_data = null;

        private DialogueEvent current_event = null;
        private DialogueMessage current_message = null;
        private DialogueActor current_actor = null;
        private DialogueChoice current_choice = null;
        private DialogueActor choice_player = null;

        private AudioSource audio_source;
        private int dialogue_index = -1;
        private GameObject talk_bubble_inst;
        private Vector3 talk_bubble_size;
        
        private List<DialogueEvent> cinematic_queue = new List<DialogueEvent>();

        private bool paused = false;
        private float cinematic_timer = 0f;
        private float dialogue_timer = 0f;
        private float interact_timer = 0f;

        private static NarrativeManager _instance;
        private void Awake()
        {
            _instance = this;
            audio_source = GetComponent<AudioSource>();

            if (dialogue_data == null)
                LoadData();
            if (dialogue_data == null)
                dialogue_data = new NarrativeData();
            dialogue_data.FixData();
        }

        public void LoadData()
        {
            //TO DO: load dialogue data from save file
            //ex: dialogue_data = LoadFromFile("file_name");
        }

        public void SaveData()
        {
            //TO DO: Save to file
            //ex: SaveToFile("file_name", dialogue_data);
        }

        void Start()
        {
            talk_bubble_inst = GameObject.Instantiate(talk_bubble_prefab);
            talk_bubble_size = talk_bubble_inst.transform.localScale;
            talk_bubble_inst.SetActive(false);
        }

        void Update()
        {
            if (NarrativeManager.Get().IsPaused())
                return;

            interact_timer += Time.deltaTime;

            if (current_event != null)
            {
                cinematic_timer += Time.deltaTime;
                dialogue_timer += Time.deltaTime;

                //Stop dialogue
                if (current_message != null)
                {
                    bool auto_stop = !current_event.zoomed_in || (current_actor == null);
                    if (auto_stop)
                    {
                        if (dialogue_timer > current_message.duration + current_message.pause)
                        {
                            StopDialogue();
                        }
                        else if (dialogue_timer > current_message.duration)
                        {
                            HideDialogue();
                        }
                    }
                }

                //Skip to next
                if (Input.GetKeyDown(talk_button) && CanInteract())
                {
                    SkipDialogue();
                }

                //Next Dialogue
                if (current_message == null)
                {
                    int index = 0;
                    foreach (DialogueMessage dialogue in current_event.GetDialogues())
                    {
                        if (current_message == null && index > dialogue_index)
                        {
                            dialogue_index = index;
                            if (dialogue.AreConditionsMet())
                                StartDialogue(dialogue);
                        }
                        index++;
                    }

                    //If still null, stop cinematic
                    if (current_message == null)
                        StopEvent();
                }
                
                //Follow dialogue
                UpdateTalkBubble();
            }

            if (current_event == null && cinematic_queue.Count > 0)
            {
                DialogueEvent next = cinematic_queue[0];
                cinematic_queue.RemoveAt(0);
                StartEvent(next);
            }
        }

        private void UpdateTalkBubble()
        {
            if (current_message != null && current_actor != null)
            {
                talk_bubble_inst.transform.position = current_actor.transform.position + new Vector3(0.65f, 0.65f, 0f);
                talk_bubble_inst.transform.localScale = talk_bubble_size * 0.8f;

                if (current_actor != null)
                {
                    float bubble_flip = Mathf.Sign(current_actor.transform.localScale.x) * (current_actor.bubble_flip ? -1f : 1f);
                    talk_bubble_inst.transform.position = current_actor.transform.position + new Vector3(current_actor.bubble_offset.x * bubble_flip, current_actor.bubble_offset.y, -current_actor.bubble_z);
                    talk_bubble_inst.transform.localScale = talk_bubble_size * current_actor.bubble_size;

                    float mult = talk_bubble_inst.transform.localScale.x * bubble_flip;
                    if (mult < 0f)
                    {
                        Vector3 scale = talk_bubble_inst.transform.localScale;
                        talk_bubble_inst.transform.localScale = new Vector3(scale.x * -1f, scale.y, scale.z);
                    }
                }
            }
        }

        public void AddCinematicToQueue(DialogueEvent dialogue_event)
        {
            if (!IsInCinematicQueue(dialogue_event))
                cinematic_queue.Add(dialogue_event);
        }

        public bool IsInCinematicQueue(DialogueEvent dialogue_event)
        {
            return cinematic_queue.Contains(dialogue_event);
        }

        public void StartEvent(DialogueEvent dialogue_event)
        {

            if (current_event != dialogue_event)
            {
                StopEvent(true);
                //Debug.Log("Start Cinematic: " + cinematic_trigger.gameObject.name);
                current_event = dialogue_event;
                current_message = null;
                dialogue_index = -1;
                cinematic_timer = 0f;
                dialogue_timer = 0f;
                interact_timer = 0f;

                if (current_event.camera_lock_on != null)
                {
                    FollowCamera pcam = FollowCamera.Get();
                    pcam.LockCameraOn(current_event.camera_lock_on);
                }

                if (OnDialogueStart != null)
                {
                    OnDialogueStart.Invoke(dialogue_event);
                }
                if (dialogue_event.OnStart != null)
                {
                    dialogue_event.OnStart.Invoke();
                }
            }
        }

        public void StopEvent(bool interupted = false)
        {
            //Debug.Log("Stop Cinematic");
            DialogueEvent trigger = current_event;
            current_event = null;
            current_message = null;
            talk_bubble_inst.SetActive(false);
            DialoguePanel.Get().Hide();
            DialogueZoomPanel.Get().Hide();

            if (trigger != null && trigger.camera_lock_on != null)
            {
                FollowCamera pcam = FollowCamera.Get();
                pcam.UnlockCamera();
            }

            if (trigger != null && trigger.zoomed_in)
            {
                //TheGame.Instance.UnlockGameplay();
            }

            if (!interupted && trigger != null)
            {
                trigger.TriggerEffects();
                if (OnDialogueEnd != null)
                {
                    OnDialogueEnd.Invoke(trigger);
                }
                if (trigger.OnEnd != null)
                {
                    trigger.OnEnd.Invoke();
                }
            }
        }

        public void StartDialogue(DialogueMessage dialogue)
        {
            //Debug.Log("Start Dialogue " + dialogue_index);
            current_message = dialogue;
            current_actor = dialogue.actor;
            interact_timer = 0f;

            Sprite portrait = default_portrait;
            Sprite portrait_zoom = default_portrait_zoomed;
            string title = "";
            CinematicActorPortrait zoom_portrait = null;
            int side = 1;
            bool flipped = false;
            
            if (current_actor != null)
            {
                portrait = current_actor.portrait;
                portrait_zoom = current_actor.portrait_zoomed;
                zoom_portrait = current_actor.GetPortrait();
                title = current_actor.title;
                side = current_message.side != 0 ? current_message.side : (current_actor.is_player ? -1 : 1);
                flipped = side >= 1;
                talk_bubble_inst.SetActive(true);
            }

            bool display_dialogue = (current_actor != null);
            if (current_event.zoomed_in)
            {
                if (display_dialogue)
                {
                    DialogueZoomPanel.Get().SetDialogue(title, portrait_zoom, zoom_portrait, current_message.text, side, "", flipped);
                    DialogueZoomPanel.Get().Show();
                    //TheGame.Instance.LockGameplay();
                }
            }
            else
            {
                DialoguePanel.Get().Set(portrait, title, current_message.text);
                DialoguePanel.Get().Show();
            }

            if (display_dialogue)
                NarrativeData.Get().AddToHistory(new DialogueMessageData(title, portrait, current_message.text));

            UpdateTalkBubble();
            dialogue_timer = 0f;

            if (audio_source != null && dialogue.voice_clip != null)
                audio_source.PlayOneShot(dialogue.voice_clip, 0.8f);

            if (current_message.OnStart != null)
            {
                current_message.OnStart.Invoke();
            }
        }

        public void HideDialogue()
        {
            if (DialoguePanel.Get().IsVisible())
            {
                DialoguePanel.Get().Hide();
                talk_bubble_inst.SetActive(false);
            }
        }

        public bool CanInteract()
        {
            return interact_timer > 0.5f;
        }

        public void SkipDialogue()
        {
            if (current_event != null)
            {
                if (current_event.zoomed_in)
                {
                    if (DialogueZoomPanel.Get().IsTextAnimCompleted())
                        StopDialogue();
                    else
                        DialogueZoomPanel.Get().SkipTextAnim();
                }
                else
                {
                    StopDialogue();
                }
            }
        }

        public void StopDialogue()
        {
            HideDialogue();
            DialoguePanel.Get().Hide(true);
            DialogueZoomPanel.Get().Hide();

            if (current_message && current_message.OnEnd != null)
            {
                current_message.OnEnd.Invoke();
            }

            if (current_message)
            {
                current_message.TriggerEffects();
            }

            current_message = null;
            current_actor = null;
            interact_timer = 0f;
        }

        public void StartDiagChoice(DialogueChoice dialogue_choice, DialogueActor player_trigger)
        {
            if (IsRunning())
                return;
            if (dialogue_choice == current_choice)
                return;

            //Debug.Log("Trigger choice " + gameObject.name);
            current_choice = dialogue_choice;
            choice_player = player_trigger;
            DialogueChoicePanel.Get().Show(dialogue_choice, player_trigger);
        }

        public void DoDiagChoice(int choice_id)
        {
            //Debug.Log("Select choice " + gameObject.name);
            current_choice.RunChoiceEffect(choice_id);
            current_choice = null;
            DialogueChoicePanel.Get().AfterSelectChoice();
        }

        public void CancelDiagChoice()
        {
            if (current_choice != null)
            {
                DialogueChoicePanel.Get().AfterSelectChoice();
                current_choice = null;
            }
        }
        
        public GameObject GetNearestPlayer(Vector3 pos, float radius, bool alive_only=true)
        {
            GameObject player = null;

            PlayerCharacter pcharacter = PlayerCharacter.GetNearest(pos, radius, alive_only);
            if (pcharacter != null)
                player = pcharacter.gameObject;

            DialogueActor actor = DialogueActor.GetPlayerActor();
            if (actor != null)
            {
                float dist = (actor.transform.position - pos).magnitude;
                if (dist < radius)
                    player = actor.gameObject;
            }

            return player;
        }

        public bool IsRunning()
        {
            return (current_event != null || current_choice != null || cinematic_queue.Count > 0);
        }

        public void Pause(){ paused = true; }
        public void Unpause() { paused = false; }
        public bool IsPaused() { return paused; }

        public DialogueEvent GetCurrent()
        {
            return current_event;
        }

        public DialogueChoice GetCurrentChoice()
        {
            return current_choice;
        }

        public static NarrativeManager Get()
        {
            return _instance;
        }
    }

}