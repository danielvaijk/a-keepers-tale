using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioClip))]

public class PlayerController : MonoBehaviour
{
    public float walkingSpeed;
    public float runningSpeed;

    [HideInInspector]
    public bool isGrounded;

    public AudioClip[] stepSounds;

    private float stepTimer = 0f;

    private void Update ()
    {
        float currentSpeed = Input.GetButton("Sprint") ? runningSpeed : walkingSpeed;
        float horizontalSpeed = Input.GetAxis("Horizontal");
        float verticalSpeed = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalSpeed, 0.0f, verticalSpeed);

        moveDirection = transform.TransformDirection(moveDirection);

        transform.position += moveDirection * currentSpeed * Time.deltaTime;

        if (moveDirection.magnitude > 0.01f)
        {
            if (stepTimer > 1f / currentSpeed)
            {
                this.GetComponent<AudioSource>().volume = Random.Range(0.4f, 0.6f);
                this.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.2f);
                this.GetComponent<AudioSource>().PlayOneShot(stepSounds[Random.Range(0, stepSounds.Length - 1)]);

                stepTimer = 0f;
            }
            else
            {
                stepTimer += Time.deltaTime;
            }
        }
        else
        {
            stepTimer = 0f;
        }

        Vector3 rayCenterOffset = this.transform.position + Vector3.down * this.transform.localScale.y;

        Ray groundRay = new Ray(rayCenterOffset, Vector3.down);

        RaycastHit hit;

        if (Physics.Raycast(groundRay, out hit, Mathf.Infinity))
        {
            float distance = Vector3.Distance(rayCenterOffset, hit.point);

            isGrounded = (distance <= 0.2f);
        }
        else
        {
            isGrounded = false;
        }
    }
}