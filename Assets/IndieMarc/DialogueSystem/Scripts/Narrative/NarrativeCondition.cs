using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using IndieMarc.TopDown;

namespace IndieMarc.DialogueSystem
{
    public enum NarrativeConditionType
    {
        None = 0,
        CustomInt = 1,
        CustomFloat = 2,
        CustomString = 3,

        IsVisible = 5,
        InsideRegion = 10,

        QuestStarted=20,
        QuestStartedActive=21,
        QuestCompleted=22,
        
        DialogueTriggered = 30,

        CustomFunction=99,
    }

    public enum NarrativeConditionOperator
    {
        Equal = 0,
        NotEqual = 1,
        GreaterEqual = 2,
        LessEqual = 3,
        Greater = 4,
        Less = 5,
    }

    public enum NarrativeConditionOperator2
    {
        IsTrue = 0,
        IsFalse = 1,
    }

    public enum NarrativeConditionTargetType
    {
        Value = 0,
        Target = 1,
    }
    
    public interface CustomContidion
    {
        bool IsMet();
    }

    public class NarrativeCondition : MonoBehaviour
    {
        public NarrativeConditionType type;
        public NarrativeConditionOperator oper;
        public NarrativeConditionOperator2 oper2;
        public NarrativeConditionTargetType target_type;
        public NarrativeTarget target = new NarrativeTarget();
        public string target_id = "";
        public string other_target_id;
        public GameObject value_object;
        public int value_int = 0;
        public float value_float = 0f;
        public string value_string = "";
        
        public GameObject custom_condition;

        public bool IsMet(GameObject triggerer)
        {
            bool condition_met = false;

            if (type == NarrativeConditionType.None)
            {
                condition_met = true;
            }

            if (type == NarrativeConditionType.CustomInt)
            {
                int i1 = NarrativeData.Get().GetCustomInt(target_id);
                int i2 = target_type == NarrativeConditionTargetType.Target ? NarrativeData.Get().GetCustomInt(other_target_id) : value_int;
                condition_met = CompareInt(i1, i2);
            }

            if (type == NarrativeConditionType.CustomFloat)
            {
                float f1 = NarrativeData.Get().GetCustomFloat(target_id);
                float f2 = target_type == NarrativeConditionTargetType.Target ? NarrativeData.Get().GetCustomFloat(other_target_id) : value_float;
                condition_met = CompareFloat(f1, f2);
            }

            if (type == NarrativeConditionType.CustomString)
            {
                string s1 = NarrativeData.Get().GetCustomString(target_id);
                string s2 = target_type == NarrativeConditionTargetType.Target ? NarrativeData.Get().GetCustomString(other_target_id) : value_string;
                condition_met = CompareString(s1, s2);
            }

            if (type == NarrativeConditionType.IsVisible)
            {
                GameObject targ = target.GetTargetObject(triggerer);
                condition_met = (targ != null && targ.activeSelf);
                if (targ != null)
                {
                    condition_met = targ.activeSelf;
                }
                if (oper2 == NarrativeConditionOperator2.IsFalse)
                {
                    condition_met = !condition_met;
                }
            }
            
            if (type == NarrativeConditionType.InsideRegion)
            {
                GameObject targ = target.GetTargetObject(triggerer);
                if (targ && value_object)
                {
                    if (value_object.GetComponent<Region>())
                        condition_met = value_object.GetComponent<Region>().IsInside(targ);
                }
                if (oper2 == NarrativeConditionOperator2.IsFalse)
                {
                    condition_met = !condition_met;
                }
            }
            
            if (type == NarrativeConditionType.QuestStarted)
            {
                if (value_object && value_object.GetComponent<NarrativeQuest>()){
                    NarrativeQuest quest = value_object.GetComponent<NarrativeQuest>();
                    condition_met = NarrativeData.Get().IsQuestStarted(quest.quest_id);
                    if (oper2 == NarrativeConditionOperator2.IsFalse)
                        condition_met = !condition_met;
                }
            }

            if (type == NarrativeConditionType.QuestStartedActive)
            {
                if (value_object && value_object.GetComponent<NarrativeQuest>())
                {
                    NarrativeQuest quest = value_object.GetComponent<NarrativeQuest>();
                    condition_met = NarrativeData.Get().IsQuestActive(quest.quest_id);
                    if (oper2 == NarrativeConditionOperator2.IsFalse)
                        condition_met = !condition_met;
                }
            }

            if (type == NarrativeConditionType.QuestCompleted)
            {
                if (value_object && value_object.GetComponent<NarrativeQuest>())
                {
                    NarrativeQuest quest = value_object.GetComponent<NarrativeQuest>();
                    condition_met = NarrativeData.Get().IsQuestCompleted(quest.quest_id);
                    if (oper2 == NarrativeConditionOperator2.IsFalse)
                        condition_met = !condition_met;
                }
            }

            if (type == NarrativeConditionType.DialogueTriggered)
            {
                GameObject targ = target.GetTargetObject();
                if (targ && targ.GetComponent<DialogueEvent>())
                {
                    DialogueEvent cinema = targ.GetComponent<DialogueEvent>();
                    condition_met = cinema.GetTriggerCount() >= 1;
                    if (oper2 == NarrativeConditionOperator2.IsFalse)
                        condition_met = !condition_met;
                }
            }

            if (type == NarrativeConditionType.CustomFunction)
            {
                if (custom_condition != null)
                {
                    if(custom_condition.GetComponent<CustomContidion>() != null)
                        condition_met = custom_condition.GetComponent<CustomContidion>().IsMet();
                    if (oper2 == NarrativeConditionOperator2.IsFalse)
                        condition_met = !condition_met;
                }
            }

            return condition_met;
        }

        public bool CompareInt(int ival1, int ival2)
        {
            bool condition_met = true;
            if (oper == NarrativeConditionOperator.Equal && ival1 != ival2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.NotEqual && ival1 == ival2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.GreaterEqual && ival1 < ival2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.LessEqual && ival1 > ival2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Greater && ival1 <= ival2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Less && ival1 >= ival2)
            {
                condition_met = false;
            }
            return condition_met;
        }

        public bool CompareFloat(float fval1, float fval2)
        {
            bool condition_met = true;
            if (oper == NarrativeConditionOperator.Equal && fval1 != fval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.NotEqual && fval1 == fval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.GreaterEqual && fval1 < fval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.LessEqual && fval1 > fval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Greater && fval1 <= fval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Less && fval1 >= fval2)
            {
                condition_met = false;
            }
            return condition_met;
        }

        public bool CompareString(string sval1, string sval2)
        {
            bool condition_met = true;
            if (oper == NarrativeConditionOperator.Equal && sval1 != sval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.NotEqual && sval1 == sval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.GreaterEqual && sval1 != sval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.LessEqual && sval1 != sval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Greater && sval1 == sval2)
            {
                condition_met = false;
            }
            if (oper == NarrativeConditionOperator.Less && sval1 == sval2)
            {
                condition_met = false;
            }
            return condition_met;
        }
    }

}
