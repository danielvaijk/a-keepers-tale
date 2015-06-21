using UnityEngine;
using System.Collections;

public class PlaceableItem : MonoBehaviour
{
    public float maxBuildDistance;

    [HideInInspector]
    public bool placeItem;

    private bool isColliding = false;

    private Transform player = null;
    private Transform playerCamera = null;

    private RaycastHit hit;

    private Item item = null;

    private void Start ()
    {
        player = GameObject.Find("Player").transform;
        playerCamera = player.Find("Player Camera");
        item = this.GetComponent<Item>();
    }

    private void FixedUpdate ()
    {
        if (placeItem && !item.wasPlaced)
        {
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);

            if (Physics.Raycast(ray, out hit, maxBuildDistance))
            {
                transform.position = new Vector3
                                     (
                                        hit.point.x + hit.normal.x / 3, 
                                        hit.point.y + transform.localScale.y / 2, 
                                        hit.point.z + hit.normal.z / 3
                                     );

                if (Input.GetMouseButtonDown(0) && !isColliding)
                {
                    player.GetComponent<Inventory>().CraftItem(this.gameObject, item.itemID);
                    item.wasPlaced = true;
                    gameObject.layer = 0;
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                player.GetComponent<Inventory>().userHasOption = false;
                Destroy(this.gameObject);
            }
        }
    }

    private void OnGUI ()
    {
        if (placeItem && !item.wasPlaced)
        {
            Rect labelRect = new Rect(Screen.width / 2 - 100, Screen.height - 80, 200, 40);

            GUI.Label(labelRect, "Left mouse button - Place object");

            Rect labelRect2 = new Rect(Screen.width / 2 - 100, Screen.height - 40, 200, 40);

            GUI.Label(labelRect2, "Right mouse button - Cancel");
        }
    }
}