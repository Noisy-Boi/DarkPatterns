using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace IndieMarc.DialogueSystem
{

    [CustomEditor(typeof(NarrativeCondition))]
    public class NarrativeConditionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            NarrativeCondition myScript = target as NarrativeCondition;

            bool value_compare = false;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Type", GetLabelWidth());
            GUILayout.FlexibleSpace();
            myScript.type = (NarrativeConditionType)EditorGUILayout.EnumPopup(myScript.type, GetWidth());
            GUILayout.EndHorizontal();

            if (myScript.type == NarrativeConditionType.CustomInt
                || myScript.type == NarrativeConditionType.CustomFloat
                || myScript.type == NarrativeConditionType.CustomString)
            {

                myScript.target_id = AddTextField("Target ID", myScript.target_id);
                myScript.oper = (NarrativeConditionOperator)AddEnumField("Operator", myScript.oper);
                myScript.target_type = (NarrativeConditionTargetType)AddEnumField("Other Type", myScript.target_type);
                value_compare = true;

                if (myScript.target_type == NarrativeConditionTargetType.Target)
                {
                    myScript.other_target_id = AddTextField("Other Target ID", myScript.other_target_id);
                }

                if (myScript.target_type == NarrativeConditionTargetType.Value)
                {
                    if (myScript.type == NarrativeConditionType.CustomInt)
                    {
                        myScript.value_int = AddIntField("Int Value", myScript.value_int);
                    }

                    if (myScript.type == NarrativeConditionType.CustomFloat)
                    {
                        myScript.value_float = AddFloatField("Float Value", myScript.value_float);
                    }

                    if (myScript.type == NarrativeConditionType.CustomString)
                    {
                        myScript.value_string = AddTextField("String Value", myScript.value_string);
                    }
                }

            }

            //Special cases
            if (myScript.type == NarrativeConditionType.IsVisible)
            {
                GenerateTargetGUI(myScript.target);
            }
            
            if (myScript.type == NarrativeConditionType.InsideRegion)
            {

                GenerateTargetGUI(myScript.target);
                myScript.value_object = AddGameObjectField("Region", myScript.value_object);
            }

            if (myScript.type == NarrativeConditionType.DialogueTriggered)
            {

                GenerateTargetGUI(myScript.target);
            }

            if (myScript.type == NarrativeConditionType.QuestStartedActive
                || myScript.type == NarrativeConditionType.QuestStarted
                || myScript.type == NarrativeConditionType.QuestCompleted)
            {
                myScript.value_object = AddGameObjectField("Quest", myScript.value_object);
            }
            
            if (myScript.type == NarrativeConditionType.CustomFunction)
            {
                EditorGUILayout.LabelField("Custom Function", EditorStyles.boldLabel);
                GUILayout.Label("Create a new script that inherit MonoBehavior");
                GUILayout.Label("and IndieMarc.DialogueSystem.CustomCondition interface, ex:");
                GUILayout.Label("public class MyCond : MonoBehavior, CustomCondition");
                GUILayout.Label("then attach to a gameObject and reference it here.");
                myScript.custom_condition = AddGameObjectField("Custom Condition", myScript.custom_condition);
            }

            if (!value_compare)
            {
                myScript.oper2 = (NarrativeConditionOperator2)AddEnumField("Operator", myScript.oper2);
            }
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(myScript);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                AssetDatabase.SaveAssets();
            }
        }

        private void GenerateTargetGUI(NarrativeTarget myScript)
        {
            if (myScript != null)
            {
                myScript.type = (MadNarrativeTargetType)AddEnumField("Target Type", myScript.type);

                if (myScript.type == MadNarrativeTargetType.GameObject)
                {
                    myScript.game_object = AddGameObjectField("Target", myScript.game_object);
                }

                if (myScript.type == MadNarrativeTargetType.Texture)
                {
                    myScript.texture = AddSpriteField("Target", myScript.texture);
                }

                if (myScript.type == MadNarrativeTargetType.Sound)
                {
                    myScript.sound = AddAudioField("Target", myScript.sound);
                }
            }
        }

        private string AddTextField(string label, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetLabelWidth());
            GUILayout.FlexibleSpace();
            string outval = EditorGUILayout.TextField(value, GetWidth());
            GUILayout.EndHorizontal();
            return outval;
        }

        private string AddTextAreaField(string label, string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetShortLabelWidth());
            GUILayout.FlexibleSpace();
            EditorStyles.textField.wordWrap = true;
            string outval = EditorGUILayout.TextArea(value, GetLongWidth(), GUILayout.Height(50));
            GUILayout.EndHorizontal();
            return outval;
        }

        private int AddIntField(string label, int value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetLabelWidth());
            GUILayout.FlexibleSpace();
            int outval = EditorGUILayout.IntField(value, GetWidth());
            GUILayout.EndHorizontal();
            return outval;
        }

        private float AddFloatField(string label, float value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetLabelWidth());
            GUILayout.FlexibleSpace();
            float outval = EditorGUILayout.FloatField(value, GetWidth());
            GUILayout.EndHorizontal();
            return outval;
        }

        private System.Enum AddEnumField(string label, System.Enum value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetLabelWidth());
            GUILayout.FlexibleSpace();
            System.Enum outval = EditorGUILayout.EnumPopup(value, GetWidth());
            GUILayout.EndHorizontal();
            return outval;
        }

        private GameObject AddGameObjectField(string label, GameObject value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetLabelWidth());
            GUILayout.FlexibleSpace();
            GameObject outval = (GameObject)EditorGUILayout.ObjectField(value, typeof(GameObject), true, GetWidth());
            GUILayout.EndHorizontal();
            return outval;
        }

        private ScriptableObject AddScriptableObjectField(string label, ScriptableObject value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetLabelWidth());
            GUILayout.FlexibleSpace();
            ScriptableObject outval = (ScriptableObject)EditorGUILayout.ObjectField(value, typeof(ScriptableObject), true, GetWidth());
            GUILayout.EndHorizontal();
            return outval;
        }

        private Sprite AddSpriteField(string label, Sprite value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetLabelWidth());
            GUILayout.FlexibleSpace();
            Sprite outval = (Sprite)EditorGUILayout.ObjectField(value, typeof(Sprite), true, GetWidth());
            GUILayout.EndHorizontal();
            return outval;
        }

        private AudioClip AddAudioField(string label, AudioClip value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GetLabelWidth());
            GUILayout.FlexibleSpace();
            AudioClip outval = (AudioClip)EditorGUILayout.ObjectField(value, typeof(AudioClip), true, GetWidth());
            GUILayout.EndHorizontal();
            return outval;
        }

        private GUILayoutOption GetLabelWidth()
        {
            return GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.4f);
        }

        private GUILayoutOption GetWidth()
        {
            return GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.45f);
        }

        private GUILayoutOption GetShortLabelWidth()
        {
            return GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.25f);
        }

        private GUILayoutOption GetLongWidth()
        {
            return GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.65f);
        }
    }

}