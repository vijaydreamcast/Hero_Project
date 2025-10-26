
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ServerManager))]
public class ServerManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector fields first
        DrawDefaultInspector();

        // Add a small UI section for broadcast
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Inspector Broadcast", EditorStyles.boldLabel);

        var server = (ServerManager)target;

        // Editable message field (keeps the value serialized on the component)
        server.inspectorBroadcastMessage = EditorGUILayout.TextField("Message", server.inspectorBroadcastMessage);

        // Button enabled only during Play mode and when server is running
        bool canBroadcastNow = Application.isPlaying && server.isRunning && server != null;

        using (new EditorGUI.DisabledScope(!canBroadcastNow))
        {
            if (GUILayout.Button("Broadcast Message"))
            {
                server.BroadcastInspectorMessage();
            }
        }

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter Play mode to broadcast (server runs in Play mode).", MessageType.Info);
        }
        else if (!server.isRunning)
        {
            EditorGUILayout.HelpBox("Server is not running. Ensure the component started the server.", MessageType.Warning);
        }
    }
}