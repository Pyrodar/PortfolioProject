using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBaseClass : MonoBehaviour
{
    //Currently only used to differ between InGameHUD and LoadoutHUD
    protected UIType uiType;
    public UIType UIType { get { return uiType; } }
}

public enum UIType
{
    InGame,
    Loadout
}
