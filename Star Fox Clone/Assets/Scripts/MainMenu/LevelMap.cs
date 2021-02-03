using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelMap : MonoBehaviour
{
    [SerializeField] Transform[] LevelLocations;
    [SerializeField] Image Map;
    [SerializeField] Canvas MyCanvas;

    private void Start()
    {
        transform.localPosition = Vector3.zero;  //starting in the center
        //GetComponent<RectTransform>().sizeDelta = Map.rectTransform.sizeDelta * MyCanvas.scaleFactor;

        //Adjusting collider to the size of the Map
        GetComponent<BoxCollider2D>().offset = Vector3.zero;
        GetComponent<BoxCollider2D>().size = (Map.rectTransform.sizeDelta * MyCanvas.scaleFactor) + new Vector2(Screen.width, Screen.height);  //seems to be smaller than the editor says it is
    }

    private void OnMouseDrag()
    {
        //Debug.Log("Dragging");


        Vector3 value = Vector3.zero;
        value.x += Input.GetAxis("Mouse X");
        value.y += Input.GetAxis("Mouse Y");

        transform.localPosition += value * MyCanvas.scaleFactor * 5; //no Idea why I need to multiply it with 5


        checkBounds();
    }

    void checkBounds()
    {
        Vector2 maxSize = (Map.rectTransform.sizeDelta / 2) - new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 maxPos = transform.localPosition;

        if (maxPos.x > maxSize.x) maxPos.x = maxSize.x;
        if (maxPos.x < -maxSize.x) maxPos.x = -maxSize.x;
        
        if (maxPos.y > maxSize.y) maxPos.y = maxSize.y;
        if (maxPos.y < -maxSize.y) maxPos.y = -maxSize.y;

        transform.localPosition = maxPos;
    }

    public Transform getLevelPosition(int level)
    {
        if (level < LevelLocations.Length)
        {
            return LevelLocations[level];
        }

        return null;
    }
}
