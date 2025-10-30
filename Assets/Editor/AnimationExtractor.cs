using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AnimationExtractor : EditorWindow
{
    private DefaultAsset folderAsset;
    private string folderPath;
    private bool includeSubfolders = true;
    private bool replaceInAnimatorControllers = false;
    private string clipSuffix = "_anim";
    private int processedCount;

    [MenuItem("Tools/Animation Extractor")]
    public static void OpenWindow() => GetWindow<AnimationExtractor>("Animation Extractor");

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
        GUILayout.Label("Extract AnimationClips from Model Assets (FBX)", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        folderAsset = (DefaultAsset)EditorGUILayout.ObjectField("Folder (Project)", folderAsset, typeof(DefaultAsset), false);
        if (GUILayout.Button("Select", GUILayout.Width(60)))
        {
            string p = EditorUtility.OpenFolderPanel("Select Folder (Project)", "Assets", "");
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
        replaceInAnimatorControllers = EditorGUILayout.Toggle("Replace in AnimatorControllers", replaceInAnimatorControllers);
        clipSuffix = EditorGUILayout.TextField("Clip Suffix", clipSuffix);

        EditorGUILayout.Space();
        if (GUILayout.Button("Extract Animations"))
        {
            if (string.IsNullOrEmpty(folderPath) || !AssetDatabase.IsValidFolder(folderPath))
            {
                EditorUtility.DisplayDialog("Folder required", "Please assign a valid folder inside Assets.", "OK");
            }
            else
            {
                ExtractAnimations(folderPath, includeSubfolders, replaceInAnimatorControllers);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Processed Assets", processedCount.ToString());
    }

    private void ExtractAnimations(string folder, bool recurse, bool replaceAnimatorRefs)
    {
        processedCount = 0;
        Debug.Log($"[AnimationExtractor] Starting extraction in: {folder} (recurse={recurse}, replaceAnimatorRefs={replaceAnimatorRefs})");

        string absFolder = folder.StartsWith("Assets") ? folder.Replace("Assets", Application.dataPath) : Path.GetFullPath(folder);
        SearchOption searchOpt = recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        string[] modelFiles;
        try
        {
            modelFiles = Directory.GetFiles(absFolder, "*.fbx", searchOpt);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[AnimationExtractor] Failed to enumerate model files: {ex.Message}");
            EditorUtility.DisplayDialog("Error", "Failed to enumerate model files. See Console for details.", "OK");
            return;
        }

        Debug.Log($"[AnimationExtractor] Found {modelFiles.Length} model files under {absFolder}.");

        if (modelFiles.Length == 0)
        {
            EditorUtility.DisplayDialog("No FBX found", $"No .fbx files were found in folder: {folder}", "OK");
            return;
        }

        var createdClips = new Dictionary<string, string>(); // key -> created clip path

        try
        {
            int total = modelFiles.Length;
            for (int i = 0; i < total; i++)
            {
                string file = modelFiles[i];
                if (!file.StartsWith(Application.dataPath)) continue;
                string assetPath = "Assets" + file.Substring(Application.dataPath.Length).Replace('\\', '/');

                if (!recurse)
                {
                    string dir = Path.GetDirectoryName(assetPath).Replace('\\', '/').TrimEnd('/');
                    if (dir != folder.TrimEnd('/')) continue;
                }

                EditorUtility.DisplayProgressBar("Extracting animations", Path.GetFileName(assetPath), (float)i / Mathf.Max(1, total));
                Debug.Log($"[AnimationExtractor] Processing: {assetPath}");

                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                if (assets == null || assets.Length == 0) { processedCount++; continue; }

                // Collect animation clips embedded in the model asset
                List<AnimationClip> clips = new List<AnimationClip>();
                foreach (var a in assets)
                {
                    if (a is AnimationClip clip)
                    {
                        // Skip Unity's preview clips (name starts with "Preview" or contains "preview")
                        if (clip.name.ToLower().Contains("preview")) continue;
                        clips.Add(clip);
                    }
                }

                if (clips.Count == 0) { processedCount++; continue; }

                string targetFolder = Path.GetDirectoryName(assetPath).Replace('\\', '/');

                foreach (var src in clips)
                {
                    // create stable key to avoid duplicates across assets
                    string key = $"{assetPath}:{src.name}";

                    if (!createdClips.TryGetValue(key, out string createdPath))
                    {
                        // sanitize clip name -> remove illegal filename chars (fixes ArgumentException)
                        string safeClipName = MakeSafeFileName(src.name);
                        string safeAssetName = MakeSafeFileName(Path.GetFileNameWithoutExtension(assetPath));

                        string baseName = safeAssetName + "_" + safeClipName + clipSuffix + ".anim";
                        // build an asset-path style candidate (use forward slashes and Assets root)
                        string candidatePath = (targetFolder.TrimEnd('/') + "/" + baseName).Replace('\\', '/');
                        candidatePath = AssetDatabase.GenerateUniqueAssetPath(candidatePath);

                        AnimationClip newClip = Object.Instantiate(src);
                        newClip.name = Path.GetFileNameWithoutExtension(candidatePath);
                        AssetDatabase.CreateAsset(newClip, candidatePath);
                        createdPath = candidatePath;
                        createdClips[key] = createdPath;

                        Debug.Log($"[AnimationExtractor] Created clip: {createdPath} (from {src.name})");
                    }

                    // Optionally replace references in AnimatorControllers inside the same folder (best-effort)
                    if (replaceAnimatorRefs)
                    {
                        AnimationClip replacement = AssetDatabase.LoadAssetAtPath<AnimationClip>(createdPath);
                        ReplaceClipReferencesInFolder(folder, src, replacement, recurse);
                    }
                }

                processedCount++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[AnimationExtractor] Exception: {ex}");
            EditorUtility.DisplayDialog("Error", $"Extraction failed: {ex.Message}", "OK");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        Debug.Log($"[AnimationExtractor] Done. Processed {processedCount} model files. Clips created: {createdClips.Count}");
        EditorUtility.DisplayDialog("Done", $"Processed {processedCount} model file(s). Clips created: {createdClips.Count}", "OK");
    }

    private void ReplaceClipReferencesInFolder(string folder, AnimationClip source, AnimationClip replacement, bool recurse)
    {
        // Find AnimatorControllers in the target folder and replace state motions that match the source clip.
        string[] searchFolders = new[] { folder };
        string filter = "t:AnimatorController";
        string[] guids = AssetDatabase.FindAssets(filter, searchFolders);

        if (guids == null || guids.Length == 0) return;

        foreach (var g in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(g);
            AnimatorController ac = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (ac == null) continue;

            bool changed = false;
            foreach (var layer in ac.layers)
            {
                var sm = layer.stateMachine;
                if (sm == null) continue;
                // iterate states
                foreach (var state in sm.states)
                {
                    var motion = state.state.motion;
                    if (motion == null) continue;

                    // best-effort comparison: reference equality or name match
                    if (motion == source || motion.name == source.name)
                    {
                        state.state.motion = replacement;
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                EditorUtility.SetDirty(ac);
                Debug.Log($"[AnimationExtractor] Replaced clips in AnimatorController: {path}");
            }
        }

        if (recurse)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    // Replace or sanitize characters that are invalid for file names.
    private string MakeSafeFileName(string name)
    {
        if (string.IsNullOrEmpty(name)) return "clip";
        var invalid = Path.GetInvalidFileNameChars();
        foreach (var c in invalid)
            name = name.Replace(c, '_');
        // Also replace common problematic characters
        name = name.Replace('/', '_').Replace('\\', '_').Replace(':', '_');
        name = name.Replace(" ", "_");
        return name;
    }
}