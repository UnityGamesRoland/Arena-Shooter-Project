using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UI_SceneManager : MonoBehaviour
{
    public CanvasGroup loadingScreen;

    public enum SceneLoaderType {loopLevels, reloadCurrent}
    public SceneLoaderType loadType;

    public LevelSection[] levelSections;

    [HideInInspector] public bool isLoading;
    [HideInInspector] public bool isInitialized;

    #region Singleton And References
    public static UI_SceneManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (SceneManager.GetActiveScene().buildIndex == 0) StartCoroutine(InitializeMenu());
        else StartCoroutine(InitializeGameLevel());
    }
    #endregion

    public void LoadMenu()
    {
        //Check if the game is loading something.
        if (isLoading) return;

        //Crossfade to section 1's soundtrack.
        AudioManager.Instance.CrossFadeMusic(AudioManager.Instance.soundtracks[0], 0.7f);

        //Start the loading process.
        StartCoroutine(LoadToMenu());
    }

    public void LoadNextLevel()
    {
        //Check if the game is loading something.
        if (isLoading) return;

        //Get the target level index.
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int targetLevel = currentLevel + 1;

        //TEMPORARY FEATURE... Load the first level or reload the current one based on the selected loading type.
        if (targetLevel > SceneManager.sceneCountInBuildSettings - 1) targetLevel = (loadType == SceneLoaderType.loopLevels) ? 1 : currentLevel;

        //Execute on new section events.
        foreach (LevelSection section in levelSections)
        {
            if (targetLevel == section.fromLevel)
            {
                AudioManager.Instance.CrossFadeMusic(AudioManager.Instance.soundtracks[section.levelSoundtrackIndex], 1.2f);
                break;
            }
        }

        //Start the loading process.
        StartCoroutine(LoadNextGameLevel(targetLevel));
    }

    public void RestartLevel()
    {
        //Check if the game is loading something.
        if (isLoading) return;

        //Start the loading process.
        StartCoroutine(ReloadGameLevel());
    }

    public void StartNewGame()
    {
        //Check if the game is loading something.
        if (isLoading) return;

        //CREATE NEW SAVE PROFILE !!

        //Crossfade to section 1's soundtrack.
        AudioManager.Instance.CrossFadeMusic(AudioManager.Instance.soundtracks[1], 1.2f);

        //Start the loading process.
        StopAllCoroutines();
        StartCoroutine(LoadLevelFromMenu(1));
    }

    public void QuitGame()
    {
        //Quit the game. Should only be called from the main menu.
        if (!Application.isEditor) System.Diagnostics.Process.GetCurrentProcess().Kill();
    }

    private IEnumerator LoadNextGameLevel(int index)
    {
        //Make sure that no other loading will start while the current async operation is running.
        isLoading = true;

        //Reset the progress.
        float progress = 0;

        //Enable the loading screen.
        while (progress < 1)
        {
            progress += Time.unscaledDeltaTime / 0.2f;
            loadingScreen.alpha = Mathf.Lerp(0, 1, progress);
            yield return null;
        }

        //Apply the loading delay.
        yield return new WaitForSecondsRealtime(1);

        //Start the loading process.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);
        asyncLoad.allowSceneActivation = true;
    }

    private IEnumerator ReloadGameLevel()
    {
        //Make sure that no other loading will start while the current async operation is running.
        isLoading = true;

        //Transition to normal audio.
        AudioManager.Instance.TransitionToSnapshot("Paused", 0.8f);

        //Reset the progress.
        float progress = 0;

        //Enable the loading screen.
        while(progress < 1)
        {
            progress += Time.unscaledDeltaTime / 0.2f;
            loadingScreen.alpha = Mathf.Lerp(0, 1, progress);
            yield return null;
        }

        //Start the loading process.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        asyncLoad.allowSceneActivation = true;
    }

    private IEnumerator LoadLevelFromMenu(int index)
    {
        //Make sure that no other loading will start while the current async operation is running.
        isLoading = true;

        //Enable the loading screen.
        loadingScreen.alpha = 1f;

        //Apply the loading delay.
        yield return new WaitForSecondsRealtime(1);

        //Start the loading process.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);
        asyncLoad.allowSceneActivation = true;
    }

    private IEnumerator LoadToMenu()
    {
        //Make sure that no other loading will start while the current async operation is running.
        isLoading = true;

        //Enable the loading screen.
        loadingScreen.alpha = 1f;

        //Apply the loading delay.
        yield return new WaitForSecondsRealtime(1);

        //Start the loading process.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0);
        asyncLoad.allowSceneActivation = true;
    }

    private IEnumerator InitializeGameLevel()
    {
        //Show the loading screen in order to let it fade out.
        loadingScreen.alpha = 1;

        //Let the Awake() and Start() to complete processing.
        yield return new WaitForSecondsRealtime(1);
        isInitialized = true;

        //Transition to normal audio.
        AudioManager.Instance.TransitionToSnapshot("Normal", 1.3f);

        //Reset the progress.
        float progress = 0;

        //Enable the loading screen.
        while (progress < 1)
        {
            progress += Time.unscaledDeltaTime / 0.2f;
            loadingScreen.alpha = Mathf.Lerp(1, 0, progress);
            yield return null;
        }
    }

    private IEnumerator InitializeMenu()
    {
        //Show the loading screen in order to let it fade out.
        loadingScreen.alpha = 1;

        //Let the Awake() and Start() to complete processing.
        yield return new WaitForSecondsRealtime(1);
        isInitialized = true;

        //Transition to normal audio.
        AudioManager.Instance.TransitionToSnapshot("Normal", 0.01f);

        //Enable the loading screen.
        while (loadingScreen.alpha > 0)
        {
            loadingScreen.alpha = Mathf.MoveTowards(loadingScreen.alpha, 0f, Time.unscaledDeltaTime / 2.5f);
            yield return null;
        }
    }
}

[System.Serializable]
public class LevelSection
{
    public string name = "New Section";
    public int fromLevel = 1;
    public int levelSoundtrackIndex = 1;
}
