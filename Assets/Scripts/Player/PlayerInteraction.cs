using UnityEngine;
using System.Collections;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance;
    public float messageTime;

    [HideInInspector]
    public float interactTime = float.MaxValue;

    [HideInInspector]
    public string interactLabel;

    [HideInInspector]
    public string messageLabel;

    [HideInInspector]
    public GameObject currentObject;

    public Texture2D emptyBarTexture;
    public Texture2D fullBarTexture;

    public KeyCode interactButton;

    private float interactTimer = 0f;
    private float messageTimer = 0f;

    private bool pressedKey = false;

    private RaycastHit hit;

    private void Update ()
    {
        // Create a new Ray from our position pointing forward.
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            float hitDistance = Vector3.Distance(transform.position, hit.point);

            if (hitDistance <= interactDistance)
            {
                currentObject = hit.transform.gameObject;

                if (Input.GetMouseButton(2))
                {
                    // FINISH DRAG OBJECTS.

                    Vector3 dragVector = new Vector3
                                             (
                                                 Input.mousePosition.x, 
                                                 Input.mousePosition.y, 
                                                 hitDistance
                                             );

                    Camera playerCamera = GetComponent<Camera>();

                    hit.transform.position = playerCamera.ScreenToWorldPoint(dragVector);
                }
                else
                {

                    if (Input.GetKeyDown(interactButton))
                        pressedKey = true;

                    if (Input.GetKey(interactButton) && pressedKey)
                    {
                        currentObject.SendMessage("Interacting", SendMessageOptions.DontRequireReceiver);

                        if (interactTimer >= interactTime)
                        {
                            currentObject.SendMessage("Interacted", SendMessageOptions.DontRequireReceiver);

                            interactTimer = 0f;
                            pressedKey = false;
                        }
                        else
                        {
                            interactTimer += Time.deltaTime;
                        }
                    }
                    else
                    {
                        currentObject = null;
                        interactTimer = 0f;
                        pressedKey = false;
                    }
                }
            }
            else
            {
                currentObject = null;
                interactTimer = 0f;
                pressedKey = false;
            }
        }
        else
        {
            currentObject = null;
            interactTimer = 0f;
            pressedKey = false;
        }

        if (messageLabel != string.Empty)
        {
            if (messageTime > messageTimer)
            {
                messageTimer += Time.deltaTime;
            }
            else
            {
                messageTimer = 0f;
                messageLabel = string.Empty;
            }
        }
    }

    private void OnGUI ()
    {
        Rect messageLabelRect = new Rect
                                (
                                    Screen.width / 2 - messageLabel.Length,
                                    Screen.height - 70,
                                    messageLabel.Length * 10,
                                    50
                                );

        GUI.Label(messageLabelRect, messageLabel);

        if (currentObject != null && Input.GetKey(interactButton))
        {
            Rect interactLabelRect = new Rect
                                     (
                                         Screen.width / 2 - 100 + (interactLabel.Length * 5),
                                         Screen.height - 60,
                                         interactLabel.Length * 10,
                                         50
                                     );

            GUI.Label(interactLabelRect, interactLabel);

            Rect barRect1 = new Rect(Screen.width / 2 - 100, Screen.height - 40, 200, 10);
            Rect barRect2 = new Rect(Screen.width / 2 - 100, Screen.height - 40, (interactTimer / interactTime) * 200, 10);

            GUI.DrawTexture(barRect1, emptyBarTexture, ScaleMode.StretchToFill);
            GUI.DrawTexture(barRect2, fullBarTexture, ScaleMode.StretchToFill);
        }
    }
}