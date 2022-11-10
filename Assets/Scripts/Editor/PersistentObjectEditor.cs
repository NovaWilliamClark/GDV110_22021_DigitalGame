/*******************************************************************************************
*
*    File: PersistentObjectEditor.cs
*    Purpose: Editor inspector for Persistent Object, sets unique Guid value (can be overwritten by user)
*    Author: Josh Taylor
*    Date: 09/11/2022
*
**********************************************************************************************/


using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


// https://stackoverflow.com/questions/65872685/unity-resets-parameter-value-set-with-editor-script-on-play
[CustomEditor(typeof(PersistentObject))]
public class PersistentObjectEditor : Editor
{
    private SerializedProperty m_uniqueId;

    private void OnEnable()
    {
        m_uniqueId = serializedObject.FindProperty("uniqueId");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (!PrefabStageUtility.GetCurrentPrefabStage())
        {
            if (m_uniqueId.stringValue == "" || String.IsNullOrWhiteSpace(m_uniqueId.stringValue) || String.IsNullOrEmpty(m_uniqueId.stringValue))
            {
                m_uniqueId.stringValue = Guid.NewGuid().ToString();
            }

            var otherObjs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            if (otherObjs.Length > 1)
            {
                foreach (var otherObj in otherObjs)
                {
                    var mycomp = target as PersistentObject;
                    var po = otherObj.GetComponent<PersistentObject>();
                    if (po && po == mycomp) continue;
                    if (po)
                    {
                        if (po.Id == m_uniqueId.stringValue)
                            m_uniqueId.stringValue = Guid.NewGuid().ToString();
                    }
                }
            }
            
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(m_uniqueId, new GUIContent("Unique ID"));
            EditorGUI.EndDisabledGroup();
            
            if (GUILayout.Button("Generate New GUID"))
            {
                m_uniqueId.stringValue = Guid.NewGuid().ToString();
            }
        }
        else
        {
            EditorGUILayout.LabelField("Currently in Prefab mode, Unique id cannot be set");
            if (m_uniqueId.stringValue != "")
            {
                Debug.Log("Id value on prefab original isn't blank");
                m_uniqueId.stringValue = "";
            }
        }
       
        serializedObject.ApplyModifiedProperties();
    }
}