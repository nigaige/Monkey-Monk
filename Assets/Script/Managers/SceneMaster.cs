using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMaster : MonoBehaviour
{
    public static SceneMaster Instance { get; private set; }

    [SerializeField] private LoadingScreen loadingScreen;

    private void Awake()
    {
        Instance = this;
    }

    public void LoadLevel(LevelSO level)
    {
        StartCoroutine(LoadLevelCoroutine(level));
    }

    private IEnumerator LoadLevelCoroutine(LevelSO level)
    {
        yield return loadingScreen.FadeIn();

        AsyncOperation asyncMapUnload = SceneManager.UnloadSceneAsync("HubScene");
        AsyncOperation asyncLevelLoad = SceneManager.LoadSceneAsync(level.SceneName, LoadSceneMode.Additive);
        asyncLevelLoad.completed += _ => SceneManager.SetActiveScene(SceneManager.GetSceneByName(level.SceneName));

        yield return loadingScreen.DisplayLoading(asyncMapUnload, asyncLevelLoad);
        
        yield return loadingScreen.FadeOut();
    }

}
