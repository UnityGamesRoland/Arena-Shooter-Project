using UnityEngine;

public class PauseManager : MonoBehaviour
{
	public CanvasGroup pauseUI;

    [HideInInspector] public bool isPaused;
    [HideInInspector] public bool cursorStateBeforePause;

    private PlayerManager player;

	#region Singleton And References
	public static PauseManager Instance {get; private set;}
	private void Awake()
	{
		if(Instance == null) Instance = this;
        
        pauseUI.alpha = isPaused ? 1 : 0;
        pauseUI.interactable = isPaused;
        pauseUI.blocksRaycasts = isPaused;
    }
	#endregion

	private void Start()
	{
		//Initialize the pause menu.
		ContinueGame();
	}

	private void Update()
	{
		//Fade the pause UI.
		pauseUI.alpha = Mathf.Lerp(pauseUI.alpha, isPaused ? 1 : 0, Time.unscaledDeltaTime * (isPaused ? 12f : 16f));
		pauseUI.interactable = isPaused;
		pauseUI.blocksRaycasts = isPaused;
	}

	public void PauseGame()
	{
		//IMPORTANT! Since dash particle is fucked, it has to be paused before time scale gets set to prevent memory leak.
		//player.dashParticle.Pause();

        //Show the cursor.
        cursorStateBeforePause = Cursor.visible;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        //Adjust the time scale.
        Time.timeScale = 0f;

        //Pause the sounds.
        AudioManager.Instance.TransitionToSnapshot("Paused", 0.05f);
        AudioListener.pause = true;

		//Initializes the pause menu's UI elements and enables interaction with them.
		UI_Manager.Instance.interactable = true;

        //Update the paused state.
        isPaused = true;
    }

	public void ContinueGame()
	{
        //Show the cursor.
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = cursorStateBeforePause;

        //Adjust the time scale.
        Time.timeScale = 1f;

        //If the game was paused (state update happens at the end of this function) reset to normal snapshot.
        if(isPaused) AudioManager.Instance.TransitionToSnapshot("Normal", 0.05f);
        AudioListener.pause = false;

		//Disable the interaction with the UI.
		UI_Manager.Instance.interactable = false;

        //Update the paused state and delay the next pause by a few seconds.
        isPaused = false;

        //IMPORTANT! Since the dashing particle is paused, check if we are dashing and restart/clear the particles.
        //if(motor.passive.isDashing) player.dashParticle.Play();
        //else player.dashParticle.Clear();
    }
}
