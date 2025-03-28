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

    public AudioSource jumpScareSFX;

    private Coroutine lightOffTimer;
    public float lightsTurnedOffCounter = 0f;
    public float lightsTurnedOffTime = 0f;
    public float endGameTimer = 0f;
    public float timeNearSwitch = 0f;

    public float endGameTime = 180f;
    public float campingLightsTime = 30f;
    public float lightsOffTooLongTime = 10f;

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

        endGameTimer += Time.deltaTime;
        if (endGameTimer >= endGameTime)
        {
            JumpScare();
        }

        TriggerJEarlyumpScare();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            intIcon.SetActive(true);
            playerInRange = true; // Player is near the switch
            timeNearSwitch += Time.deltaTime;
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
        if (!switchTurnOn.isPlaying && !ambience.isPlaying)
        {
            switchTurnOn.Play();
            ambience.Play();
        }

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
        lightsTurnedOffCounter += 1;
    }

    private void TimeNearSwitch()
    {
        if(timeNearSwitch >= 5f)
        {

        }
    }

    private void TriggerJEarlyumpScare()
    {
        if(lightIsOn == false)
        {
            lightsTurnedOffTime += Time.deltaTime;
            if(lightsTurnedOffTime >= lightsOffTooLongTime)
            {
                JumpScare();
            }
        }
        if(lightIsOn == true)
        {
            lightsTurnedOffTime = 0f;
        }
    }

    private void JumpScare()
    {
        jumpScareSFX.Play();
        Debug.Log("Boo!");
    }
}
