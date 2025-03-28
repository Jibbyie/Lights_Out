using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public GameObject intIcon, lightOn;
    public bool lightIsOn = false;
    public AudioSource switchSound;

    private bool playerInRange = false; // Tracks if player is near the switch

    public void Start()
    {
        RenderSettings.fog = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            intIcon.SetActive(true);
            playerInRange = true; // Player is near the switch
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            intIcon.SetActive(false);
            playerInRange = false; // Player leaves switch area
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            lightIsOn = true;
            RenderSettings.fog = false;
            lightOn.SetActive(true);
            switchSound.Play();
        }
    }
}
