using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using IndieMarc.TopDown;

namespace IndieMarc.DialogueSystem
{
    public enum NarrativeEffectType
    {
        None = 0,
        CustomInt = 1,
        CustomFloat = 2,
        CustomString = 3,

        Move = 5,

        Show = 10,
        Hide = 11,
        Spawn = 15,
        Destroy = 16,
        
        LockCamera = 23,
        UnlockCamera = 24,
        LockGameplay = 25,
        UnlockGameplay = 26,

        StartQuest = 40,
        CancelQuest = 41,
        CompleteQuest = 42,

        StartDialogue = 50,
        RunEvent = 55,
        RunEventIfMet = 56,

        Wait = 95,
        CallFunction = 99,
    }

    public enum NarrativeEffectOperator
    {
        Add = 0,
        Set = 1,
    }

    [System.Serializable]
    public class NarrativeEffectCallback : UnityEvent<int> { }

    public class NarrativeEffect : MonoBehaviour
    {

        public NarrativeEffectType type;
        public string target_id = "";
        public NarrativeTarget target = new NarrativeTarget();
        public NarrativeEffectOperator oper;
        public GameObject value_object;
        public ScriptableObject value_data;
        public int value_int = 0;
        public float value_float = 1f;
        public string value_string = "";

        [SerializeField]
        public UnityEvent callfunc_evt;

        void Awake()
        {

        }

        void Update()
        {

        }

        public void Trigger(GameObject triggerer)
        {
            if (type == NarrativeEffectType.CustomInt)
            {
                NarrativeData.Get().SetCustomInt(target_id, value_int);
            }

            if (type == NarrativeEffectType.CustomFloat)
            {
                NarrativeData.Get().SetCustomFloat(target_id, value_float);
            }

            if (type == NarrativeEffectType.CustomString)
            {
                NarrativeData.Get().SetCustomString(target_id, value_string);
            }

            if (type == NarrativeEffectType.Move)
            {
                GameObject targ = target.GetTargetObject();
                GameObject targ_pos = value_object;
                if (targ != null && targ_pos != null)
                {
                    targ.transform.position = targ_pos.transform.position;
                }
            }

            if (type == NarrativeEffectType.Show)
            {
                GameObject targ = target.GetTargetObject();
                if (targ)
                    targ.SetActive(true);
            }

            if (type == NarrativeEffectType.Hide)
            {
                GameObject targ = target.GetTargetObject();
                if (targ)
                    targ.SetActive(false);
            }

            if (type == NarrativeEffectType.Spawn)
            {
                GameObject targ = target.GetTargetObject();
                if (targ != null)
                {
                    GameObject.Instantiate(targ, triggerer.transform.position, Quaternion.identity);
                }
            }

            if (type == NarrativeEffectType.Destroy)
            {
                GameObject targ = target.GetTargetObject(triggerer);
                Destroy(targ);
            }
            
            if (type == NarrativeEffectType.StartDialogue)
            {
                GameObject targ = target.GetTargetObject(triggerer);
                if (targ && targ.GetComponent<DialogueEvent>())
                {
                    if (targ.GetComponent<DialogueEvent>().AreConditionsMet())
                        NarrativeManager.Get().StartEvent(targ.GetComponent<DialogueEvent>());
                }
                if (targ && targ.GetComponent<DialogueChoice>())
                {
                    if (targ.GetComponent<DialogueChoice>().AreConditionsMet())
                        targ.GetComponent<DialogueChoice>().Trigger(triggerer);
                }
            }
            
            if (type == NarrativeEffectType.LockCamera)
            {
                FollowCamera pcam = FollowCamera.Get();
                if (pcam != null)
                    pcam.LockCameraOn(value_object);
            }

            if (type == NarrativeEffectType.UnlockCamera)
            {
                FollowCamera pcam = FollowCamera.Get();
                if (pcam != null)
                    pcam.UnlockCamera();
            }

            if (type == NarrativeEffectType.LockGameplay)
            {
                foreach (PlayerControls controls in PlayerControls.GetAll())
                    controls.disable_controls = true;
            }

            if (type == NarrativeEffectType.UnlockGameplay)
            {
                foreach (PlayerControls controls in PlayerControls.GetAll())
                    controls.disable_controls = false;
            }
            
            if (type == NarrativeEffectType.StartQuest)
            {
                NarrativeQuest quest = value_object.GetComponent<NarrativeQuest>();
                NarrativeData.Get().StartQuest(quest.quest_id);
                if(QuestBox.Get())
                    QuestBox.Get().ShowBox(quest, "New Quest");
            }

            if (type == NarrativeEffectType.CancelQuest)
            {
                NarrativeQuest quest = value_object.GetComponent<NarrativeQuest>();
                NarrativeData.Get().CancelQuest(quest.quest_id);
                if (QuestBox.Get())
                    QuestBox.Get().ShowBox(quest, "Quest Failed");
            }

            if (type == NarrativeEffectType.CompleteQuest)
            {
                NarrativeQuest quest = value_object.GetComponent<NarrativeQuest>();
                NarrativeData.Get().CompleteQuest(quest.quest_id);
                if (QuestBox.Get())
                    QuestBox.Get().ShowBox(quest, "Quest Completed");
            }
            
            if (type == NarrativeEffectType.RunEvent)
            {
                GameObject targ = target.GetTargetObject(triggerer);
                if (targ && targ.GetComponent<NarrativeEvent>())
                {
                    targ.GetComponent<NarrativeEvent>().Run(triggerer);
                }
            }

            if (type == NarrativeEffectType.RunEventIfMet)
            {
                GameObject targ = target.GetTargetObject(triggerer);
                if (targ && targ.GetComponent<NarrativeEvent>())
                {
                    targ.GetComponent<NarrativeEvent>().RunIfConditionsMet(triggerer);
                }
            }

            if (type == NarrativeEffectType.CallFunction)
            {
                if (callfunc_evt != null)
                {
                    callfunc_evt.Invoke();
                }
            }

        }

        public float GetWaitTime()
        {
            if (type == NarrativeEffectType.Wait)
            {
                return value_float;
            }
            return 0f;
        }
    }

}