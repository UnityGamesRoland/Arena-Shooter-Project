using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public CanvasGroup loadingScreen;
    public LevelSection[] levelSections;

    [HideInInspector] public int currentSceneIndex;
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
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentSceneIndex = scene.buildIndex;
        StartCoroutine(InitializeLevel());
    }
    #endregion

    public void LoadLevel(string mode)
    {
        if (isLoading) return;

        int currentLevel = currentSceneIndex;
        int targetLevel = -1;

        if (mode == "New") targetLevel = 1;
        if (mode == "Menu") targetLevel = 0;
        if (mode == "Next") targetLevel = currentLevel + 1;
        if (mode == "Restart") targetLevel = currentLevel;

        //Temporary feature... Level looping.
        if (targetLevel > SceneManager.sceneCountInBuildSettings - 1)
        {
            targetLevel = 1;
            StartCoroutine(LoadLevel(targetLevel));
            return;
        }

        if (currentLevel != targetLevel)
        {
            for (int i = 0; i < levelSections.Length; i++)
            {
                if(levelSections[i].fromLevel == targetLevel)
                {
                    AudioManager.Instance.CrossFadeMusic(AudioManager.Instance.soundtracks[levelSections[i].levelSoundtrackIndex], 1.2f);
                    break;
                }
            }
        }

        StartCoroutine(LoadLevel(targetLevel));
    }

    private IEnumerator InitializeLevel()
    {
        isLoading = false;

        yield return new WaitForSecondsRealtime(1);

        isInitialized = true;

        AudioManager.Instance.TransitionToSnapshot("Normal", 0.2f);

        while (loadingScreen.alpha > 0)
        {
            loadingScreen.alpha = Mathf.MoveTowards(loadingScreen.alpha, 0, Time.unscaledDeltaTime / 0.2f);
            yield return null;
        }
    }

    private IEnumerator LoadLevel(int sceneIndex)
    {
        AudioManager.Instance.TransitionToSnapshot("Paused", 0.2f);

        while (loadingScreen.alpha < 1)
        {
            loadingScreen.alpha = Mathf.MoveTowards(loadingScreen.alpha, 1, Time.unscaledDeltaTime / 0.2f);
            yield return null;
        }

        isLoading = true;
        isInitialized = false;

        SceneManager.LoadSceneAsync(sceneIndex);
    }
}

[System.Serializable]
public class LevelSection
{
    public string name = "New Section";
    public int fromLevel = 1;
    public int levelSoundtrackIndex = 1;
}
