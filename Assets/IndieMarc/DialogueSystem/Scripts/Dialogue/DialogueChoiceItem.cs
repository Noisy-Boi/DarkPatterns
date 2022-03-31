using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.DialogueSystem {

    public class DialogueChoiceItem : MonoBehaviour
    {
        [TextArea(3, 5)]
        public string text;
        public GameObject go_to;

        private NarrativeCondition[] conditions;

        private void Awake()
        {
            conditions = GetComponents<NarrativeCondition>();
        }

        public bool AreConditionsMet(GameObject triggerer = null)
        {
            bool met = true;
            foreach (NarrativeCondition condition in conditions)
            {
                met = met && condition.IsMet(triggerer);
            }
            return met;
        }
    }

}
