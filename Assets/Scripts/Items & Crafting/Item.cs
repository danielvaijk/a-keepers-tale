using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour
{
    public int itemID;

    public float interactTime;

    public bool stackable;
    public bool placeable;

    [HideInInspector]
    public bool wasPlaced;

    public Texture2D inventoryIcon;

    private Renderer thisRenderer = null;
    private Collider thisCollider = null;

    private Inventory playerInventory = null;

    private void Start ()
    {
        thisRenderer = GetComponent<Renderer>();
        thisCollider = GetComponent<Collider>();

        playerInventory = GameObject.Find("Player").GetComponent<Inventory>();
    }

    public void PickupItem (Transform player)
    {
        // Set our parent as the Player's Camera.
        transform.parent = player.Find("Player Camera").Find("Inventory Items");

        if (thisRenderer == null)
        {
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).GetComponent<Renderer>().enabled = false;
        }
        else
        {
            thisRenderer.enabled = false;
        }

        thisCollider.isTrigger = true;
    }

    public void DropItem ()
    {
        transform.position = transform.parent.position;
        transform.rotation = transform.parent.rotation;
        transform.parent = null;

        if (thisRenderer == null)
        {
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).GetComponent<Renderer>().enabled = true;
        }
        else
        {
            thisRenderer.enabled = true;
        }

        thisCollider.isTrigger = false;
        GetComponent<Rigidbody>().AddForce(transform.forward * 40, ForceMode.Force);
    }

    private void Interacting ()
    {
        if (interactTime <= 0)
            Debug.LogWarning("Bug Warning: <interactTime> has to be bigger than 0");

        if (!playerInventory.InventoryHasSpace())
            return;

        GameObject playerCamera = playerInventory.transform.FindChild("Player Camera").gameObject;

        PlayerInteraction interaction = playerCamera.GetComponent<PlayerInteraction>();

        interaction.interactTime = interactTime;
        interaction.interactLabel = "Picking up...";
    }

    private void Interacted ()
    {
        playerInventory.AddItem(this);
    }
}