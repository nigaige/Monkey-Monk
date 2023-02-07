using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MonkeyMonk.Inputs;

public class SceneMaster : MonoBehaviour
{
    public static SceneMaster Instance { get; private set; }

    public LevelSO CurrentLevel { get; private set; } 

    [SerializeField] private LoadingScreen loadingScreen;


    private void Awake()
    {
        Instance = this;

        if(SceneManager.sceneCount == 1)
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        }
    }


    public void LoadHubWorld()
    {
        CurrentLevel = null;
        StartCoroutine(LoadHubWorldCoroutine());
    }

    public IEnumerator LoadHubWorldCoroutine()
    {
        InputManager.Instance.DisableInput();
        yield return loadingScreen.FadeIn();

        // Unload
        List<AsyncOperation> asyncOps = UnloadLoadedScenes();

        // Load
        AsyncOperation asyncLevelLoad = SceneManager.LoadSceneAsync("HubWorld", LoadSceneMode.Additive);
        asyncLevelLoad.completed += _ => SceneManager.SetActiveScene(SceneManager.GetSceneByName("HubWorld"));
        asyncOps.Add(asyncLevelLoad);

        yield return loadingScreen.DisplayLoading(asyncOps.ToArray());

        yield return loadingScreen.FadeOut();
        InputManager.Instance.SwitchInputMap(InputMap.Map);
    }



    public void LoadLevel(LevelSO level)
    {
        CurrentLevel = level;
        StartCoroutine(LoadLevelCoroutine(level));
    }

    private IEnumerator LoadLevelCoroutine(LevelSO level)
    {
        InputManager.Instance.DisableInput();
        yield return loadingScreen.FadeIn();

        // Unload
        List<AsyncOperation> asyncOps = UnloadLoadedScenes();

        // Load
        AsyncOperation asyncLevelLoad = SceneManager.LoadSceneAsync(level.SceneName, LoadSceneMode.Additive);
        asyncLevelLoad.completed += _ => SceneManager.SetActiveScene(SceneManager.GetSceneByName(level.SceneName));
        asyncOps.Add(asyncLevelLoad);
        asyncOps.Add(SceneManager.LoadSceneAsync("GameUI", LoadSceneMode.Additive));

        yield return loadingScreen.DisplayLoading(asyncOps.ToArray());
        
        yield return loadingScreen.FadeOut();
        InputManager.Instance.SwitchInputMap(InputMap.Game);
    }

    /// <summary>
    /// Unload all loaded scenes except Master
    /// </summary>
    private List<AsyncOperation> UnloadLoadedScenes()
    {
        List<AsyncOperation> asyncOps = new();

        List<string> scenesNames = new();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == "Master") continue;

            scenesNames.Add(SceneManager.GetSceneAt(i).name);
        }

        foreach (string scene in scenesNames)
        {
            asyncOps.Add(SceneManager.UnloadSceneAsync(scene));
        }

        return asyncOps;
    }

}
