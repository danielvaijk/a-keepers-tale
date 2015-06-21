using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour
{
    public float playerHealth;
    public float playerHunger;
    public float playerThirst;
    public float playerTemperature;
    // public float playerSanity;

    public float maxHealth;
    public float maxHunger;
    public float maxThirst;

    public List<float> hungerRates;
    public List<float> thirstRates;
    public List<float> temperatureRates;

    private string healthStatus = string.Empty;
    private string hungerStatus = string.Empty;
    private string thirstStatus = string.Empty;

    //private string temperatureStatus = string.Empty;

    // Used to calculate temperature difference damage output.
    private float inicialTemperature = 0.0f;

    private void Start ()
    {
        inicialTemperature = playerTemperature;
    }

    private void Update ()
    {
        if (playerHealth <= 0)
            Debug.Log("You are Dead.");

        if (Mathf.Abs(inicialTemperature - playerTemperature) > 2)
            playerHealth -= Mathf.Abs((inicialTemperature - playerTemperature) / 20 * Time.deltaTime);

        if (Mathf.Abs(maxHunger - playerHunger) > 50)
            playerHealth -= Mathf.Abs((maxHunger - playerHunger) / 20 * Time.deltaTime);

        foreach (float rate in hungerRates)
            playerHunger += rate * Time.deltaTime;

        foreach (float rate in thirstRates)
            playerThirst += rate * Time.deltaTime;

        foreach (float rate in temperatureRates)
            playerTemperature += rate * Time.deltaTime;

        HealthStatus((playerHealth / maxHealth) * 100);
        HungerStatus((playerHunger / maxHunger) * 100);
        ThirstStatus((playerThirst / maxThirst) * 100);
    }

    private void OnGUI ()
    {
        GUILayout.Label("Health: " + healthStatus);
        GUILayout.Label("Hunger: " + hungerStatus);
        GUILayout.Label("Thirst: " + thirstStatus);
    }

    private void HealthStatus (float healthPercentage)
    {
        if (ValueBetween(healthPercentage, 40, 60))
        {
            healthStatus = "Injured";
        }
        else if (ValueBetween(healthPercentage, 20, 40))
        {
            healthStatus = "Baddly Injured";
        }
        else if (ValueBetween(healthPercentage, 0, 20))
        {
            healthStatus = "Severely Injured";
        }
        else if (healthPercentage > 60)
        {
            healthStatus = "OK";
        }
    }

    private void HungerStatus (float hungerPercentange)
    {
        if (ValueBetween(hungerPercentange, 40, 60))
        {
            hungerStatus = "Hungry";
        }
        else if (ValueBetween(hungerPercentange, 20, 40))
        {
            hungerStatus = "Really Hungry";
        }
        else if (ValueBetween(hungerPercentange, 0, 20))
        {
            hungerStatus = "Starving";
        }
        else if (hungerPercentange > 60)
        {
            hungerStatus = "OK";
        }
    }

    private void ThirstStatus (float thirstPercentange)
    {
        if (ValueBetween(thirstPercentange, 40, 60))
        {
            thirstStatus = "Thirsty";
        }
        else if (ValueBetween(thirstPercentange, 20, 40))
        {
            thirstStatus = "Really Thirsty";
        }
        else if (ValueBetween(thirstPercentange, 0, 20))
        {
            thirstStatus = "Dehidrated";
        }
        else if (thirstPercentange > 60)
        {
            thirstStatus = "OK";
        }
    }

    private bool ValueBetween (float value, float min, float max)
    {
        return value <= max && value >= min;
    }
}