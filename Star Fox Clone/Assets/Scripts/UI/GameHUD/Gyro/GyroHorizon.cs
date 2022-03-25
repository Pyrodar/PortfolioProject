using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GyroHorizon : MonoBehaviour
{
    [SerializeField] RectTransform ZRotator;
    [SerializeField] RectTransform RotationXContent;
    //[SerializeField] RectTransform RotationXDirection;
    [SerializeField] RectTransform RotationXNumbers;

    public void displayRotation(Quaternion playerZRotation, Vector3 playerXYOffset)
    {
        //Debug.Log("CurrentRotation: " + playerRotation);

        ZRotator.eulerAngles = new Vector3(ZRotator.eulerAngles.x, ZRotator.eulerAngles.y, playerZRotation.eulerAngles.z);
        RotationXContent.anchoredPosition = new Vector3(-playerXYOffset.x, -playerXYOffset.y, 0) * 10;

        float relativeYOffset = (Mathf.Cos(playerZRotation.eulerAngles.z * Mathf.Deg2Rad) * playerXYOffset.y) - (Mathf.Sin(playerZRotation.eulerAngles.z * Mathf.Deg2Rad) * playerXYOffset.x);

        RotationXNumbers.anchoredPosition = (new Vector3(0, -relativeYOffset, 0) * 10);    //is in relation to current UI rotation so 0 points at GameplayPlane horizon and gameplay plane forward vector
    }
}
