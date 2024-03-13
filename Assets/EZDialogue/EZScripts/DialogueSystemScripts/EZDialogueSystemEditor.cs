using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EZDialogueSystem))]
public class EZDialogueSystemEditor : Editor
{
    // // Serialized properties for the variables you want to hide/reveal
    // SerializedProperty normalFont;
    // SerializedProperty hoverFont;
    // SerializedProperty UseBuiltInChoiceSystem;
    // // SerializedProperty property3;

    // // Boolean field to track the visibility state
    // bool revealVariables = false;

    // void OnEnable()
    // {
    //     // Initialize the serialized properties
    //     hoverFont = serializedObject.FindProperty("hoverFont");
    //     normalFont = serializedObject.FindProperty("normalFont");
    //     UseBuiltInChoiceSystem = serializedObject.FindProperty("UseBuiltInChoiceSystem");
    //     // property3 = serializedObject.FindProperty("variable3");
    // }

    // public override void OnInspectorGUI()
    // {
    //     // Draw the default inspector
    //     DrawDefaultInspector();

    //     // Update the serialized object
    //     serializedObject.Update();

    //     // Add a button to reveal/hide variables
    //     // revealVariables = EditorGUILayout.Toggle("Reveal Variables", revealVariables);

    //     // Conditionally display variables based on the button state
    //     if (UseBuiltInChoiceSystem.boolValue)
    //     {
    //         EditorGUILayout.PropertyField(hoverFont);
    //         EditorGUILayout.PropertyField(normalFont);
            
    //     }
        

    //     // Apply changes to the serialized object
    //     serializedObject.ApplyModifiedProperties();
    // }
}
