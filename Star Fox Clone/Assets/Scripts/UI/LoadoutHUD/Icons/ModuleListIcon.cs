using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleListIcon : MonoBehaviour, IManeuverableListEntry
{
    Image icon;
    void Start()
    {
        icon = GetComponent<Image>();
    }

    public void SelectEntry()
    {
        icon.color = Color.red;
    }
    
    public void DeselectEntry()
    {
        icon.color = Color.yellow;
    }

    public void MarkEntry()
    {
        icon.color = Color.yellow;
    }

    public void UnmarkEntry()
    {
        icon.color = Color.white;
    }

}
