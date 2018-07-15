using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public CanvasGroup loadingScreen;

    [HideInInspector] public bool isLoading;
    [HideInInspector] public bool isInitialized;

    #region Singleton
    public static LevelManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else if (Instance != this) DestroyImmediate(gameObject);
    }
    #endregion

    #region OnSceneLoaded Subscription
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("Assigned");
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(InitializeLevel());
    }
    #endregion

    private void LoadLevel(string mode)
    {
        if (isLoading) return;

        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int targetLevel = -1;

        if(mode == "Menu") targetLevel = 0;
        if(mode == "Next") targetLevel = currentLevel + 1;
        if (mode == "Reload") targetLevel = currentLevel;

        //Temporary feature...
        if (targetLevel > SceneManager.sceneCountInBuildSettings - 1) targetLevel = 1;

        //Replace with soundtrack manager!!!
        if(currentLevel != targetLevel)
        {
            if(targetLevel == 0) AudioManager.Instance.CrossFadeMusic(AudioManager.Instance.soundtracks[0], 1.2f);
            else AudioManager.Instance.CrossFadeMusic(AudioManager.Instance.soundtracks[1], 1.2f);
        }

        StartCoroutine(LoadLevel(targetLevel));
    }

    private IEnumerator InitializeLevel()
    {
        isLoading = false;

        yield return new WaitForSecondsRealtime(1);

        isInitialized = true;

        AudioManager.Instance.TransitionToSnapshot("Normal", 1.3f);

        while (loadingScreen.alpha > 0)
        {
            loadingScreen.alpha = Mathf.MoveTowards(loadingScreen.alpha, 0, Time.unscaledDeltaTime / 0.2f);
            yield return null;
        }
    }

    private IEnumerator LoadLevel(int sceneIndex)
    {
        isLoading = true;
        isInitialized = false;

        AudioManager.Instance.TransitionToSnapshot("Paused", 0.8f);

        while (loadingScreen.alpha < 1)
        {
            loadingScreen.alpha = Mathf.MoveTowards(loadingScreen.alpha, 1, Time.unscaledDeltaTime / 0.2f);
            yield return null;
        }

        SceneManager.LoadSceneAsync(sceneIndex);
    }
}
