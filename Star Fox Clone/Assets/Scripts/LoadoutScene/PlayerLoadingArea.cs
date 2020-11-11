using UnityEngine;

public class PlayerLoadingArea : MonoBehaviour
{
    [SerializeField] Player playerObject;
    Player player;

    void Start()
    {
        if (Player.Instance == null)
        {
            player = Instantiate(playerObject);//Load Player
        }
        else
        {
            player = Player.Instance;
        }

        player.transform.position = this.transform.position;

    }

    /*
    void FixedUpdate()
    {
        //if mouse button (left hand side) pressed instantiate a raycast
        if (Input.GetMouseButtonDown(0))
        {
            //create a ray cast and set it to the mouses cursor position in game
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500))
            {
                //draw invisible ray cast/vector
                Debug.DrawLine(ray.origin, hit.point);
                //log hit area to the console
                Debug.Log(hit.collider.name);

            }
        }
    }
    */
}
