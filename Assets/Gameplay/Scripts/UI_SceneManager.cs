using UnityEngine;

public class UI_SceneManager : MonoBehaviour
{
    #region Singleton
    public static UI_SceneManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    #endregion

    public void LoadLevel(string mode)
    {
        //Load a scene defined with <mode>.
        LevelManager.Instance.LoadLevel(mode);
    }

    public void NewGame()
    {
        //Create save profile

        LevelManager.Instance.LoadLevel("New");
    }

    public void QuitGame()
    {
        //Quit the game. Should only be called from the main menu.
        if (!Application.isEditor) System.Diagnostics.Process.GetCurrentProcess().Kill();
    }
}
