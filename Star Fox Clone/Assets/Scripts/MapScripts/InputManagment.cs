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
        if (state.Players != null)
        {
            if (state.Players[0].IsInGame)
            {
                state.Players[0].ApplyMovement(inputMovementKeyboard());
                state.Players[0].ApplyRotation(inputRotationKeyboard());
                combatInputsKeyboard(state.Players[0]);
            }

            if (state.Players.Length > 1 && state.Players[1].IsInGame) 
            { 
                state.Players[1].ApplyMovement(inputMovementController());
                state.Players[1].ApplyRotation(inputRotationController());
                combatInputsController(state.Players[1]);
            }
        }
    }
    
    Vector3 inputMovementKeyboard()
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

    Vector3 inputMovementController()
    {
        Vector3 retVal = Vector3.zero;

        if (Input.GetAxis("Horizontal-2") != 0)
        {
            retVal += new Vector3(Input.GetAxis("Horizontal-2"), 0, 0);
        }

        if (Input.GetAxis("Vertical-2") != 0)
        {
            retVal += new Vector3(0, Input.GetAxis("Vertical-2"), 0);
        }

        return retVal;
    }

    float inputRotationKeyboard()
    {
        float retVal = 0;

        if (Input.GetAxis("Rotation") != 0)
        {
            //Debug.Log("Added rotation: " + Input.GetAxis("Rotation"));
            retVal += Input.GetAxis("Rotation");
        }
        return retVal;
    }
    
    float inputRotationController()
    {
        float retVal = 0;

        if (Input.GetAxis("Rotation-2") != 0)
        {
            //Debug.Log("Added rotation: " + Input.GetAxis("Rotation"));
            retVal += Input.GetAxis("Rotation-2");
        }
        return retVal;
    }
    
    void combatInputsKeyboard(Player player)
    {
        if (Input.GetButton("Mark")) player.applyCombatInputs(INPUTS.Scan);

        if (Input.GetButtonDown("SwitchTargets")) player.applyCombatInputs(INPUTS.SwitchTargets);
        //NumButtons
        if (Input.GetButtonDown("Missle1")) player.applyCombatInputs(INPUTS.Missle, 0);
        if (Input.GetButtonDown("Missle2")) player.applyCombatInputs(INPUTS.Missle, 1);
        if (Input.GetButtonDown("Missle3")) player.applyCombatInputs(INPUTS.Missle, 2);
        if (Input.GetButtonDown("SwitchMissles")) player.applyCombatInputs(INPUTS.SwitchMissle);
        if (Input.GetButtonDown("FallBack")) GameStateConnection.Instance.SwitchPlayerPositions();
    }

    void combatInputsController(Player player)
    {
        if (Input.GetButton("Mark-2")) player.applyCombatInputs(INPUTS.Scan);

        if (Input.GetButtonDown("SwitchTargets-2")) player.applyCombatInputs(INPUTS.SwitchTargets);
        //NumButtons
        if (Input.GetButtonDown("Missle1-2")) player.applyCombatInputs(INPUTS.Missle, 0);
        if (Input.GetButtonDown("Missle2-2")) player.applyCombatInputs(INPUTS.Missle, 1);
        if (Input.GetButtonDown("Missle3-2")) player.applyCombatInputs(INPUTS.Missle, 2);
        if (Input.GetButtonDown("SwitchMissles-2")) player.applyCombatInputs(INPUTS.SwitchMissle);
        if (Input.GetButtonDown("FallBack-2")) GameStateConnection.Instance.SwitchPlayerPositions();
        //Dpad axis TODO: called every frame after pushing axis down
        if (Input.GetAxisRaw("DPad X") < -0.1) { if (DPadBool) player.applyCombatInputs(INPUTS.Missle, 0); DPadBool = false; }
        else if (Input.GetAxisRaw("DPad X") > 0.1) { if (DPadBool) player.applyCombatInputs(INPUTS.Missle, 1); DPadBool = false; }
        else if (Input.GetAxisRaw("DPad Y") > 0.1) { if (DPadBool) player.applyCombatInputs(INPUTS.Missle, 2); DPadBool = false; }
        else DPadBool = true;
    }

    /*
    #region checkInputDevice

    private bool isMouseKeyboard()
    {
        // mouse & keyboard buttons and mouse movement
        if (Input.anyKey ||
            Input.GetMouseButton(0) ||
            Input.GetMouseButton(1) ||
            Input.GetMouseButton(2) ||
            Input.GetAxis("Mouse ScrollWheel") != 0.0f)
        {
            return true;
        }
        return false;
    }

    private bool isControllerInput()
    {
        // joystick buttons
        // check if we're not using a key for the axis' at the end 
        if (Input.GetKey(KeyCode.Joystick1Button0) ||
           Input.GetKey(KeyCode.Joystick1Button1) ||
           Input.GetKey(KeyCode.Joystick1Button2) ||
           Input.GetKey(KeyCode.Joystick1Button3) ||
           Input.GetKey(KeyCode.Joystick1Button4) ||
           Input.GetKey(KeyCode.Joystick1Button5) ||
           Input.GetKey(KeyCode.Joystick1Button6) ||
           Input.GetKey(KeyCode.Joystick1Button7) ||
           Input.GetKey(KeyCode.Joystick1Button8) ||
           Input.GetKey(KeyCode.Joystick1Button9) ||
           Input.GetKey(KeyCode.Joystick1Button10) ||
           Input.GetKey(KeyCode.Joystick1Button11) ||
           Input.GetKey(KeyCode.Joystick1Button12) ||
           Input.GetKey(KeyCode.Joystick1Button13) ||
           Input.GetKey(KeyCode.Joystick1Button14) ||
           Input.GetKey(KeyCode.Joystick1Button15) ||
           Input.GetKey(KeyCode.Joystick1Button16) ||
           Input.GetKey(KeyCode.Joystick1Button17) ||
           Input.GetKey(KeyCode.Joystick1Button18) ||
           Input.GetKey(KeyCode.Joystick1Button19) ||
           Input.GetAxis("Horizontal") != 0.0f ||
           Input.GetAxis("Vertical") != 0.0f)
        {
            return true;
        }

        return false;
    }

    #endregion
    */
}

public enum INPUTS
{
    Scan
    , SwitchTargets
    , Missle
    , SwitchMissle
}
