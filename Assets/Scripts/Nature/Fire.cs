using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour
{
    public float warmthRadius;
    public float warmthRate;

    public float flickerRate;

    private int listIndex = 0;
    private int addType = 0;

    private float inicialLightIntensity = 0.0f;

    private Light fireLight;

    private PlayerStats playerStats;

    // Called when the script is loaded or when a inspector value has changed. (Editor-only).
    private void OnValidate ()
    {
        (this.GetComponent<Collider>() as SphereCollider).radius = warmthRadius;
    }

    // First method to be called when this script is inicialized.
    private void Start ()
    {
        fireLight = transform.Find("Fire Light").GetComponent<Light>();
        inicialLightIntensity = fireLight.intensity;
    }

    // Called every frame.
    private void Update ()
    {
        FlickerLight(0, inicialLightIntensity, flickerRate);
    }

    // Called when a Collider/Rigidbody enters this Trigger.
    private void OnTriggerEnter (Collider collider)
    {
        if (collider.tag == "Player")
        {
            playerStats = collider.GetComponent<PlayerStats>();
            listIndex = playerStats.temperatureRates.Count;
        }
    }

    private void OnTriggerStay (Collider collider)
    {
        if (collider.tag == "Player")
        {
            // Distance between this fire and the Player.
            float distance = Vector3.Distance(transform.position, collider.transform.position);

            if (distance <= warmthRadius)
            {
                // Calculate the warmth output based on the distance of the Player from the fire.
                float distancePercentage = (1 / distance) * 100;
                float currentValue = (warmthRate * distancePercentage) / 100;

                if (playerStats.temperatureRates.Count > listIndex)
                    playerStats.temperatureRates.RemoveAt(listIndex);

                playerStats.temperatureRates.Add(currentValue);

                listIndex = playerStats.temperatureRates.Count - 1;
            }
        }
    }

    private void OnTriggerExit (Collider collider)
    {
        if (collider.tag == "Player")
        {
            playerStats.temperatureRates.RemoveAt(listIndex);
            listIndex = 0;
        }
    }

    private void FlickerLight (float min, float max, float rate)
    {
        if (fireLight.intensity <= min)
            addType = +1;

        if (fireLight.intensity >= max)
            addType = -1;

        fireLight.intensity += addType * rate * Time.deltaTime;
    }
}