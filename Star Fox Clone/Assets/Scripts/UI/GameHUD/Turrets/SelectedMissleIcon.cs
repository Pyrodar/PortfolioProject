using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedMissleIcon : MonoBehaviour
{
    [SerializeField] Image Icon;
    [SerializeField] Text MissleName;

    public void UpdateMissle(MissleData data)
    {
        //Icon.sprite = data.missleIcon;
        MissleName.text = data.name;
    }
}
