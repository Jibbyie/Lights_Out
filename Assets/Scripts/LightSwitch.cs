using System.Collections;
using UnityEngine;
using TMPro;

public class LightSwitch : MonoBehaviour
{
    public GameObject intIcon, lightOn;
    public bool lightIsOn = false;

    public AudioSource switchTurnOn;
    public AudioSource switchTurnOff;
    public AudioSource ambience;

    private Coroutine lightOffTimer;
    public float lightsTurnedOffCounter = 0f;
    public float lightsTurnedOffTime = 0f;
    public float lightsOffTooLongTime = 10f;

    private bool playerInRange = false;
    public CampingSwitch campingSwitch;
    public JumpScares jumpScares;
    public EndGame endGame; 

    private void Start()
    {
        RenderSettings.fog = true;
        lightOn.SetActive(false);

        // Ensure all references are assigned in the Inspector
        if (campingSwitch == null) Debug.LogError("CampingSwitch is not assigned in LightSwitch!");
        if (jumpScares == null) Debug.LogError("JumpScares is not assigned in LightSwitch!");
        if (endGame == null) Debug.LogError("EndGame is not assigned in LightSwitch!");
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TurnOnLights();
        }

        TriggerEarlyJumpScare();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            intIcon.SetActive(true);
            playerInRange = true;

            if (campingSwitch != null)
            {
                campingSwitch.PlayerNearSwitch(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            intIcon.SetActive(false);
            playerInRange = false;

            if (campingSwitch != null)
            {
                campingSwitch.PlayerNearSwitch(false);
            }
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

    private void TriggerEarlyJumpScare()
    {
        if (!lightIsOn)
        {
            lightsTurnedOffTime += Time.deltaTime;
            if (lightsTurnedOffTime >= lightsOffTooLongTime)
            {
                if (jumpScares != null)
                {
                    jumpScares.TriggerJumpScare();
                }
            }
        }
        else
        {
            lightsTurnedOffTime = 0f;
        }
    }
}
