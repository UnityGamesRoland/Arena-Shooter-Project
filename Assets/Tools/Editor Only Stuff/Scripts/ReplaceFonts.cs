using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class ReplaceFonts : MonoBehaviour
{
    public Font font;

    [Button(ButtonSizes.Medium)]
    public void ReplaceFont()
    {
        Text[] texts = FindObjectsOfType<Text>();
        foreach (Text text in texts) text.font = font;
    }
}
