using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalBar : MonoBehaviour
{ 
    [SerializeField] private RectTransform rectTransform;

    float originalWidth;
    float maxValue;
    [SerializeField] float currentValue;
    public float CurrentValue
    {
        set
        {
            currentValue = Mathf.Clamp(value, 0, maxValue);
            UpdateView();
        }
    }

    void Awake()
    {
        originalWidth = rectTransform.sizeDelta.x;
    }

    public void Initialize(float max)
    {
        maxValue = max;
        currentValue = max;
    }

    private void UpdateView()
    {
        float width = (originalWidth / maxValue) * currentValue;

        rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
    }
}
