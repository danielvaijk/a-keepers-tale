using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour
{
    public float thirstReplenish;
    public float interactTime;

    private PlayerStats playerStats;
    private PlayerInteraction playerInteraction;

    private void Start ()
    {
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        playerInteraction = GameObject.Find("Player").transform.FindChild("Player Camera").
                            GetComponent<PlayerInteraction>();
    }

    private void Interacting ()
    {
        playerInteraction.interactTime = interactTime;
        playerInteraction.interactLabel = "Drinking...";
    }

    private void Interacted ()
    {
        playerStats.playerThirst += thirstReplenish;
    }
}