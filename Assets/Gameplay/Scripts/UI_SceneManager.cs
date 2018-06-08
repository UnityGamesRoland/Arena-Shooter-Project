using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UI_SceneManager : MonoBehaviour
{
    public CanvasGroup loadingScreen;
    public bool isLastLevel;

    [HideInInspector] public bool isLoading;
    [HideInInspector] public bool isInitialized;

    #region Singleton And References
    public static UI_SceneManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        StartCoroutine(InitializeLevel());
    }
    #endregion

    public void LoadLevel(int index)
    {
        //Check if the game is loading something.
        if (isLoading) return;

        //Start the loading process.
        StartCoroutine(LoadLevelAsync(index));
    }

    public void LoadNextLevel()
    {
        //Check if the game is loading something.
        if (isLoading) return;

        //Get the target level index.
        int targetLevel = SceneManager.GetActiveScene().buildIndex + 1;

        //TEMPORARY FEATURE... Load the first level if we completed the last one.
        if (isLastLevel) targetLevel = 0;

        //Start the loading process.
        StartCoroutine(LoadLevelAsync(targetLevel));
    }

    public void RestartLevel()
    {
        //Get the target level index.
        int targetLevel = SceneManager.GetActiveScene().buildIndex;

        //Start the loading process.
        StartCoroutine(LoadLevelAsync(targetLevel));
    }

    public void QuitGame()
    {
        //Quit the game. Should only be called from the main menu.
        Application.Quit();
    }

    private IEnumerator InitializeLevel()
    {
        //Show the loading screen in order to let it fade out.
        loadingScreen.alpha = 1;

        //Let the Awake() and Start() to complete processing.
        yield return new WaitForSeconds(1);
        isInitialized = true;

        //Reset the progress.
        float progress = 0;

        //Enable the loading screen.
        while (progress < 1)
        {
            progress += Time.deltaTime / 0.4f;
            loadingScreen.alpha = Mathf.Lerp(1, 0, progress);
            yield return null;
        }
    }

    private IEnumerator LoadLevelAsync(int index)
    {
        //Make sure that no other loading will start while the current async operation is running.
        isLoading = true;

        //Reset the progress.
        float progress = 0;

        //Enable the loading screen.
        while(progress < 1)
        {
            progress += Time.unscaledDeltaTime / 0.4f;
            loadingScreen.alpha = Mathf.Lerp(0, 1, progress);
            yield return null;
        }

        //Start the loading process.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);
        asyncLoad.allowSceneActivation = true;

        //Check if the loading is still going.
        while (!asyncLoad.isDone)
        {
            //Wait for the next frame.
            yield return null;
        }
    }
}
