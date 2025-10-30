using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


public class CharacterExtractor : EditorWindow
{
    private DefaultAsset folderAsset;
    private string folderPath;
    private bool includeSubfolders = true;
    private bool replacePrefabMaterials = false;
    private string materialSuffix = "_mat";
    private int processedCount;

    [MenuItem("Tools/Character Extractor")]
    public static void OpenWindow() => GetWindow<CharacterExtractor>("Character Extractor");

    private void OnEnable()
    {
        // Prefill folder if a folder is selected in Project view
        if (Selection.activeObject != null && AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(Selection.activeObject)))
        {
            folderAsset = Selection.activeObject as DefaultAsset;
            folderPath = AssetDatabase.GetAssetPath(folderAsset);
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Extract Materials from FBX / Model Assets", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        folderAsset = (DefaultAsset)EditorGUILayout.ObjectField("Folder (Project)", folderAsset, typeof(DefaultAsset), false);
        if (GUILayout.Button("Select", GUILayout.Width(60)))
        {
            string p = EditorUtility.OpenFolderPanel("Select Character Folder (Project)", "Assets", "");
            if (!string.IsNullOrEmpty(p) && p.StartsWith(Application.dataPath))
            {
                folderPath = "Assets" + p.Substring(Application.dataPath.Length);
                folderAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(folderPath);
            }
            else if (!string.IsNullOrEmpty(p))
            {
                EditorUtility.DisplayDialog("Invalid Folder", "Please choose a folder inside the project's Assets folder.", "OK");
            }
        }
        EditorGUILayout.EndHorizontal();

        if (folderAsset != null)
            folderPath = AssetDatabase.GetAssetPath(folderAsset);

        EditorGUILayout.LabelField("Folder Path", folderPath);

        includeSubfolders = EditorGUILayout.Toggle("Include Subfolders", includeSubfolders);
        replacePrefabMaterials = EditorGUILayout.Toggle("Replace Model Materials", replacePrefabMaterials);
        materialSuffix = EditorGUILayout.TextField("Material Suffix", materialSuffix);

        EditorGUILayout.Space();

        if (GUILayout.Button("Extract Materials from FBX"))
        {
            if (string.IsNullOrEmpty(folderPath) || !AssetDatabase.IsValidFolder(folderPath))
            {
                EditorUtility.DisplayDialog("Folder required", "Please assign a valid folder inside Assets.", "OK");
            }
            else
            {
                ExtractMaterialsFromFBX(folderPath, includeSubfolders, replacePrefabMaterials);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Processed Assets", processedCount.ToString());
    }

    private void ExtractMaterialsFromFBX(string folder, bool recurse, bool replace)
    {
        processedCount = 0;
        Debug.Log($"[CharacterExtractor] Starting extraction in: {folder} (recurse={recurse}, replace={replace})");

        // Convert asset folder to absolute filesystem path
        string absFolder = folder.StartsWith("Assets") ? folder.Replace("Assets", Application.dataPath) : Path.GetFullPath(folder);

        SearchOption searchOpt = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        string[] fbxFiles;
        try
        {
            fbxFiles = Directory.GetFiles(absFolder, "*.fbx", searchOpt);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[CharacterExtractor] Failed to enumerate FBX files: {ex.Message}");
            EditorUtility.DisplayDialog("Error", "Failed to enumerate FBX files. See Console for details.", "OK");
            return;
        }

        Debug.Log($"[CharacterExtractor] Found {fbxFiles.Length} .fbx files under {absFolder}.");

        if (fbxFiles.Length == 0)
        {
            EditorUtility.DisplayDialog("No FBX found", $"No .fbx files were found in folder: {folder}", "OK");
            return;
        }

        var createdMaterials = new Dictionary<string, string>(); // key -> created material asset path

        try
        {
            int total = fbxFiles.Length;
            for (int i = 0; i < total; i++)
            {
                string file = fbxFiles[i];
                // convert absolute path back to Asset path
                if (!file.StartsWith(Application.dataPath))
                    continue;
                string assetPath = "Assets" + file.Substring(Application.dataPath.Length).Replace('\\', '/');

                if (!recurse)
                {
                    string dir = Path.GetDirectoryName(assetPath).Replace('\\', '/').TrimEnd('/');
                    if (dir != folder.TrimEnd('/')) continue;
                }

                EditorUtility.DisplayProgressBar("Extracting FBX materials", Path.GetFileName(assetPath), (float)i / Mathf.Max(1, total));
                Debug.Log($"[CharacterExtractor] Processing FBX: {assetPath}");

                GameObject modelRoot = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                if (modelRoot == null)
                {
                    Debug.LogWarning($"[CharacterExtractor] Could not load model at {assetPath}");
                    continue;
                }

                // get all renderers (MeshRenderer, SkinnedMeshRenderer, etc.)
                var renderers = modelRoot.GetComponentsInChildren<Renderer>(true);
                if (renderers == null || renderers.Length == 0)
                {
                    processedCount++;
                    continue;
                }

                string prefabFolder = Path.GetDirectoryName(assetPath).Replace('\\', '/');

                foreach (var rend in renderers)
                {
                    if (rend == null) continue;
                    var mats = rend.sharedMaterials;
                    bool changed = false;

                    for (int m = 0; m < mats.Length; m++)
                    {
                        var mat = mats[m];
                        if (mat == null) continue;

                        // create a stable key for duplicate prevention
                        string matAssetPath = AssetDatabase.GetAssetPath(mat);
                        string key = string.IsNullOrEmpty(matAssetPath) ? mat.name + "_" + mat.GetInstanceID() : matAssetPath;

                        if (!createdMaterials.TryGetValue(key, out string createdPath))
                        {
                            // ensure unique name and place in same folder as the FBX
                            string baseName = Path.GetFileNameWithoutExtension(assetPath) + "_" + (rend.gameObject.name.Replace(" ", "_")) + "_" + m + materialSuffix + ".mat";
                            string candidatePath = Path.Combine(prefabFolder, baseName).Replace('\\', '/');
                            candidatePath = AssetDatabase.GenerateUniqueAssetPath(candidatePath);

                            Material newMat = new Material(mat);
                            AssetDatabase.CreateAsset(newMat, candidatePath);
                            createdPath = candidatePath;
                            createdMaterials[key] = createdPath;
                            Debug.Log($"[CharacterExtractor] Created material: {createdPath} (from {mat.name})");
                        }

                        if (replace)
                        {
                            Material imported = AssetDatabase.LoadAssetAtPath<Material>(createdPath);
                            if (imported != null)
                            {
                                mats[m] = imported;
                                changed = true;
                            }
                        }
                    }

                    if (replace && changed)
                    {
                        // Attempt to assign changed sharedMaterials back to the model asset instance.
                        // For model assets this may affect only the asset instance; it's the best-effort approach.
                        rend.sharedMaterials = mats;
                        EditorUtility.SetDirty(rend);
                    }
                }

                // Save changes if any replacements were made
                if (replace)
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                processedCount++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[CharacterExtractor] Exception: {ex}");
            EditorUtility.DisplayDialog("Error", $"Extraction failed: {ex.Message}", "OK");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        Debug.Log($"[CharacterExtractor] Done. Processed {processedCount} FBX files. Materials created: {createdMaterials.Count}");
        EditorUtility.DisplayDialog("Done", $"Processed {processedCount} FBX file(s). Materials created: {createdMaterials.Count}", "OK");
    }
}