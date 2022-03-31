using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace IndieMarc.DialogueSystem
{
    public class DialogueMessage : MonoBehaviour
    {
        public DialogueActor actor;

        [TextArea(3, 10)]
        public string text;

        [Tooltip("For zoomed-in dialogues: 1 is on the right side, -1 is on the left")]
        public int side = 0;
        public AudioClip voice_clip = null;

        [Tooltip("For in-game dialogues: time dialogue is shown")]
        public float duration = 4f;
        [Tooltip("For in-game dialogues: time of the pause between this dialogue and the next one")]
        public float pause = 0.5f;

        public UnityAction OnStart;
        public UnityAction OnEnd;
        
        private List<NarrativeCondition> conditions = new List<NarrativeCondition>();
        private List<NarrativeEffect> effects = new List<NarrativeEffect>();

        void Start()
        {
            conditions.AddRange(GetComponents<NarrativeCondition>());
            effects.AddRange(GetComponents<NarrativeEffect>());
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

        public void TriggerEffects()
        {
            foreach (NarrativeEffect effect in effects)
            {
                if (effect.enabled)
                    effect.Trigger(gameObject);
            }
        }
    }

}