using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MeleeManager : MonoBehaviour
{
    public MeleeItem currentMelee;

    private int currentComboIndex = 0;
    private bool canAttack = true;

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
        if (player.isDead || pause.isPaused) return;

        if (Input.GetMouseButtonDown(0) && Time.time > player.actionTimer && !ability.isShielded && canAttack)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        canAttack = false;

        anim.isIKActive = false;
        anim.animator.applyRootMotion = true;

        MeleeAttack currentAttack = currentMelee.attacks[currentComboIndex];
        List<MeleeHitregTimer> hitregData = currentAttack.hitregTimer.ToList();

        anim.animator.CrossFade(currentAttack.animName, currentAttack.transitionTime);
        player.actionTimer = Time.time + currentAttack.duration / currentAttack.animSpeed;

        currentMelee.hitreg.DisableHitreg();

        float attackProgressInSeconds = 0;
        bool hasAttackInput = false;

        while (attackProgressInSeconds < currentAttack.duration / currentAttack.animSpeed)
        {
            attackProgressInSeconds += Time.deltaTime;

            if(Input.GetKeyDown(KeyCode.Mouse0) && !hasAttackInput && currentComboIndex < currentMelee.attacks.Length - 1)
            {
                if (attackProgressInSeconds > 0.1f && attackProgressInSeconds < currentAttack.duration / currentAttack.animSpeed - 0.1f)
                {
                    hasAttackInput = true;
                }
            }

            if(attackProgressInSeconds >= currentAttack.minDuration / currentAttack.animSpeed && hasAttackInput)
            {
                attackProgressInSeconds = currentAttack.duration / currentAttack.animSpeed;
            }

            for (int i = hitregData.Count - 1; i >= 0; i--)
            {
                if (attackProgressInSeconds >= hitregData[i].fromSeconds / currentAttack.animSpeed)
                {
                    currentMelee.hitreg.EnableHitReg(hitregData[i].duration / currentAttack.animSpeed);
                    hitregData.RemoveAt(i);
                    break;
                }
            }

            yield return null;
        }

        if(hasAttackInput)
        {
            currentComboIndex ++;
            StartCoroutine(Attack());
        }

        else
        {
            anim.isIKActive = true;
            anim.animator.applyRootMotion = false;
            canAttack = true;
            currentComboIndex = 0;
        }
    }
}
