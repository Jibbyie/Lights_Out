using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public AudioSource footstep;

    public float speed = 2f;
    public float gravity = -9.81f;
    public float footstepDelay = 0.05f; // Time between footstep sounds

    private Vector3 velocity;
    private bool canPlayFootstep = true; // Prevents spamming footsteps

    void Update()
    {
        Move();
    }

    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (move.magnitude > 0) 
        {

            if (canPlayFootstep && !footstep.isPlaying) // Prevent overlapping sounds
            {
                StartCoroutine(PlayFootstep());
            }
        }

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    IEnumerator PlayFootstep()
    {
        canPlayFootstep = false; // Prevents overlapping sounds
        footstep.Play();
        yield return new WaitForSeconds(footstepDelay);
        canPlayFootstep = true; // Allows the next footstep to play
    }
}

