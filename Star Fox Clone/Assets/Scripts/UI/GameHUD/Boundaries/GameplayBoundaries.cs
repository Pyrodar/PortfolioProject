using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayBoundaries : MonoBehaviour
{
    [SerializeField] SpriteRenderer TopBorder;
    [SerializeField] SpriteRenderer LeftBorder;
    [SerializeField] SpriteRenderer RightBorder;
    [SerializeField] SpriteRenderer BottomBorder;


    public void SetBorderOpacity(GameplayBorder border, float opacity)
    {
        //Debug.Log($"Setting {border.ToString()} Border to: {opacity}");

        switch (border)
        {
            case GameplayBorder.TOP:
                TopBorder.color = new Color(1f, 1f, 1f, opacity);
                break;
            case GameplayBorder.LEFT:
                LeftBorder.color = new Color(1f, 1f, 1f, opacity);
                break;
            case GameplayBorder.RIGHT:
                RightBorder.color = new Color(1f, 1f, 1f, opacity);
                break;
            case GameplayBorder.BOTTOM:
                BottomBorder.color = new Color(1f, 1f, 1f, opacity);
                break;
            default:
                break;
        }
    }

    public enum GameplayBorder
    {
        TOP,
        LEFT,
        RIGHT,
        BOTTOM
    }
}
