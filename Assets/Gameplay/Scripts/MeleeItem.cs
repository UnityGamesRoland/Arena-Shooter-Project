using UnityEngine;

public class MeleeItem : MonoBehaviour
{
    public int itemID;
    public MeleeHitreg hitreg;
    public MeleeAttack[] attacks;
}

[System.Serializable]
public class MeleeAttack
{
    public string animName;
    public float animSpeed;
    public float minDuration;
    public float duration;
    public float transitionTime;
    public float damage;
    public MeleeHitregTimer[] hitregTimer;
}

[System.Serializable]
public class MeleeHitregTimer
{
    public float fromSeconds;
    public float duration;
}
