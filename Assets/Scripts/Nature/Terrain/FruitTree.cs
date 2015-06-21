using UnityEngine;
using System.Collections;

public class FruitTree : MonoBehaviour
{
    public int fruitAmount;

    public float bushHealth;
    public float fruitHungerReplenish;
    public float interactTime;

    private void Interacting ()
    {
        Transform playerCamera = GameObject.Find("Player").transform.FindChild("Player Camera");

        PlayerInteraction interaction = playerCamera.GetComponent<PlayerInteraction>();

        interaction.interactTime = interactTime;
        interaction.interactLabel = "Searching...";
    }

    private void Interacted ()
    {
        Transform playerCamera = GameObject.Find("Player").transform.FindChild("Player Camera");

        PlayerInteraction interaction = playerCamera.GetComponent<PlayerInteraction>();

        if (fruitAmount > 0)
        {
            interaction.messageLabel = string.Format("Found {0} fruit.", fruitAmount);

            PlayerStats playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();

            playerStats.playerHunger += (fruitHungerReplenish * fruitAmount);

            fruitAmount = 0;
        }
        else
        {
            interaction.messageLabel = "No fruit was found.";
        }
    }
}