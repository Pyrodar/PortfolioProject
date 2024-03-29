﻿using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Used to relay editorbound UnityEvents to the components of this prefab
/// </summary>
public class PrefabEventRelay : MonoBehaviour
{
    [SerializeField] UnityEvent StartPathFollow;
    [SerializeField] UnityEvent ActivateEnemy;
    [SerializeField] UnityEvent Other;


    public void OnStartFollow()
    {
        StartPathFollow?.Invoke();
    }

    public void OnActivateEnemy()
    {
        ActivateEnemy?.Invoke();
    }

    public void OnOtherEvent()
    {
        Other?.Invoke();
    }
}
