using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.DialogueSystem
{

    public class MadNarrativeGroup : MonoBehaviour
    {
        void Awake()
        {
            foreach (NarrativeEvent evt in GetComponentsInChildren<NarrativeEvent>())
            {
                evt.AddConditions(GetComponents<NarrativeCondition>());
            }
        }
    }

}
