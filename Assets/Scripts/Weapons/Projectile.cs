using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    public float destroyDelay;

    [HideInInspector]
    public float damageAmount;

    private bool hasHit = false;

    private Transform projectileTip = null;

    private RaycastHit hit;

    private void Start ()
    {
        foreach (Transform child in this.transform)
        {
            if (child.name.Contains("Tip"))
            {
                projectileTip = child;
                break;
            }
        }
    }

    private void Update ()
    {
        Ray ray = new Ray(projectileTip.position, projectileTip.forward);

        if (Physics.Raycast(ray, out hit, 10f))
        {
            float hitDistance = Vector3.Distance(projectileTip.position, hit.point);

            if (!hasHit && hitDistance <= 1f)
            {
                // Disable the effects of physics on this projectile and all its children.
                foreach (Transform child in transform)
                    child.GetComponent<Rigidbody>().isKinematic = true;

                // Adjust penetration depth amount.
                transform.position -= transform.up / 1.5f;

                // Apply damage to the hit object (If it can receive any).
                hit.transform.SendMessage("TakeDamage", damageAmount, SendMessageOptions.DontRequireReceiver);

                Destroy(gameObject, destroyDelay);

                hasHit = true;
            }
        }

        Debug.DrawRay(projectileTip.position, projectileTip.forward * 10f, Color.red);
    }
}