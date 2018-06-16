using UnityEngine;
using System.Collections;
using UnityEditor;
using Completed;

[CustomEditor(typeof(EnvironmentObject))]
public class EnvironmentObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EnvironmentObject myObj = (EnvironmentObject)target;

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        if (GUILayout.Button("Move Up"))
        {
            myObj.Move(Vector2.up);
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        if (GUILayout.Button("Move Left"))
        {
            myObj.Move(Vector2.left);
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.EndVertical();
        if (GUILayout.Button("Move Right"))
        {
            myObj.Move(Vector2.right);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        if (GUILayout.Button("Move Down"))
        {
            myObj.Move(Vector2.down);
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

    }
}