using UnityEngine;
using UnityEngine.UI;

public class VerticalBar : MonoBehaviour
{ 
    //using Unitys fill method for unscalable sprites

    [SerializeField] private Image fill;

    float originalHeight;
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
        originalHeight = fill.fillAmount;
    }

    public void Initialize(float max)
    {
        maxValue = max;
        currentValue = max;
    }

    public void SetCurrentValue(float newValue)
    {
        currentValue = Mathf.Clamp(newValue, 0, maxValue);
        UpdateView();
    }

    private void UpdateView()
    {
        fill.fillAmount = (originalHeight / maxValue) * currentValue;
    }
}
