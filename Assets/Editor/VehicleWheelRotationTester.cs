using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;

public class VehicleWheelRotationTester : EditorWindow
{
    public VehicleWheelRotation target;
    private bool isRunning;
    private double lastTime;

    [MenuItem("Window/Vehicle Wheel Rotation Tester")]
    public static void OpenWindow() => GetWindow<VehicleWheelRotationTester>("Wheel Tester");

    void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
        lastTime = EditorApplication.timeSinceStartup;
    }

    void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
        isRunning = false;
    }

    void OnGUI()
    {
        EditorGUILayout.Space();
        target = (VehicleWheelRotation)EditorGUILayout.ObjectField("Target", target, typeof(VehicleWheelRotation), true);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(isRunning ? "Stop" : "Start"))
        {
            isRunning = !isRunning;
            lastTime = EditorApplication.timeSinceStartup;
            Repaint();
        }

        if (GUILayout.Button("Step"))
        {
            DoStep((float)(1f / 60f)); // simulate one 60 FPS frame
        }
        EditorGUILayout.EndHorizontal();

        if (target != null)
        {
            EditorGUILayout.LabelField("Current Speed", target.currentSpeed.ToString());
            if (GUILayout.Button("Reset Wheels to initial rotation"))
            {
                Undo.RecordObject(target, "Reset Wheels");
                for (int i = 0; i < target.transform.childCount; i++) { } // noop to keep pattern; initial rotations are not stored here
                EditorUtility.SetDirty(target);
                EditorSceneManager.MarkSceneDirty(target.gameObject.scene);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Assign a GameObject that has a VehicleWheelRotation component (drag from Hierarchy).", MessageType.Info);
        }
    }

    private void OnEditorUpdate()
    {
        if (!isRunning || target == null) return;

        double now = EditorApplication.timeSinceStartup;
        float delta = (float)(now - lastTime);
        lastTime = now;

        // perform tick
        DoStep(delta);
    }

    private void DoStep(float deltaTime)
    {
        if (target == null) return;

        // Make the changes undoable and mark dirty so they persist in scene
        Undo.RecordObject(target, "Wheel Rotation Tick");
        target.EditorTick(deltaTime);
        EditorUtility.SetDirty(target);
        // Mark modified transforms dirty as well so prefab changes persist
        foreach (var wheel in target.wheels)
        {
            if (wheel == null) continue;
            EditorUtility.SetDirty(wheel);
        }
        EditorSceneManager.MarkSceneDirty(target.gameObject.scene);
    }
}