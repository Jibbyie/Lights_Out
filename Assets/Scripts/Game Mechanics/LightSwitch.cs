using System.Collections;
using UnityEngine;
using TMPro;

public class LightSwitch : MonoBehaviour
{
    public GameObject intIcon, lightOn;
    private CampingSwitch campingSwitch;
    private LightSwitchManager lightSwitchManager;
    private EndGame endGame;

    public bool lightIsOn = false;
    private bool playerInRange = false;

    public AudioSource switchTurnOn;
    public AudioSource switchTurnOff;
    public AudioSource ambience;

    private Coroutine lightOffTimer;

    private void Start()
    {
        RenderSettings.fog = true;
        lightOn.SetActive(false);

        campingSwitch = FindObjectOfType<CampingSwitch>();
        lightSwitchManager = FindObjectOfType<LightSwitchManager>();
        endGame = FindObjectOfType<EndGame>();
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

        lightSwitchManager.OnLightsTurnedOn();
        endGame.StartGameTimer();

        if (lightOffTimer != null) StopCoroutine(lightOffTimer);

        if (!DebugFlags.IsDebugging)
        {
            lightOffTimer = StartCoroutine(LightOffCountdown());
        }
    }

    IEnumerator LightOffCountdown()
    {
        float waitTime;
        if (endGame.endGameTimer < 40)
        {
            waitTime = Random.Range(12f, 16f);
            Debug.Log("Round 1 wait time " + waitTime);
        }
        else if (endGame.endGameTimer < 90)
        {
            waitTime = Random.Range(8f, 12f);
            Debug.Log("Round 2 wait time " + waitTime);
        }
        else
        {
            waitTime = Random.Range(1f, 5f);
            Debug.Log("Round 3 wait time " + waitTime);
        }

        yield return new WaitForSeconds(waitTime);

        lightIsOn = false;
        RenderSettings.fog = true;
        lightOn.SetActive(false);
        switchTurnOff.Play();
        ambience.Stop();

        // Update the global counter in SwitchManager
        lightSwitchManager.OnLightsTurnedOff();
    }

}