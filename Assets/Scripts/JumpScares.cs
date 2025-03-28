using UnityEngine;

public class JumpScares : MonoBehaviour
{
    public AudioSource jumpScareSFX; 

    public void TriggerJumpScare()
    {
        if (jumpScareSFX != null)
        {
            jumpScareSFX.Play();
        }

        Debug.Log("Boo! Jumpscare triggered!");

    }
}
