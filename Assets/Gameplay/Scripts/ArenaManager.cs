using UnityEngine;
using System.Collections;

public class ArenaManager : MonoBehaviour
{
    public AnimationCurve levelEndGlowIntensity;
    public Material glowMaterial;

    #region Singleton And References
    public static ArenaManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        glowMaterial.SetColor("_Color", Color.red);
        glowMaterial.SetFloat("_Intensity", 3.5f);
    }
    #endregion

    public void CompleteLevel()
    {
        //End the level.
        StartCoroutine(EndLevel());
    }

    private IEnumerator EndLevel()
    {
        //Make the player invincible after the level is finished.
        PlayerManager.Instance.ApplyInvincibility(60);

        //Update the arena LED material and tint the background.
        glowMaterial.SetColor("_Color", Color.green);
        Camera.main.backgroundColor = new Color32(43, 53, 30, 0);

        //Reset the progress.
        float progress = 0f;

        while (progress < 1)
        {
            //Update the progress.
            progress += Time.unscaledDeltaTime / 0.3f;

            //Update the arena LED material.
            glowMaterial.SetFloat("_Intensity", 3f * levelEndGlowIntensity.Evaluate(progress));

            //Wait for the next frame.
            yield return null;
        }

        //Load the next level after a bit of delay.
        yield return new WaitForSeconds(0.7f);
        UI_SceneManager.Instance.LoadNextLevel();
    }
}
