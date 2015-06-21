using UnityEngine;

using System.Collections;
using System.Collections.Generic;

// Used OnMouseDown() OnMouseDrag() and etc for GUI mouse stuff.

public class Inventory : MonoBehaviour
{
    public int verticalSlots;
    public int horizontalSlots;

    public int maxStackAmount;

    public float slotSize;

    [HideInInspector]
    public bool showInventory;

    [HideInInspector]
    public bool userHasOption = false;

    public Texture2D emptySlotTexture;

    public List<Slot> inventory;

    private bool dragging = false;

    private GameObject equipedObject = null;

    private Vector2 windowPosition = Vector2.zero;
    private Vector2 windowSize = Vector2.zero;
    private Vector2 clickMousePosition = Vector2.zero;

    private Slot dragSlot = new Slot();
    private Slot receiver = new Slot();

    private PlayerInteraction playerInteraction = null;

    // Represents an Inventory Slot.
    public struct Slot
    {
        public int id; // Identifier used to find this Slot.
        public int itemID;

        // Returns the actual amount of Items inside this Slot.
        public int stacks
        {
            get { return slotObjects.Count; }
        }

        // Returns if we have more then one Item inside this Slot.
        public bool stackableSlot
        {
            get { return slotObjects.Count > 1; }
        }

        public List<GameObject> slotObjects;

        public Texture2D slotTexture;

        public Rect originalRect;
        public Rect currentRect;

        // Returns a bool indicating if this slots original Rect contains <cursorPosition>.
        public bool Contains (Vector2 cursorPosition)
        {
            return this.originalRect.Contains(cursorPosition);
        }

        // Returns a bool indicating if this slot is outside the GUI Windows Rect.
        public bool OutsideWindow (Vector2 windowPosition, Vector2 windowSize)
        {
            Rect windowRect = new Rect(windowPosition.x, windowPosition.y, windowSize.x, windowSize.y);

            return !windowRect.Contains(Event.current.mousePosition);
        }
    }

    private void Start ()
    {
        // Find our PlayerInteraction MonoBehaviour on our Player Camera GameObject.
        playerInteraction = transform.Find("Player Camera").GetComponent<PlayerInteraction>();

        // Inicialize our <inventory> List with a set <inventorySize> capacity.
        inventory = new List<Slot>(horizontalSlots * verticalSlots);

        // Calculate the <windowSize> based on the <slotSize> and the amount of slots.
        windowSize = new Vector2(horizontalSlots * slotSize, verticalSlots * slotSize);

        // Calculate the <windowPosition> based on <windowSize>.
        windowPosition = new Vector2((Screen.width - windowSize.x) / 2,
                                        (Screen.height - windowSize.y) / 2);

        // Our current (X,Y) positions inside our GUI Window.
        float currentX = windowPosition.x;
        float currentY = windowPosition.y;

        // Add an empty slot for each available slot in the <inventory>.
        for (int i = 0; i < inventory.Capacity; i++)
        {
            Slot emptySlot = new Slot();

            emptySlot.id = i;
            emptySlot.itemID = 0;
            emptySlot.slotObjects = new List<GameObject>(maxStackAmount);
            emptySlot.slotTexture = emptySlotTexture;
            emptySlot.originalRect = new Rect(currentX, currentY, slotSize, slotSize);
            emptySlot.currentRect = emptySlot.originalRect;

            // Move horizontally by <slotSize>.
            currentX += slotSize;

            // Check if we can fit another Slot horizontally.
            if (currentX + slotSize > windowPosition.x + windowSize.x)
            {
                // If we cannot fit another slot horizontally then move vertically and
                // reset our horizontal position.
                currentX = windowPosition.x;
                currentY += slotSize;
            }

            inventory.Add(emptySlot);
        }
    }

    private void Update ()
    {
        // Show or hide this inventory.
        if (Input.GetButtonDown("Inventory"))
        {
            showInventory = !showInventory;

            Cursor.visible = showInventory;

            GetComponent<MouseRotation>().enabled = !showInventory;
            transform.Find("Player Camera").GetComponent<MouseRotation>().enabled = !showInventory;
        }
    }

    private void OnGUI ()
    {
        if (showInventory)
        {
            // The Inventory's Header.
            GUI.Box(new Rect(windowPosition.x, windowPosition.y - 25,
                     windowSize.x, 25), "Inventory");

            // The Inventory's Window.
            GUI.Box(new Rect(windowPosition.x, windowPosition.y,
                             windowSize.x, windowSize.y), "");

            foreach (Slot slot in inventory)
            {
                if (slot.stackableSlot)
                {
                    // If this slot is stackable then draw a label counting the amount of
                    // stacks and disable the ability to equip this stackable item.

                    string stackLabel = string.Format("<b><size=20>{0}</size></b>", slot.stacks);

                    // Only show the stack amount label is we have more then 1 item stacked.
                    if (slot.stacks > 0)
                        GUI.Label(slot.currentRect, stackLabel);

                    // Stackable items are not equipable.
                    GUI.Button(slot.currentRect, slot.slotTexture);
                }
                else
                {
                    // If we click this Slot Button then equip the GameObject it holds.
                    if (GUI.Button(slot.currentRect, slot.slotTexture) && !userHasOption)
                    {
                        if (slot.slotObjects.Count > 0)
                        {
                            Transform item = this.transform.Find("Player Camera").
                                             Find(slot.slotObjects[0].name + " Usable");

                            // If the Item GameObject exists inside our Player Camera
                            // then equip it.
                            if (item != null)
                            {
                                if (equipedObject != null)
                                {
                                    equipedObject.SetActive(false);
                                    equipedObject = null;
                                }

                                equipedObject = item.gameObject;
                                equipedObject.SetActive(true);
                            }
                        }
                    }
                }

                // If the mouse is inside this Slot then watch for dragging.
                if (slot.Contains(Event.current.mousePosition) && !dragging)
                {
                        // If we click with the left mouse button set its click screen position;
                        if (Input.GetMouseButtonDown(0))
                        {
                            clickMousePosition = Event.current.mousePosition;
                        }

                        // If we clicked then moved our mouse that means we are dragging the slot.
                        if (Input.GetMouseButton(0) && Event.current.mousePosition != clickMousePosition)
                        {
                            // Prevent the Player from dragging empty slots.
                            if (slot.slotObjects.Count > 0)
                            {
                                dragSlot = slot;
                                dragging = true;
                            }
                        }
                }

                if (dragging)
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        // If we released our left mouse button and the Slots Rect is
                        // outside the Window, then drop the item it contained.
                        if (dragSlot.OutsideWindow(windowPosition, windowSize))
                        {
                            if (dragSlot.slotObjects.Count > 0)
                            {
                                Item item = null;

                                if (dragSlot.stackableSlot)
                                {
                                    // If we are a stackable Slot then we drop 1 of the stacked items.

                                    if (dragSlot.slotObjects.Count == 1)
                                    {
                                        // If we only have 1 GameObject left then drop it and restore this
                                        // slot to a normal empty slot.

                                        item = dragSlot.slotObjects[0].GetComponent<Item>();

                                        dragSlot = ResetSlot(dragSlot);
                                    }
                                    else
                                    {
                                        int lastItemIndex = dragSlot.slotObjects.Count - 1;

                                        item = dragSlot.slotObjects[lastItemIndex].GetComponent<Item>();

                                        dragSlot.slotObjects.RemoveAt(lastItemIndex);
                                    }
                                }
                                else
                                {
                                    // If the Slot we dragged out of the Window is not stackable
                                    // then just drop the item it contained.

                                    item = dragSlot.slotObjects[0].GetComponent<Item>();

                                    dragSlot = ResetSlot(dragSlot);

                                    if (equipedObject != null)
                                    {
                                        equipedObject.SetActive(false);
                                        equipedObject = null;
                                    }
                                }

                                item.DropItem();

                                // Since we are modifying this Slot inside the main foreach loop
                                // calling UpdateSlotData() would be extremelly slow due to a
                                // loop inside another loop.
                                inventory[slot.id] = slot;
                            }
                        }
                        else
                        {
                            // If we dropped the Item inside the Window, check what Slot we dropped
                            // it upon.

                            for (int i = 0; i < inventory.Count; i++)
                            {
                                // If this instance is the <dragSlot> itself then skip it.
                                if (inventory[i].id == dragSlot.id)
                                    continue;

                                // If we found a Slot that contains the mouse then exchange
                                // Slot information from the <dragSlot> to the <receiverSlot>
                                // and the <receiverSlot> to the <dragSlot>.
                                if (inventory[i].Contains(Event.current.mousePosition))
                                {
                                    receiver = inventory[i];
                                    userHasOption = true;
                                    break;
                                }
                            }
                        }

                        // If we got our left mouse button up return the <dragSlot>
                        // to its original position.
                        dragSlot.currentRect = dragSlot.originalRect;

                        // Update the <dragSlot>'s data.
                        UpdateSlotData(dragSlot);

                        // And since we released the left mouse button we are no longer dragging.
                        dragging = false;
                        break;
                    }

                    // If we are dragging then set the <dragSlot>'s position to be the current
                    // mouse cursor's position (Compensating for the <slotSize> offset).
                    dragSlot.currentRect.x = Event.current.mousePosition.x - 35;
                    dragSlot.currentRect.y = Event.current.mousePosition.y - 35;

                    // If the current slot being iterated is the <dragSlot> then update its info.
                    if (slot.id == dragSlot.id)
                    {
                        inventory[inventory.IndexOf(slot)] = dragSlot;
                    }
                }
            }

            if (userHasOption)
            {
                // Prompt the Player if he wants to create whatever the combination
                // of both items can create or just exchange Slots.

                if (receiver.slotObjects.Count == 0)
                {
                    if (dragSlot.stackableSlot && (dragSlot.stacks - 1) > 0)
                    {
                        receiver.itemID = dragSlot.itemID;
                        receiver.slotObjects.Add(dragSlot.slotObjects[dragSlot.slotObjects.Count - 1]);
                        receiver.slotTexture = dragSlot.slotTexture;

                        dragSlot.slotObjects.RemoveAt(dragSlot.slotObjects.Count - 1);

                        UpdateSlotData(receiver);
                        UpdateSlotData(dragSlot);
                    }
                    else
                    {
                        ExchangeSlots(dragSlot, receiver);
                    }

                    userHasOption = false;
                }
                else
                {
                    Rect button1Rect = new Rect(Screen.width - windowPosition.x,
                                                Screen.height - windowPosition.y - 120, 150, 40);

                    Rect button2Rect = new Rect(Screen.width - windowPosition.x,
                                                Screen.height - windowPosition.y - 80, 150, 40);

                    Rect button3Rect = new Rect(Screen.width - windowPosition.x,
                                                Screen.height - windowPosition.y - 40, 150, 40);

                    // GUI Button for combining 2 Slot items.
                    CraftingCombinations crafting = this.GetComponent<CraftingCombinations>();
                    string craftableItem = "";
                    int newItemID = 0;


                    foreach (CraftingCombinations.ItemCombination combination in crafting.itemCombinations)
                    {
                        int combinationSum = (combination.itemID1 * 10) + combination.itemID2;
                        int inventorySum = (dragSlot.itemID * 10) + receiver.itemID;

                        if (combinationSum == inventorySum)
                        {
                            craftableItem = combination.name;
                            newItemID = combinationSum;
                        }
                    }

                    if (newItemID != 0)
                    {
                        if (GUI.Button(button1Rect, "Craft: " + craftableItem))
                        {
                            GameObject craftedObject = Resources.Load<GameObject>(craftableItem);
                            GameObject craftedInstance = (GameObject)Instantiate(craftedObject);

                            craftedInstance.name = craftableItem;

                            Item craftedItem = craftedInstance.GetComponent<Item>();

                            if (craftedItem.placeable && !craftedItem.wasPlaced)
                            {
                                craftedInstance.GetComponent<PlaceableItem>().placeItem = true;
                            }
                            else
                            {
                                CraftItem(craftedInstance, newItemID);
                            }
                        }
                    }

                    if (receiver.itemID == dragSlot.itemID)
                    {
                        if (GUI.Button(button2Rect, "Stack Items") && receiver.stacks < maxStackAmount)
                        {
                            for (int i = 0; i < maxStackAmount; i++)
                            {
                                if (receiver.stacks < maxStackAmount && dragSlot.stacks > 0)
                                {
                                    int index = dragSlot.slotObjects.Count - 1;

                                    receiver.slotObjects.Add(dragSlot.slotObjects[index]);
                                    dragSlot.slotObjects.RemoveAt(index);
                                }
                                else if (dragSlot.stacks == 0)
                                {
                                    dragSlot = ResetSlot(dragSlot);
                                    break;
                                }
                            }

                            UpdateSlotData(dragSlot);
                            UpdateSlotData(receiver);

                            userHasOption = false;
                        }
                    }

                    // GUI Button for swaping Slots.
                    if (GUI.Button(button3Rect, "Swap Items"))
                    {
                        ExchangeSlots(dragSlot, receiver);
                        userHasOption = false;
                    }
                }
            }
        }
    }

    public void CraftItem (GameObject craftedObject, int newItemID)
    {
        Item craftedItem = craftedObject.GetComponent<Item>();

        // Remove the item from the <dragSlot>.
        if (dragSlot.slotObjects.Count > 0)
        {
            Destroy(dragSlot.slotObjects[dragSlot.slotObjects.Count - 1]);

            if (dragSlot.stacks > 1)
            {
                dragSlot.slotObjects.RemoveAt(dragSlot.slotObjects.Count - 1);
            }
            else
            {
                dragSlot = ResetSlot(dragSlot);
            }
        }

        // Remove the item from the <receiver> Slot.
        if (receiver.slotObjects.Count > 0)
        {
            Destroy(receiver.slotObjects[receiver.slotObjects.Count - 1]);

            if (receiver.stacks > 1)
            {
                receiver.slotObjects.RemoveAt(receiver.slotObjects.Count - 1);
            }
            else
            {
                receiver = ResetSlot(receiver);
            }

            if (!craftedItem.placeable)
            {
                craftedItem.PickupItem(this.transform);

                if (InventoryHasSpace())
                {
                    receiver.itemID = newItemID;
                    receiver.slotTexture = craftedItem.inventoryIcon;

                    if (receiver.slotObjects.Count > 0)
                        receiver.slotObjects = new List<GameObject>(maxStackAmount);

                    receiver.slotObjects.Add(craftedObject);
                }
                else
                {
                    craftedItem.DropItem();
                }
            }
        }

        UpdateSlotData(receiver);
        UpdateSlotData(dragSlot);

        userHasOption = false;
    }

    // Finds the <target> Slot inside the Inventory by using its id.
    private void UpdateSlotData (Slot target)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].id == target.id)
                inventory[i] = target;
        }
    }

    // Finds the next available empty Slot.
    private Slot FindSlot ()
    {
        foreach (Slot slot in inventory)
        {
            if (slot.slotObjects.Count == 0)
                return slot;
        }

        return new Slot();
    }

    // Finds the slot that contains a GameObject with the same
    // type as the <objectName>.
    private Slot FindSlot (int objectID)
    {
        foreach (Slot slot in inventory)
        {
            if (slot.itemID == objectID)
                return slot;
        }

        return new Slot();
    }

    // Exchanges information between the <sender> and <receiver> Slots.
    private void ExchangeSlots (Slot sender, Slot receiver)
    {
        // Check if the <sender> has any information to exchange, otherwise
        // there is no reason to exchange Nothing for Nothing.
        if (sender.slotObjects.Count > 0)
        {
            Slot oldSender = sender;

            // Change the <dragSlot>'s information to the ones the <receiver> Slot had.
            sender.itemID = receiver.itemID;
            sender.slotObjects = receiver.slotObjects;
            sender.slotTexture = receiver.slotTexture;

            // Change the <receiver>'s information to the ones the <dragSlot> had.
            receiver.itemID = oldSender.itemID;
            receiver.slotObjects = oldSender.slotObjects;
            receiver.slotTexture = oldSender.slotTexture;

            UpdateSlotData(sender);
            UpdateSlotData(receiver);
        }
    }

    public void AddItem (Item item)
    {
        if (!InventoryHasSpace())
            return;

        Slot editSlot = new Slot();

        // If this item is stackable then find a Slot with its similar
        // GameObject type and stack it, if there is none, just add it.
        if (item.stackable)
        {
            // Search for any stackable Slots with the same GameObject type as 
            // the one we want to add.
            Slot stackableSlot = FindSlot(item.itemID);

            if (stackableSlot.slotTexture != null && stackableSlot.stacks < maxStackAmount)
            {
                // If we found a stackable Slot with the same GameObject type as
                // the one we want to add, then add another stack to that slot.

                editSlot = FindSlot(item.itemID);

                // If this is the Slots first stack set it to stackable.
                editSlot.slotObjects.Add(playerInteraction.currentObject);
            }
            else
            {
                // If we do not already contain this GameObject type then just add it.

                // Find the next empty Slot.
                editSlot = FindSlot();

                if (editSlot.slotTexture != null)
                {
                    // Add the GameObject we were interacting with to the slot.
                    editSlot.slotObjects.Add(playerInteraction.currentObject);
                    editSlot.slotTexture = item.inventoryIcon;
                    editSlot.itemID = item.itemID;
                }
            }
        }
        else
        {
            // If this item is not stackable then just add it to an empty Slot.

            // Find the next empty Slot.
            editSlot = FindSlot();

            if (editSlot.slotTexture != null)
            {
                // Add the GameObject we were interacting with to the slot.
                editSlot.slotObjects.Add(playerInteraction.currentObject);
                editSlot.slotTexture = item.inventoryIcon;
                editSlot.itemID = item.itemID;
            }
        }

        // Pickup the item we were interacting with.
        item.PickupItem(this.transform);

        // Update the actual inventory Slot.
        UpdateSlotData(editSlot);
    }

    private Slot ResetSlot (Slot slot)
    {
        slot.itemID = 0;
        slot.slotObjects = new List<GameObject>(maxStackAmount);
        slot.slotTexture = emptySlotTexture;

        return slot;
    }

    public bool InventoryHasSpace ()
    {
        foreach (Slot slot in inventory)
        {
            if (slot.slotObjects.Count == 0)
                return true;
        }

        return false;
    }
}