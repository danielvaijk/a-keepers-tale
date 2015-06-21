using UnityEngine;
using System.Collections;

public class WildAnimalAI : MonoBehaviour
{
    public AnimalAIState animalState = AnimalAIState.Roaming;

    public float health;

    public float roamingSpeed;
    public float fleeingSpeed;

    public float fieldOfView;
    public float visionRange;

    public float fleeingDistance;

    public Transform[] movementWaypoints;

    private float distanceToWaypoint = float.MaxValue;

    private bool canSeePlayer = false;

    private Transform currentWaypoint = null;
    private Transform player = null;

    private NavMeshAgent navMeshAgent = null;

    public enum AnimalAIState
    { 
        Idle, 
        Roaming, 
        Fleeing, 
        Chasing, 
        Scared, 
        Aggressive
    }

    private void Start ()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player").transform;
    }

    private void Update ()
    {
        if (health <= 0f)
        {
            // If this animal is dead then replace it with its dead lootable body.
            navMeshAgent.Stop();

            Destroy(gameObject);
        }
        else
        {
            Vector3 playerDirection = player.position - transform.position;
            Ray rayToPlayer = new Ray(transform.position, playerDirection);

            if (!Physics.Raycast(rayToPlayer, visionRange))
            {
                // I can only see the Player if there are no objects between us and he
                // is within my vision range.

                bool insideFOV = Vector3.Angle(playerDirection, transform.forward) < fieldOfView;
                bool insideRange = Vector3.Distance(transform.position, player.position) <= visionRange;

                canSeePlayer = insideFOV && insideRange;
            }

            // If we can see the Player run from him.
            if (canSeePlayer)
                animalState = AnimalAIState.Fleeing;

            if (animalState == AnimalAIState.Roaming)
            {
                // Move from waypoint to waypoint to caracterise a roaming movement.

                if (currentWaypoint == null)
                {
                    // If we have no waypoint selected, pick the nearest waypoint to start roaming.

                    float smallestDistance = float.MaxValue;
                    Transform closestWaypoint = null;

                    foreach (Transform waypoint in movementWaypoints)
                    {
                        float distance = Vector3.Distance(transform.position, waypoint.position);

                        if (distance < smallestDistance)
                        {
                            closestWaypoint = waypoint;
                            smallestDistance = distance;
                        }
                    }

                    currentWaypoint = closestWaypoint;

                    navMeshAgent.SetDestination(currentWaypoint.position);
                }
                else
                {
                    distanceToWaypoint = Vector3.Distance(transform.position, currentWaypoint.position);

                    navMeshAgent.speed = roamingSpeed;

                    if (distanceToWaypoint <= 0.5f)
                    {
                        // If we have already reached our waypoint, choose the next waypoint randomly.

                        currentWaypoint = movementWaypoints[Random.Range(0, movementWaypoints.Length - 1)];

                        navMeshAgent.SetDestination(currentWaypoint.position);
                    }
                }
            }
            else if (animalState == AnimalAIState.Fleeing || animalState == AnimalAIState.Scared)
            {
                // Flee until at a certain distance from the Player or from the point we where
                // scared from.

                Vector3 scarePosition = transform.position;

                // If we had a roaming waypoint, set it to null so we can reset it afterwards.
                if (currentWaypoint != null)
                {
                    currentWaypoint = null;
                    scarePosition = transform.position;
                }

                // Look away from the Player.
                Quaternion movementDirection = Quaternion.identity;

                if (animalState == AnimalAIState.Fleeing)
                {
                    movementDirection = Quaternion.LookRotation(transform.position - player.position);
                }
                else
                {
                    movementDirection = transform.rotation;
                }

                transform.rotation = Quaternion.Slerp(transform.rotation, movementDirection, Time.deltaTime);

                // The point in that we are going to move towards to get away from the Player.
                Vector3 movementPoint = transform.position + transform.forward * fleeingSpeed;

                navMeshAgent.speed = fleeingSpeed;
                navMeshAgent.SetDestination(movementPoint);

                float distance = float.MaxValue;

                if (animalState == AnimalAIState.Fleeing)
                {
                    distance = Vector3.Distance(transform.position, player.position);

                    if (distance >= fleeingDistance && !canSeePlayer)
                        animalState = AnimalAIState.Roaming;
                }
                else
                {
                    distance = Vector3.Distance(transform.position, scarePosition);

                    if (distance >= fleeingDistance)
                        animalState = AnimalAIState.Roaming;
                }
            }
        }
    }

    private void OnDrawGizmos ()
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < movementWaypoints.Length; i++)
        {
            Vector3 rayDirection = Vector3.zero;

            if (i + 1 == movementWaypoints.Length)
            {
                rayDirection = movementWaypoints[0].position - movementWaypoints[i].position;
            }
            else
            {
                rayDirection = movementWaypoints[i + 1].position - movementWaypoints[i].position;
            }

            Gizmos.DrawRay(movementWaypoints[i].position, rayDirection);
        }

        Vector3 rotatedFOVRay1 = Quaternion.AngleAxis(fieldOfView, Vector3.up) * transform.forward;
        Vector3 rotatedFOVRay2 = Quaternion.AngleAxis(-fieldOfView, Vector3.up) * transform.forward;

        Gizmos.DrawRay(transform.position, rotatedFOVRay1 * visionRange);
        Gizmos.DrawRay(transform.position, rotatedFOVRay2 * visionRange);
    }

    private void TakeDamage (float damageAmount)
    {
        health -= damageAmount;

        animalState = AnimalAIState.Scared;
    }
}