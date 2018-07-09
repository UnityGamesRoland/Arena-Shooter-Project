using UnityEngine;

[CreateAssetMenu(fileName = "Keybindings Profile", menuName = "Keybindings Profile")]
public class KeybindingsProfile : SingletonScriptableObject<KeybindingsProfile>
{
    public KeyCode moveForwardKey = KeyCode.W;
    public KeyCode moveBackwardKey = KeyCode.S;
    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;
    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode dashKey = KeyCode.LeftShift;
    public KeyCode shieldKey = KeyCode.Space;

    public void SetAndValidateKeybinding(int keyID, KeyCode newKey)
    {
        //Create an array out of the keybindings.
        KeyCode[] keybindings = new KeyCode[] { moveForwardKey, moveBackwardKey, moveLeftKey, moveRightKey, shootKey, dashKey, shieldKey };

        //Loop through the keybindings.
        for (int i = 0; i < keybindings.Length; i++)
        {
            //Replace the keybinding at keyID.
            if (i == keyID) keybindings[i] = newKey;

            //Make sure that a key can only be assigned to one keybinding.
            else if (keybindings[i] == newKey) keybindings[i] = KeyCode.None;
        }

        //Plug back the validated keybindings.
        moveForwardKey = keybindings[0];
        moveBackwardKey = keybindings[1];
        moveLeftKey = keybindings[2];
        moveRightKey = keybindings[3];
        shootKey = keybindings[4];
        dashKey = keybindings[5];
        shieldKey = keybindings[6];
    }
}
