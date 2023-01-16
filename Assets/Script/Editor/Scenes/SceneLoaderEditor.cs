#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoaderEditor : EditorWindow
{
    private static bool _autoLoad;

    [InitializeOnLoadMethod]
    static void AppLoad()
    {
        _autoLoad = EditorPrefs.GetBool("SceneLoader.autoLoad", _autoLoad);
        EditorApplication.hierarchyChanged += EditorApplication_hierarchyChanged;
    }

    private static void EditorApplication_hierarchyChanged()
    {
        if (_autoLoad) TryToLoadMasterScene();
    }

    [MenuItem("Window/SceneLoader", priority = 4000)]
    public static void ShowWindow()
    {
        GetWindow<SceneLoaderEditor>("SceneLoader");
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        _autoLoad = EditorGUILayout.Toggle("Auto load Master scene", _autoLoad);

        if (EditorGUI.EndChangeCheck())
        {
            if (_autoLoad) TryToLoadMasterScene();
        }
    }

    private static void TryToLoadMasterScene()
    {
        if (EditorSceneManager.GetActiveScene().name == "Master") return;

        bool isMasterLoaded = false;
        for (int i = 0; i < EditorSceneManager.sceneCount; i++)
        {
            if (EditorSceneManager.GetSceneAt(i).isLoaded && EditorSceneManager.GetSceneAt(i).name == "Master")
            {
                isMasterLoaded = true;
                break;
            }
        }

        if (!isMasterLoaded)
        {
            string masterPath = FindMasterScenePath();

            EditorSceneManager.OpenScene(masterPath, OpenSceneMode.Additive);
            EditorSceneManager.MoveSceneBefore(EditorSceneManager.GetSceneByName("Master"), EditorSceneManager.GetSceneAt(0));
            EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByName("Master"));

            Debug.Log("Master scene auto loaded !");
        }
        else
        {
            EditorSceneManager.SetActiveScene(EditorSceneManager.GetSceneByName("Master"));
        }
    }

    private static string FindMasterScenePath()
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            int lastSlash = path.LastIndexOf("/");
            string sceneName = path.Substring(lastSlash + 1, path.LastIndexOf(".") - lastSlash - 1);

            if (sceneName == "Master") return path;
        }

        return "";
    }

    private void OnDisable()
    {
        EditorPrefs.SetBool("SceneLoader.autoLoad", _autoLoad);
    }
}
#endif