using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebuggingMenu : MonoBehaviour
{

    #region Debugging
    public void LevelUnlock(int value)
    {
        GameConnection.Instance.LevelUnlock(value);
    }

    public void AMSUnlock(int value)
    {
        GameConnection.Instance.AMSUnlock(value);
    }
    public void ATGUnlock(int value)
    {
        GameConnection.Instance.ATGUnlock(value);
    }
    public void MSLUnlock(int value)
    {
        GameConnection.Instance.MSLUnlock(value);
    }
    #endregion

}
