using UnityEngine;
using System.Collections;

public class RotationDelay : MonoBehaviour
{
    public float delayAmount;
    public float maxDelayAmount;
    public float delaySmoothAmount;

    private Vector3 localPosition = Vector3.zero;

    private void Start ()
    {
        localPosition = transform.localPosition;
    }

    private void Update ()
    {
        float delayX = -Input.GetAxis("Mouse X") * delayAmount;
        float delayY = -Input.GetAxis("Mouse Y") * delayAmount;

        delayX = Mathf.Clamp(delayX, -maxDelayAmount, maxDelayAmount);
        delayY = Mathf.Clamp(delayY, -maxDelayAmount, maxDelayAmount);

        Vector3 delayedPosition = new Vector3
                                      (
                                          localPosition.x + delayX,
                                          localPosition.y + delayY,
                                          localPosition.z
                                      );

        transform.localPosition = Vector3.Lerp
                                  (
                                      transform.localPosition, 
                                      delayedPosition, 
                                      Time.deltaTime * delaySmoothAmount
                                  );
    }
}