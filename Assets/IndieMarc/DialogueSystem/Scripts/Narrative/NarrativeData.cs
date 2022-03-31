using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.DialogueSystem
{
    [System.Serializable]
    public class DialogueMessageData
    {
        public string actor_title;
        public Sprite actor_image;
        public string text;

        public DialogueMessageData(string actor_title, Sprite actor_image, string text)
        {
            this.actor_title = actor_title;
            this.actor_image = actor_image;
            this.text = text;
        }
    }

    [System.Serializable]
    public class NarrativeData
    {
        //Dialogue data
        public List<DialogueMessageData> dialogue_history = new List<DialogueMessageData>();
        public Dictionary<string, int> trigger_counts = new Dictionary<string, int>();

        //Quest data
        public Dictionary<string, int> quests_status = new Dictionary<string, int>(); //0=NotStarted, 1=Ongoing, 2=Completed, 3=Failed

        //Custom data
        public Dictionary<string, int> custom_values_int = new Dictionary<string, int>();
        public Dictionary<string, float> custom_values_float = new Dictionary<string, float>();
        public Dictionary<string, string> custom_values_str = new Dictionary<string, string>();

        public void FixData()
        {
            //Fix data when data version is different
            if (dialogue_history == null)
                dialogue_history = new List<DialogueMessageData>();
            if (trigger_counts == null)
                trigger_counts = new Dictionary<string, int>();

            if (custom_values_int == null)
                custom_values_int = new Dictionary<string, int>();
            if (custom_values_float == null)
                custom_values_float = new Dictionary<string, float>();
            if (custom_values_str == null)
                custom_values_str = new Dictionary<string, string>();
        }

        public void AddToHistory(DialogueMessageData msg)
        {
            dialogue_history.Add(msg);
        }

        public void SetTriggerCount(string event_id, int value)
        {
            if (event_id != "")
            {
                trigger_counts[event_id] = value;
            }
        }

        public int GetTriggerCount(string event_id)
        {
            if (trigger_counts.ContainsKey(event_id))
                return trigger_counts[event_id];
            return 0;
        }

        public void StartQuest(string quest_id)
        {
            if(!IsQuestStarted(quest_id))
                quests_status[quest_id] = 1;
        }

        public void CancelQuest(string quest_id)
        {
            if (IsQuestStarted(quest_id))
                quests_status[quest_id] = 0;
        }

        public void CompleteQuest(string quest_id, bool success=true)
        {
            if (IsQuestStarted(quest_id))
                quests_status[quest_id] = success ? 2 : 3;
        }

        public int GetQuestStatus(string quest_id)
        {
            if (quests_status.ContainsKey(quest_id))
                return quests_status[quest_id];
            return 0;
        }

        public bool IsQuestStarted(string quest_id)
        {
            int status = GetQuestStatus(quest_id);
            return status >= 1;
        }

        public bool IsQuestActive(string quest_id)
        {
            int status = GetQuestStatus(quest_id);
            return status == 1;
        }

        public bool IsQuestCompleted(string quest_id)
        {
            int status = GetQuestStatus(quest_id);
            return status == 2;
        }

        public bool IsQuestFailed(string quest_id)
        {
            int status = GetQuestStatus(quest_id);
            return status == 3;
        }

        public void SetCustomInt(string val_id, int value)
        {
            if (val_id != "")
            {
                custom_values_int[val_id] = value;
            }
        }

        public void SetCustomFloat(string val_id, float value)
        {
            if (val_id != "")
            {
                custom_values_float[val_id] = value;
            }
        }

        public void SetCustomString(string val_id, string value)
        {
            if (val_id != "")
            {
                custom_values_str[val_id] = value;
            }
        }

        public bool HasCustomInt(string val_id)
        {
            return custom_values_int.ContainsKey(val_id);
        }

        public bool HasCustomFloat(string val_id)
        {
            return custom_values_float.ContainsKey(val_id);
        }

        public bool HasCustomString(string val_id)
        {
            return custom_values_str.ContainsKey(val_id);
        }

        public int GetCustomInt(string val_id)
        {
            if (custom_values_int.ContainsKey(val_id))
            {
                return custom_values_int[val_id];
            }
            return 0;
        }

        public float GetCustomFloat(string val_id)
        {
            if (custom_values_float.ContainsKey(val_id))
            {
                return custom_values_float[val_id];
            }
            return 0;
        }

        public string GetCustomString(string val_id)
        {
            if (custom_values_str.ContainsKey(val_id))
            {
                return custom_values_str[val_id];
            }
            return "";
        }

        public void DeleteCustomInt(string val_id)
        {
            custom_values_int.Remove(val_id);
        }

        public void DeleteCustomFloat(string val_id)
        {
            custom_values_float.Remove(val_id);
        }

        public void DeleteCustomString(string val_id)
        {
            custom_values_str.Remove(val_id);
        }
        
        public static NarrativeData Get()
        {
            return NarrativeManager.Get().dialogue_data;
        }

    }

}