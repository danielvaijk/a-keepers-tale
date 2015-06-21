using UnityEngine;
using System.Collections;

public class NatureResource : MonoBehaviour
{
    public int resourceAmount;

    public float resourceHealth;

    public string resourceDropName;

    public bool available;

    private GameObject dropObject = null;

    private Rigidbody thisRigidbody = null;

    private void Start ()
    {
        dropObject = Resources.Load<GameObject>(resourceDropName);
        thisRigidbody = GetComponent<Rigidbody>();

        if (thisRigidbody != null)
            thisRigidbody.isKinematic = true;
    }

    private void Update()
    {
        if (resourceHealth <= 0f && dropObject != null)
        {
            for (int i = 1; i <= (resourceAmount * 2) - 1; i += 2)
            {
                float positionOffset = dropObject.transform.localScale.y * i;

                GameObject clone = (GameObject)Instantiate(dropObject,
                                                           transform.position + transform.up * positionOffset,
                                                           transform.rotation);

                clone.name = dropObject.name;
            }

            Destroy(gameObject);
        }
    }

    private void TakeDamage (float damage)
    {
        if (available) 
            resourceHealth -= damage;
    }
}