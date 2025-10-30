using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor utility to create tidy subfolders and organise model/prefab/material/texture/animation assets
/// inside a selected project folder.
/// - Creates: FBX, Prefabs, Materials, Textures, Animations, Other
/// - Moves assets that live under the selected folder into the appropriate subfolder
/// - Creates prefabs for FBX models (best-effort)
/// - Extracts AnimationClips embedded in FBX into Animations/
/// Note: This tool moves assets (uses AssetDatabase.MoveAsset). Commit/backup before running.
/// </summary>
public class FolderOrganiser : EditorWindow
{
    private DefaultAsset folderAsset;
    private string folderPath = "";
    private bool includeSubfolders = true;
    private bool createPrefabs = true;
    private bool extractAnimations = true;
    private bool dryRun = false;
    private int processedCount;

    [MenuItem("Tools/Folder Organiser")]
    public static void OpenWindow() => GetWindow<FolderOrganiser>("Folder Organiser");

    private void OnEnable()
    {
        if (Selection.activeObject != null && AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(Selection.activeObject)))
        {
            folderAsset = Selection.activeObject as DefaultAsset;
            folderPath = AssetDatabase.GetAssetPath(folderAsset);
        }
    }

    private void OnGUI()
    {
        GUILayout.Label("Folder Organiser", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Select a project folder and organise its assets into subfolders: FBX, Prefabs, Materials, Textures, Animations, Other.\nMake a backup or commit before running.", MessageType.Info);

        EditorGUILayout.BeginHorizontal();
        folderAsset = (DefaultAsset)EditorGUILayout.ObjectField("Folder (Project)", folderAsset, typeof(DefaultAsset), false);
        if (GUILayout.Button("Select...", GUILayout.Width(80)))
        {
            string p = EditorUtility.OpenFolderPanel("Select Folder (Project)", "Assets", "");
            if (!string.IsNullOrEmpty(p) && p.StartsWith(Application.dataPath))
            {
                folderPath = "Assets" + p.Substring(Application.dataPath.Length);
                folderAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(folderPath);
            }
            else if (!string.IsNullOrEmpty(p))
            {
                EditorUtility.DisplayDialog("Invalid Folder", "Choose a folder inside the project's Assets folder.", "OK");
            }
        }
        EditorGUILayout.EndHorizontal();

        if (folderAsset != null)
            folderPath = AssetDatabase.GetAssetPath(folderAsset);

        EditorGUILayout.LabelField("Folder Path", folderPath);

        includeSubfolders = EditorGUILayout.Toggle("Include Subfolders", includeSubfolders);
        createPrefabs = EditorGUILayout.Toggle("Create Prefabs from FBX", createPrefabs);
        extractAnimations = EditorGUILayout.Toggle("Extract Animations from FBX", extractAnimations);
        dryRun = EditorGUILayout.Toggle("Dry Run (no moves)", dryRun);

        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(folderPath) || !AssetDatabase.IsValidFolder(folderPath)))
        {
            if (GUILayout.Button("Organise Folder"))
            {
                if (EditorUtility.DisplayDialog("Confirm", $"Organise assets under {folderPath}?\nThis will move assets in the project. Make a backup or commit before continuing.", "Yes", "Cancel"))
                {
                    Organise(folderPath, includeSubfolders, createPrefabs, extractAnimations, dryRun);
                }
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Processed Assets", processedCount.ToString());
    }

    private void Organise(string folder, bool recurse, bool createPrefabsFromModels, bool extractAnims, bool dryRunMode)
    {
        processedCount = 0;
        Debug.Log($"[FolderOrganiser] Start organising: {folder} (recurse={recurse}, prefabs={createPrefabsFromModels}, extractAnims={extractAnims}, dryRun={dryRunMode})");

        // prepare subfolders
        string fbxFolder = EnsureSubfolder(folder, "FBX");
        string prefabsFolder = EnsureSubfolder(folder, "Prefabs");
        string materialsFolder = EnsureSubfolder(folder, "Materials");
        string texturesFolder = EnsureSubfolder(folder, "Textures");
        string animationsFolder = EnsureSubfolder(folder, "Animations");
        string otherFolder = EnsureSubfolder(folder, "Other");

        // gather assets inside the folder (AssetDatabase find is better cross-platform)
        string[] filters = new[] { "t:Model", "t:Prefab", "t:Material", "t:Texture", "t:AnimationClip", "t:GameObject" };
        HashSet<string> candidateAssetPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var filter in filters)
        {
            string[] guids = AssetDatabase.FindAssets(filter, new[] { folder });
            foreach (var g in guids)
            {
                string ap = AssetDatabase.GUIDToAssetPath(g);
                // optionally skip nested subfolders if not recursing
                if (!recurse)
                {
                    var dir = Path.GetDirectoryName(ap).Replace('\\', '/').TrimEnd('/');
                    if (dir != folder.TrimEnd('/')) continue;
                }
                candidateAssetPaths.Add(ap);
            }
        }

        // Also include other asset files by scanning folder for common extensions
        string absFolder = folder.StartsWith("Assets") ? folder.Replace("Assets", Application.dataPath) : Path.GetFullPath(folder);
        SearchOption opt = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        var extraFiles = Directory.GetFiles(absFolder, "*.*", opt)
            .Where(p => {
                string ext = Path.GetExtension(p).ToLowerInvariant();
                return ext == ".fbx" || ext == ".prefab" || ext == ".mat" || ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".tga" || ext == ".psd" || ext == ".exr" || ext == ".anim";
            });

        foreach (var ef in extraFiles)
        {
            if (!ef.StartsWith(Application.dataPath)) continue;
            string assetPath = "Assets" + ef.Substring(Application.dataPath.Length).Replace('\\', '/');
            candidateAssetPaths.Add(assetPath);
        }

        var createdClips = new Dictionary<string, string>();
        var movedTargets = new HashSet<string>();

        try
        {
            var items = candidateAssetPaths.ToList();
            int total = items.Count;
            for (int i = 0; i < total; i++)
            {
                string ap = items[i];
                EditorUtility.DisplayProgressBar("Organising", Path.GetFileName(ap), (float)i / Math.Max(1, total));

                // skip files already inside the target subfolders to avoid moving again
                if (IsUnderSubfolder(ap, fbxFolder) || IsUnderSubfolder(ap, prefabsFolder) || IsUnderSubfolder(ap, materialsFolder) || IsUnderSubfolder(ap, texturesFolder) || IsUnderSubfolder(ap, animationsFolder) || IsUnderSubfolder(ap, otherFolder))
                    continue;

                string ext = Path.GetExtension(ap).ToLowerInvariant();
                try
                {
                    if (ext == ".fbx")
                    {
                        // Move FBX to FBX/
                        string dest = GenerateUniqueInFolder(fbxFolder, Path.GetFileName(ap));
                        if (!dryRunMode && ap != dest)
                        {
                            string mv = AssetDatabase.MoveAsset(ap, dest);
                            if (!string.IsNullOrEmpty(mv)) Debug.LogWarning($"[FolderOrganiser] MoveAsset returned: {mv} for {ap} -> {dest}");
                            else { ap = dest; Debug.Log($"[FolderOrganiser] Moved FBX: {dest}"); }
                        }
                        else ap = dest;

                        // create prefab from model (optional)
                        if (createPrefabsFromModels)
                        {
                            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(ap);
                            if (model != null)
                            {
                                string prefabName = MakeSafeFileName(Path.GetFileNameWithoutExtension(ap)) + ".prefab";
                                string prefabPath = GenerateUniqueInFolder(prefabsFolder, prefabName);
                                if (!dryRunMode)
                                {
                                    GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(model);
                                    if (inst == null) inst = GameObject.Instantiate(model);
                                    PrefabUtility.SaveAsPrefabAssetAndConnect(inst, prefabPath, InteractionMode.AutomatedAction);
                                    GameObject.DestroyImmediate(inst);
                                    Debug.Log($"[FolderOrganiser] Created prefab: {prefabPath}");
                                }
                            }
                        }

                        // extract animation clips
                        if (extractAnims)
                        {
                            System.Object[] assets = AssetDatabase.LoadAllAssetsAtPath(ap);
                            if (assets != null && assets.Length > 0)
                            {
                                foreach (var a in assets)
                                {
                                    if (a is AnimationClip clip)
                                    {
                                        if (clip.name.ToLower().Contains("preview")) continue;
                                        string key = $"{ap}:{clip.name}";
                                        if (!createdClips.ContainsKey(key))
                                        {
                                            string safeName = MakeSafeFileName(Path.GetFileNameWithoutExtension(ap)) + "_" + MakeSafeFileName(clip.name) + ".anim";
                                            string candidate = GenerateUniqueInFolder(animationsFolder, safeName);
                                            if (!dryRunMode)
                                            {
                                                AnimationClip newClip = UnityEngine.Object.Instantiate(clip);
                                                newClip.name = Path.GetFileNameWithoutExtension(candidate);
                                                AssetDatabase.CreateAsset(newClip, candidate);
                                            }
                                            createdClips[key] = candidate;
                                            Debug.Log($"[FolderOrganiser] Extracted clip: {candidate}");
                                        }
                                    }
                                }
                            }
                        }

                        // move dependencies (materials/textures) that live under base folder
                        string[] deps = AssetDatabase.GetDependencies(ap, true);
                        foreach (var d in deps)
                        {
                            if (d == ap) continue;
                            if (!d.StartsWith(folder)) continue; // only move assets inside selected folder
                            string dex = Path.GetExtension(d).ToLowerInvariant();
                            if (dex == ".mat")
                            {
                                string destMat = GenerateUniqueInFolder(materialsFolder, Path.GetFileName(d));
                                if (!movedTargets.Contains(destMat))
                                {
                                    if (!dryRunMode)
                                    {
                                        string result = AssetDatabase.MoveAsset(d, destMat);
                                        if (!string.IsNullOrEmpty(result)) Debug.LogWarning($"[FolderOrganiser] MoveAsset result: {result}");
                                    }
                                    movedTargets.Add(destMat);
                                    Debug.Log($"[FolderOrganiser] Material -> {destMat}");
                                }
                            }
                            else if (dex == ".png" || dex == ".jpg" || dex == ".jpeg" || dex == ".tga" || dex == ".psd" || dex == ".exr")
                            {
                                string destTex = GenerateUniqueInFolder(texturesFolder, Path.GetFileName(d));
                                if (!movedTargets.Contains(destTex))
                                {
                                    if (!dryRunMode)
                                    {
                                        string result = AssetDatabase.MoveAsset(d, destTex);
                                        if (!string.IsNullOrEmpty(result)) Debug.LogWarning($"[FolderOrganiser] MoveAsset result: {result}");
                                    }
                                    movedTargets.Add(destTex);
                                    Debug.Log($"[FolderOrganiser] Texture -> {destTex}");
                                }
                            }
                        }
                    }
                    else if (ext == ".prefab")
                    {
                        string dest = GenerateUniqueInFolder(prefabsFolder, Path.GetFileName(ap));
                        if (!dryRunMode)
                        {
                            string mv = AssetDatabase.MoveAsset(ap, dest);
                            if (!string.IsNullOrEmpty(mv)) Debug.LogWarning($"[FolderOrganiser] MoveAsset returned: {mv}");
                            else Debug.Log($"[FolderOrganiser] Moved prefab: {dest}");
                        }
                    }
                    else if (ext == ".mat")
                    {
                        string dest = GenerateUniqueInFolder(materialsFolder, Path.GetFileName(ap));
                        if (!dryRunMode)
                        {
                            string mv = AssetDatabase.MoveAsset(ap, dest);
                            if (!string.IsNullOrEmpty(mv)) Debug.LogWarning($"[FolderOrganiser] MoveAsset returned: {mv}");
                            else Debug.Log($"[FolderOrganiser] Moved material: {dest}");
                        }
                    }
                    else if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".tga" || ext == ".psd" || ext == ".exr")
                    {
                        string dest = GenerateUniqueInFolder(texturesFolder, Path.GetFileName(ap));
                        if (!dryRunMode)
                        {
                            string mv = AssetDatabase.MoveAsset(ap, dest);
                            if (!string.IsNullOrEmpty(mv)) Debug.LogWarning($"[FolderOrganiser] MoveAsset returned: {mv}");
                            else Debug.Log($"[FolderOrganiser] Moved texture: {dest}");
                        }
                    }
                    else if (ext == ".anim")
                    {
                        string dest = GenerateUniqueInFolder(animationsFolder, Path.GetFileName(ap));
                        if (!dryRunMode)
                        {
                            string mv = AssetDatabase.MoveAsset(ap, dest);
                            if (!string.IsNullOrEmpty(mv)) Debug.LogWarning($"[FolderOrganiser] MoveAsset returned: {mv}");
                            else Debug.Log($"[FolderOrganiser] Moved animation: {dest}");
                        }
                    }
                    else
                    {
                        // others -> Other/
                        string dest = GenerateUniqueInFolder(otherFolder, Path.GetFileName(ap));
                        if (!dryRunMode)
                        {
                            string mv = AssetDatabase.MoveAsset(ap, dest);
                            if (!string.IsNullOrEmpty(mv)) Debug.LogWarning($"[FolderOrganiser] MoveAsset returned: {mv}");
                            else Debug.Log($"[FolderOrganiser] Moved other: {dest}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[FolderOrganiser] Error processing {ap}: {ex.Message}");
                }

                processedCount++;
            }

            if (!dryRunMode)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[FolderOrganiser] Exception: {ex}");
            EditorUtility.DisplayDialog("Error", $"Organise failed: {ex.Message}", "OK");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        Debug.Log($"[FolderOrganiser] Done. Processed {processedCount} assets. Extracted clips: {createdClips.Count}");
        EditorUtility.DisplayDialog("Done", $"Processed {processedCount} asset(s). Extracted clips: {createdClips.Count}", "OK");
    }

    private string EnsureSubfolder(string parentFolder, string subfolderName)
    {
        parentFolder = parentFolder.TrimEnd('/');
        string subPath = parentFolder + "/" + subfolderName;
        if (!AssetDatabase.IsValidFolder(subPath))
        {
            string guid = AssetDatabase.CreateFolder(parentFolder, subfolderName);
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogWarning($"[FolderOrganiser] CreateFolder failed for {subPath}");
            }
            else
            {
                Debug.Log($"[FolderOrganiser] Created folder: {subPath}");
            }
        }
        return subPath;
    }

    private string GenerateUniqueInFolder(string folder, string fileName)
    {
        string candidate = (folder.TrimEnd('/') + "/" + MakeSafeFileName(fileName)).Replace('\\', '/');
        return AssetDatabase.GenerateUniqueAssetPath(candidate);
    }

    private bool IsUnderSubfolder(string assetPath, string subfolder)
    {
        if (string.IsNullOrEmpty(assetPath) || string.IsNullOrEmpty(subfolder)) return false;
        return assetPath.StartsWith(subfolder + "/", StringComparison.OrdinalIgnoreCase) || assetPath.Equals(subfolder, StringComparison.OrdinalIgnoreCase);
    }

    private string MakeSafeFileName(string name)
    {
        if (string.IsNullOrEmpty(name)) return "asset";
        var invalid = Path.GetInvalidFileNameChars();
        foreach (var c in invalid)
            name = name.Replace(c, '_');
        name = name.Replace('/', '_').Replace('\\', '_').Replace(':', '_');
        name = name.Replace(" ", "_");
        return name;
    }
}