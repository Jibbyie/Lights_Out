using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class LightSwitchManager : MonoBehaviour
{
    public static event Action LightsTurnedOn;
    public static event Action LightsTurnedOff;

    public LightSwitch[] switches; 
    private LightSwitch activeSwitch;

    private EndGame endGame;
    private Rounds rounds;

    public float lightsTurnedOffTime = 0f;
    public float lightsOffTooLongTime = 20f;
    public int globalLightsTurnedOffCounter = 0;

    public bool lightsAreOff = false;

    private void Start()
    {
        lightsAreOff = false;
        endGame = FindObjectOfType<EndGame>();
        rounds = FindObjectOfType<Rounds>();    

        // Ensure all switches are deactivated at start
        foreach (LightSwitch switchObj in switches)
        {
            switchObj.gameObject.SetActive(false);
        }

        // Activate the first random switch
        ActivateRandomSwitch();
    }

    private void Update()
    {
        if(lightsAreOff)
        {
            lightsTurnedOffTime += Time.deltaTime;
        }
        LightsOffTime();
    }

    public void ActivateRandomSwitch()
    {
        // Disable the current switch
        if (activeSwitch != null)
        {
            activeSwitch.gameObject.SetActive(false);
        }

        // Pick a new random switch
        LightSwitch newSwitch;
        do
        {
            newSwitch = switches[Random.Range(0, switches.Length)];
        } while (newSwitch == activeSwitch); // Ensure it doesn't pick the same switch

        // Activate the new switch
        newSwitch.gameObject.SetActive(true);
        activeSwitch = newSwitch;
    }

    public void OnLightsTurnedOff()
    {
        globalLightsTurnedOffCounter++;
        lightsTurnedOffTime = 0f;
        lightsAreOff = true;

        Debug.Log("Global Lights Turned Off Counter: " + globalLightsTurnedOffCounter);

        LightsTurnedOff?.Invoke(); // Fire the event but only if there are subscribers

        ActivateRandomSwitch();
    }

    public void OnLightsTurnedOn()
    {
        lightsAreOff = false;
        lightsTurnedOffTime = 0f;
        LightsTurnedOn?.Invoke();
    }

    public void LightsOffTime()
    {
        if (DebugFlags.IsDebugging) return;

        if (endGame.endGameTimer < 40)
        {
            lightsOffTooLongTime = 20f;
        }
        else if (endGame.endGameTimer < 90)
        {
            lightsOffTooLongTime = 16f;
        }
        else
        {
            lightsOffTooLongTime = 12f;
        }
    }

    public int GetLightsTurnedOffCount() // Used for round based system script
    {
        return globalLightsTurnedOffCounter;
    }

    public float GetLightsTurnedOffTime()
    {
        return lightsTurnedOffTime;
    }

    public float GetLightsOffTooLongTime()
    {
        return lightsOffTooLongTime;
    }

    public bool GetLightsTurnedOn()
    {
        return lightsAreOff;
    }
}
