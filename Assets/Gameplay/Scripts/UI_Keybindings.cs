using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_Keybindings : MonoBehaviour
{
    public KeybindingsProfile keybindings;
    public Text moveForwardText;
    public Text moveBackwardText;
    public Text moveLeftText;
    public Text moveRightText;
    public Text shootText;
    public Text usePowerupText;

    private bool canEnterKeybindingMode = true;

    public void EnterKeybindingMode()
    {
        //Enter keybinding mode.
        if(canEnterKeybindingMode) UI_Manager.Instance.isKeybinding = true;
    }

    public void SetKeybinding(int keyID)
    {
        //Empty out the keybinding text for key at keyID.
        EmptyOutKeybindingText(keyID);

        //Check if there was an input.
        if (Input.anyKeyDown)
        {
            //Loop through all the fucking keycodes to check which one was pressed.
            foreach(KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                //Check if the current key was the one that has been pressed.
                if(Input.GetKeyDown(key))
                {
                    //Check if we want to cancel the keybinding.
                    if(key == KeyCode.Escape)
                    {
                        UI_Manager.Instance.isKeybinding = false;
                        break;
                    }

                    //Check if the key is a forbidden key.
                    if (key == KeyCode.Return) continue;

                    //Set the keybinding for keyID.
                    keybindings.SetAndValidateKeybinding(keyID, key);
                    UI_Manager.Instance.isKeybinding = false;

                    //Delay the next keybinding mode enter.
                    StartCoroutine(DelayNextKeybindingModeEnter());
                    break;
                }
            }

            //After the keybindings has been validated update the display texts.
            UpdateKeybindingTexts();
        }
    }

    public void UpdateKeybindingTexts()
    {
        //Update the keybinding's display texts.
        moveForwardText.text = keybindings.moveForwardKey.ToString();
        moveBackwardText.text = keybindings.moveBackwardKey.ToString();
        moveLeftText.text = keybindings.moveLeftKey.ToString();
        moveRightText.text = keybindings.moveRightKey.ToString();
        shootText.text = keybindings.shootKey.ToString();
        usePowerupText.text = keybindings.usePowerupKey.ToString();
    }

    private void EmptyOutKeybindingText(int keyID)
    {
        //Empty out the keybinding text for key at keyID.
        if (keyID == 0) moveForwardText.text = "...";
        if (keyID == 1) moveBackwardText.text = "...";
        if (keyID == 2) moveLeftText.text = "...";
        if (keyID == 3) moveRightText.text = "...";
        if (keyID == 4) shootText.text = "...";
        if (keyID == 5) usePowerupText.text = "...";
    }

    private IEnumerator DelayNextKeybindingModeEnter()
    {
        //Delay the next enter otherwise when we bind a key to mouse click (and it is also hovering the UI Element) it would also restart the keybinding.
        canEnterKeybindingMode = false;
        yield return new WaitForSecondsRealtime(0.1f);
        canEnterKeybindingMode = true;
    }
}
