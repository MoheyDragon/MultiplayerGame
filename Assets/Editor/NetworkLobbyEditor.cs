using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NetworkLobby))]
public class NetworkLobbyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        NetworkLobby lobbyScript = (NetworkLobby)target;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("isServer"));
        // Show server-specific variables if isServer is true
        if (lobbyScript.isServer)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("NetworkMessagesBridge"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("launchGameButton"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("waitingForPlayerToJoin"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ready"));
        }
        // Show client-specific variables if isServer is false
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("localClientManager"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("joinButton"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("connectionTries"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("connectionTimeOut"));
        }

        // Apply changes to the serialized object
        serializedObject.ApplyModifiedProperties();
    }
}
