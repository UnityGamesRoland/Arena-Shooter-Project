using UnityEngine;
using UnityEngine.UI;

public class UI_GameDisplay : MonoBehaviour
{
    public Slider energySlider;

    #region Singleton
    public static UI_GameDisplay Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    #endregion

    public void UpdateEnergySlider(float value)
    {
        energySlider.value = value;
    }
}
