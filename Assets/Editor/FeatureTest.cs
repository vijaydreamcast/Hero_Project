using UnityEngine;
using UnityEditor;

public class FeatureTest : EditorWindow
{
    private SimpleBikeController bikeController;
    private GameObject bikeObject;
    private Transform bikeStartTransform;
    private GameDataSO gameData;

    [MenuItem("Tools/Bike Positioner")]
    public static void ShowWindow()
    {
        GetWindow<FeatureTest>("Bike Positioner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Bike Positioner", EditorStyles.boldLabel);

        bikeController = (SimpleBikeController)EditorGUILayout.ObjectField("SimpleBikeController", bikeController, typeof(SimpleBikeController), true);
        bikeObject = (GameObject)EditorGUILayout.ObjectField("Bike GameObject", bikeObject, typeof(GameObject), true);
        bikeStartTransform = (Transform)EditorGUILayout.ObjectField("Bike Start Transform", bikeStartTransform, typeof(Transform), true);

        GUILayout.Space(6);

        if (GUILayout.Button("Move Bike To Start Position"))
        {
            MoveBikeToStart();
        }

        GUILayout.Space(8);
        GUILayout.Label("Game Data", EditorStyles.boldLabel);
        gameData = (GameDataSO)EditorGUILayout.ObjectField("GameDataSO", gameData, typeof(GameDataSO), false);

        if (GUILayout.Button("Reset"))
        {
            ResetGameData();
        }
    }

    private void MoveBikeToStart()
    {
        if (bikeObject != null && bikeStartTransform != null)
        {
            Undo.RecordObject(bikeObject.transform, "Move Bike To Start Position");
            bikeObject.transform.position = bikeStartTransform.position;
            bikeObject.transform.rotation = bikeStartTransform.rotation;
            EditorUtility.SetDirty(bikeObject);
        }
        else
        {
            Debug.LogWarning("Please assign both the Bike GameObject and Bike Start Transform.");
        }
    }

    private void ResetGameData()
    {
        if (gameData != null)
        {
            // Call the SO method that triggers the restart event
            gameData.RestartGame();
            EditorUtility.SetDirty(gameData);
            Debug.Log("GameDataSO.RestartGame() called from Bike Positioner window.");
        }
        else
        {
            Debug.LogWarning("Please assign a GameDataSO asset before pressing Reset.");
        }
    }
}
