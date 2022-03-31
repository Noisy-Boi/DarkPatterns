using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.DialogueSystem
{
    public class NarrativeQuest : MonoBehaviour {

        [Tooltip("Important: make sure all quests have a unique ID")]
        public string quest_id;

        public string title;
        [TextArea(3, 5)]
        public string desc;

        private static List<NarrativeQuest> quest_list = new List<NarrativeQuest>();

        void Awake() {
            quest_list.Add(this);
        }

        private void OnDestroy()
        {
            quest_list.Remove(this);
        }
        
    }
}
