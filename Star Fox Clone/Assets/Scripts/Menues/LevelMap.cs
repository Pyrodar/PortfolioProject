using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelMap : MonoBehaviour
{
    [SerializeField] Transform[] levelLocations;
    [SerializeField] Image map;
    [SerializeField] Transform marker;

    [SerializeField] Canvas myCanvas;
    GraphicRaycaster m_Raycaster;
    [SerializeField] EventSystem m_EventSystem;

    float tolerance = 150f;
    float markerSpeed = 4000f;

    private void Start()
    {
        transform.localPosition = Vector3.zero;  //starting in the center

        //Adjusting collider to the size of the Map for dragging
        GetComponent<BoxCollider2D>().offset = Vector3.zero;
        GetComponent<BoxCollider2D>().size = (map.rectTransform.sizeDelta * myCanvas.scaleFactor) + new Vector2(Screen.width, Screen.height);  //seems to be smaller than the editor says it is

        if (myCanvas == null) myCanvas = GameObject.Find("MainMenuCanvas").GetComponent<Canvas>();
        m_Raycaster = myCanvas.GetComponent<GraphicRaycaster>();

        if (m_EventSystem == null) m_EventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    private void OnMouseDrag()
    {
        Vector3 value = Vector3.zero;
        value.x += Input.GetAxis("Mouse X");
        value.y += Input.GetAxis("Mouse Y");

        MoveMap(value * myCanvas.scaleFactor * 5); //no Idea why I need to multiply it with 5
    }

    private void Update()
    {
        #region Gamepad Input
        if (Mathf.Abs(Input.GetAxis("Right Joystick X")) > 0.05f || Mathf.Abs(Input.GetAxis("Right Joystick Y")) > 0.05f)
        {
            Vector3 value = Vector3.zero;
            value.x -= Input.GetAxis("Right Joystick X");
            value.y -= Input.GetAxis("Right Joystick Y");

            marker.position += value * Time.deltaTime * markerSpeed;
        }

        if (Input.GetButtonDown("MenuEnter-2") || Input.GetMouseButtonDown(0))
        {
            //Set up the new Pointer Event
            PointerEventData m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the game object
            m_PointerEventData.position = marker.transform.position - new Vector3(0, 0, -20);

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            if (results.Count > 0)
            {
                foreach (var obj in results)
                {
                    //Debug.Log("Hit " + obj.gameObject.name);
                    Button button = obj.gameObject.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.Invoke();
                        return;
                    }
                }
            }            
        }
        #endregion

        #region MouseInput
        if (Mathf.Abs(Input.GetAxis("Mouse X")) > 0.05f || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.05f)
        {
            marker.position = Input.mousePosition;
        }
        #endregion

        MarkerOnEdge();
    }

    void MarkerOnEdge()
    {
        Vector3 maxSize = new Vector3(Screen.width / 2, Screen.height / 2);
        Vector3 currentPos = marker.position - maxSize;

        Vector3 direction = new Vector3();

        if (currentPos.x > maxSize.x - tolerance)   direction.x += .1f;
        if (currentPos.x < -maxSize.x + tolerance)  direction.x -= .1f;

        if (currentPos.y > maxSize.y - tolerance)   direction.y += .1f;
        if (currentPos.y < -maxSize.y + tolerance)  direction.y -= .1f;

        #region CheckMarkerBounds
        if (currentPos.x > maxSize.x)               currentPos.x = maxSize.x;
        if (currentPos.x < -maxSize.x)              currentPos.x = -maxSize.x;

        if (currentPos.y > maxSize.y)               currentPos.y = maxSize.y;
        if (currentPos.y < -maxSize.y)              currentPos.y = -maxSize.y;

        marker.position = currentPos + maxSize;
        #endregion

        MoveMap(- direction * markerSpeed * Time.deltaTime);
    }

    void MoveMap(Vector3 direction)
    {
        transform.localPosition += direction;

        Vector3 val = checkMapBounds();

        marker.position -= new Vector3(direction.x * val.x, direction.y * val.y);
    }

    Vector3 checkMapBounds()
    {
        Vector3 retVal = new Vector3(1, 1);

        Vector2 maxSize = (map.rectTransform.sizeDelta / 2) - new Vector2(Screen.width / 2, Screen.height / 2);
        Vector2 currentPos = transform.localPosition;

        if (currentPos.x > maxSize.x)
        {
            retVal.x = 0;
            currentPos.x = maxSize.x;
        }
        if (currentPos.x < -maxSize.x)
        {
            retVal.x = 0;
            currentPos.x = -maxSize.x; 
        }

        if (currentPos.y > maxSize.y) 
        { 
            retVal.y = 0;
            currentPos.y = maxSize.y; 
        }
        if (currentPos.y < -maxSize.y)
        {
            retVal.y = 0;
            currentPos.y = -maxSize.y;
        }

        transform.localPosition = currentPos;

        return retVal;
    }

    public Transform getLevelPosition(int level)
    {
        if (level < levelLocations.Length)
        {
            return levelLocations[level];
        }

        return null;
    }
}
