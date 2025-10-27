using UnityEngine;
using UnityEditor;

public class RemoveLOD0 : EditorWindow
{
    [MenuItem("Tools/LOD/Remove LOD0 From All LODGroups")]
    public static void RemoveLOD0FromAll()
    {
        int count = 0;
        LODGroup[] lodGroups = GameObject.FindObjectsOfType<LODGroup>();

        foreach (LODGroup group in lodGroups)
        {
            LOD[] lods = group.GetLODs();

            if (lods.Length > 1)
            {
                // Create a new array without LOD0
                LOD[] newLods = new LOD[lods.Length - 1];
                for (int i = 1; i < lods.Length; i++)
                    newLods[i - 1] = lods[i];

                group.SetLODs(newLods);
                group.RecalculateBounds();
                count++;
            }
        }

        Debug.Log($"Removed LOD0 from {count} LODGroups in the scene.");
    }
}
