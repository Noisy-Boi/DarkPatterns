using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace IndieMarc.DialogueSystem
{

    [CustomEditor(typeof(NarrativeEffect))]
    public class NarrativeEffectEditor : Editor
    {
        SerializedProperty sprop;

        internal void OnEnable()
        {
            sprop = serializedObject.FindProperty("callfunc_evt");
        }

        public override void OnInspectorGUI()
        {
            NarrativeEffect myScript = target as NarrativeEffect;

            myScript.type = (NarrativeEffectType)AddEnumField("Type", myScript.type);

            if (myScript.type == NarrativeEffectType.CustomInt
                || myScript.type == NarrativeEffectType.CustomFloat
                || myScript.type == NarrativeEffectType.CustomString)
            {
                myScript.target_id = AddTextField("Target ID", myScript.target_id);

                myScript.oper = (NarrativeEffectOperator)AddEnumField("Operator", myScript.oper);

                if (myScript.type == NarrativeEffectType.CustomInt)
                {
                    myScript.value_int = AddIntField("Value Int", myScript.value_int);
                }

                if (myScript.type == NarrativeEffectType.CustomFloat)
                {
                    myScript.value_float = AddFloatField("Value Float", myScript.value_float);
                }

                if (myScript.type == NarrativeEffectType.CustomString)
                {
                    myScript.value_string = AddTextField("Value String", myScript.value_string);
                }

            }

            if (myScript.type == NarrativeEffectType.Move)
            {
                GenerateTargetGUI(myScript.target);
                myScript.value_object = (GameObject)AddGameObjectField("Target Location", myScript.value_object);
            }

            if (myScript.type == NarrativeEffectType.Show || myScript.type == NarrativeEffectType.Hide
                || myScript.type == NarrativeEffectType.Spawn || myScript.type == NarrativeEffectType.Destroy
                || myScript.type == NarrativeEffectType.StartDialogue
                || myScript.type == NarrativeEffectType.RunEvent || myScript.type == NarrativeEffectType.RunEventIfMet)
            {
                GenerateTargetGUI(myScript.target);
            }
            
            if (myScript.type == NarrativeEffectType.LockCamera)
            {
                myScript.value_object = AddGameObjectField("Target", myScript.value_object);
            }

            if (myScript.type == NarrativeEffectType.LockCamera || myScript.type == NarrativeEffectType.UnlockCamera)
            {
                myScript.value_float = AddFloatField("Zoom", myScript.value_float);
            }
            
            if (myScript.type == NarrativeEffectType.StartQuest || myScript.type == NarrativeEffectType.CancelQuest
                || myScript.type == NarrativeEffectType.CompleteQuest)
            {
                myScript.value_object = AddGameObjectField("Quest", myScript.value_object);
            }
            
            if (myScript.type == NarrativeEffectType.Wait)
            {
                myScript.value_float = AddFloatField("Time", myScript.value_float);
            }

            if (myScript.type == NarrativeEffectType.CallFunction)
            {

                //EditorGUIUtility.LookLikeControls();
                GUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.PropertyField(sprop, new GUIContent("List Callbacks", ""));
                }

                if (GUI.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                }
                GUILayout.EndHorizontal();
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