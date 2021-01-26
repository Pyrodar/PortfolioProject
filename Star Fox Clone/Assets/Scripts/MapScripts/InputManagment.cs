using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagment : MonoBehaviour
{
    GameStateConnection state;

    bool DPadBool;

    private void Start()
    {
        state = GameStateConnection.Instance;
    }

    void Update()
    {
        if (state.Players != null && state.Players[0].IsInGame)
        {
            for (int i = 0; i < state.NumberOfPlayers; i++)
            {
                state.Players[i].ApplyMovement(inputMovement());
                state.Players[i].ApplyRotation(inputRotation());
                combatInputs(state.Players[i]);
            }
        }
    }
    
    Vector3 inputMovement()
    {
        Vector3 retVal = Vector3.zero;

        if (Input.GetAxis("Horizontal") != 0)
        {
            retVal += new Vector3(Input.GetAxis("Horizontal"), 0, 0);
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            retVal += new Vector3(0, Input.GetAxis("Vertical"), 0);
        }

        return retVal;
    }

    float inputRotation()
    {
        float retVal = 0;

        if (Input.GetAxis("Rotation") != 0)
        {
            //Debug.Log("Added rotation: " + Input.GetAxis("Rotation"));
            retVal += Input.GetAxis("Rotation");
        }
        return retVal;
    }
    
    void combatInputs(Player player)
    {
        if (Input.GetButton("Mark")) player.applyCombatInputs(INPUTS.Scan);

        if (Input.GetButtonDown("SwitchTargets")) player.applyCombatInputs(INPUTS.SwitchTargets);
        //NumButtons
        if (Input.GetButtonDown("Missle1")) player.applyCombatInputs(INPUTS.Missle, 0);
        if (Input.GetButtonDown("Missle2")) player.applyCombatInputs(INPUTS.Missle, 1);
        if (Input.GetButtonDown("Missle3")) player.applyCombatInputs(INPUTS.Missle, 2);
        if (Input.GetButtonDown("SwitchMissles")) player.applyCombatInputs(INPUTS.SwitchMissle);
        //Dpad axis TODO: called every frame after pushing axis down
        if (Input.GetAxisRaw("DPad X") < -0.1) { if (DPadBool) player.applyCombatInputs(INPUTS.Missle, 0); DPadBool = false; }
        else if (Input.GetAxisRaw("DPad X") > 0.1) { if (DPadBool) player.applyCombatInputs(INPUTS.Missle, 1); DPadBool = false; }
        else if (Input.GetAxisRaw("DPad Y") > 0.1) { if (DPadBool) player.applyCombatInputs(INPUTS.Missle, 2); DPadBool = false; }
        else DPadBool = true;
    }

}

public enum INPUTS
{
    Scan
    , SwitchTargets
    , Missle
    , SwitchMissle
}
