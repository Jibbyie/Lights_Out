using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    public GameObject intIcon, lightOn;
    public bool lightIsOn = false;

    public AudioSource switchTurnOn;
    public AudioSource switchTurnOff;
    public AudioSource ambience;

    private Coroutine lightOffTimer;

    private bool playerInRange = false; // Tracks if player is near the switch

    public void Start()
    {
        RenderSettings.fog = true;
        lightOn.SetActive(false);
        ambience.Stop();
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TurnOnLights();
        }
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

    private void TurnOnLights()
    {
        lightIsOn = true;
        RenderSettings.fog = false;
        lightOn.SetActive(true);
        switchTurnOn.Play();
        ambience.Play();

        // Start a new random countdown to turn lights off
        if (lightOffTimer != null) StopCoroutine(lightOffTimer);
        lightOffTimer = StartCoroutine(LightOffCountdown());
    }

    IEnumerator LightOffCountdown()
    {
        float waitTime = Random.Range(5f, 15f);
        yield return new WaitForSeconds(waitTime);

        lightIsOn = false;
        RenderSettings.fog = true;
        lightOn.SetActive(false);
        switchTurnOff.Play();
        ambience.Stop();
    }

}
