﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#region eventClasses for initialising in code
[System.Serializable]
public class MovementEvent : UnityEvent<Vector3> { }
[System.Serializable]
public class RotationEvent : UnityEvent<float> { }
[System.Serializable]
public class IntegerEvent : UnityEvent<int> { }
[System.Serializable]
public class InputEvent : UnityEvent<INPUTS> { }
#endregion

public class InputManagment : MonoBehaviour
{
    GameConnection state;

    #region Events

    MovementEvent Movement = new MovementEvent();
    RotationEvent Rotation = new RotationEvent();


    InputEvent CombatInputs = new InputEvent();
    IntegerEvent FireMissle = new IntegerEvent();
    IntegerEvent SwitchPositions = new IntegerEvent();


    MovementEvent Movement2 = new MovementEvent();
    RotationEvent Rotation2 = new RotationEvent();


    InputEvent CombatInputs2 = new InputEvent();
    IntegerEvent FireMissle2 = new IntegerEvent();
    IntegerEvent SwitchPositions2 = new IntegerEvent();

    #endregion

    bool DPadBool;

    public void ConnectInputs() 
    {
        #region clearing Events
        Movement.RemoveAllListeners();
        Rotation.RemoveAllListeners();
        CombatInputs.RemoveAllListeners();
        FireMissle.RemoveAllListeners();
        SwitchPositions.RemoveAllListeners();
        Movement2.RemoveAllListeners();
        Rotation2.RemoveAllListeners();
        CombatInputs2.RemoveAllListeners();
        FireMissle2.RemoveAllListeners();
        SwitchPositions2.RemoveAllListeners();
        #endregion

        state = GameConnection.Instance;
        if (state == null) return;

        SwitchPositions.AddListener(state.SwitchPlayerPositions);
        SwitchPositions2.AddListener(state.SwitchPlayerPositions);



        //DEBUGGING: Later set Events based on playerNumber
        Movement.AddListener(state.Players[0].ApplyMovement);
        Rotation.AddListener(state.Players[0].ApplyRotation);
        CombatInputs.AddListener(state.Players[0].applyButtonInputs);
        FireMissle.AddListener(state.Players[0].FireMissle);
        //#####################################
        if (state.NumberOfPlayers < 2) return;
        Movement2.AddListener(state.Players[1].ApplyMovement);
        Rotation2.AddListener(state.Players[1].ApplyRotation);
        CombatInputs2.AddListener(state.Players[1].applyButtonInputs);
        FireMissle2.AddListener(state.Players[1].FireMissle);
        //#####################################
    }

    void Update()
    {
        #region debugging

        if (Input.GetKeyDown(KeyCode.O))
        {
            GameConnection.Instance.Save();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            GameConnection.Instance.Load();
        }

        if (Input.GetButtonDown("Cancel"))
        {
            MapLayoutInfo map = MapLayoutInfo.Instance;
            if (map != null) map.OpenMenu();
        }
        #endregion



        if (state == null)
        {
            ConnectInputs();
            return;
        }

        if (state.Players != null)
        {
            if (state.Players[0] != null && state.Players[0].IsInGame)
            {
                if (inputMovementKeyboard().magnitude > 0) Movement?.Invoke(inputMovementKeyboard());
                if (inputRotationKeyboard() != 0) Rotation?.Invoke(inputRotationKeyboard());

                buttonInputsKeyboard();
            }

            if (state.Players.Length > 1 && state.Players[1].IsInGame)
            {
                if (inputMovementController().magnitude > 0) Movement2?.Invoke(inputMovementController());
                if (inputRotationController() != 0) Rotation2?.Invoke(inputRotationController());

                buttonInputsController();
            }
        }
    }

    #region axis
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
    #endregion

    #region ButtonInputs

    void buttonInputsKeyboard()
    {
        if (Input.GetButton("Mark")) CombatInputs?.Invoke(INPUTS.Scan);
        if (Input.GetButton("CentreFocus")) CombatInputs?.Invoke(INPUTS.CentreFocus);

        if (Input.GetButtonDown("SwitchTargets")) CombatInputs?.Invoke(INPUTS.SwitchTargets);
        if (Input.GetButtonDown("SwitchMissles")) CombatInputs?.Invoke(INPUTS.SwitchMissle);
        if (Input.GetButtonDown("FallBack")) SwitchPositions?.Invoke(0);    //Playernumber


        //NumButtons
        if (Input.GetButtonDown("Missle1")) FireMissle?.Invoke(0);
        if (Input.GetButtonDown("Missle2")) FireMissle?.Invoke(1);
        if (Input.GetButtonDown("Missle3")) FireMissle?.Invoke(2);
    }

    void buttonInputsController()
    {
        if (Input.GetButton("Mark-2")) CombatInputs2?.Invoke(INPUTS.Scan);
        if (Input.GetButton("CentreFocus-2")) CombatInputs2?.Invoke(INPUTS.CentreFocus);

        if (Input.GetButtonDown("SwitchTargets-2")) CombatInputs2?.Invoke(INPUTS.SwitchTargets);
        if (Input.GetButtonDown("SwitchMissles-2")) CombatInputs2?.Invoke(INPUTS.SwitchMissle);
        if (Input.GetButtonDown("FallBack-2")) SwitchPositions2.Invoke(1);  //Playernumber


        //Dpad axis
        if (Input.GetAxisRaw("DPad X") < -0.1)     { if (DPadBool) FireMissle2?.Invoke(0); DPadBool = false; }
        else if (Input.GetAxisRaw("DPad X") > 0.1) { if (DPadBool) FireMissle2?.Invoke(1); DPadBool = false; }
        else if (Input.GetAxisRaw("DPad Y") > 0.1) { if (DPadBool) FireMissle2?.Invoke(2); DPadBool = false; }
        else DPadBool = true;

        //NumButtons
        if (Input.GetButtonDown("Missle1-2")) FireMissle2?.Invoke(0);
        if (Input.GetButtonDown("Missle2-2")) FireMissle2?.Invoke(1);
        if (Input.GetButtonDown("Missle3-2")) FireMissle2?.Invoke(2);
    }

    #endregion

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
    , SwitchMissle
    , Missle
    , Movement
    , Rotation
    , CentreFocus
    , None
}
