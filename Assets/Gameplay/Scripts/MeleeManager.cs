using UnityEngine;
using System.Collections;

public class MeleeManager : MonoBehaviour
{
    private PlayerManager player;
    private PauseManager pause;
    private AbilityManager ability;
    private TP_Animations anim;

    #region Singleton
    public static MeleeManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    #endregion

    private void Start()
    {
        //Initialization.
        player = PlayerManager.Instance;
        pause = PauseManager.Instance;
        ability = AbilityManager.Instance;
        anim = TP_Animations.Instance;
    }

    private void Update()
    {
        //Check if the player is alive.
        if (player.isDead || pause.isPaused) return;

        //Shoot while holding LMB.
        if (Input.GetMouseButtonDown(1) && Time.time > player.actionTimer && !ability.isShielded)
        {
            StartCoroutine(Slash());
        }
    }

    private IEnumerator Slash()
    {
        //...

        yield return null;
    }
}
