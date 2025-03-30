using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EntityEmptyRoom : MonoBehaviour
{
    public Transform playerPosition;
    private Transform playersCurrentPosition;
    private JumpScares jumpScare;
    public float speed = 2.1f;
    void Start()
    {
        jumpScare = FindObjectOfType<JumpScares>();
    }

    void Update()
    {
        playersCurrentPosition = playerPosition;
        var moveTowardsPlayerStep = speed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, playersCurrentPosition.position, moveTowardsPlayerStep);

        if (Vector3.Distance(transform.position, playersCurrentPosition.position) < 0.001f)
        {
            jumpScare.TriggerJumpScare();
        }
    }
}
