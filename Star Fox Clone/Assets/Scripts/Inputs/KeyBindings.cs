using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "KeyBinding", menuName = "Custom SO / KeyBinding")]
public class KeyBindings : ScriptableObject
{
    public KeyCode Up, Down, Left, Right, CenterFocus, RotateLeft, RotateRight, Mark, SwitchTargets, SwitchMissles, Missle1, Missle2, Missle3, Fallback;

    public INPUTS TranslateInput(string input, int player)
    {
        switch (input)
        {
            default:
                break;
        }



        return INPUTS.None;
    }
}
