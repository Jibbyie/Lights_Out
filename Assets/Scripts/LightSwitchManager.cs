using UnityEngine;
using System.Collections.Generic;

public class LightSwitchManager : MonoBehaviour
{
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

        Debug.Log("Lights are off, tracking darkness time");
        Debug.Log("Global Lights Turned Off Counter: " + globalLightsTurnedOffCounter);

        rounds.ChooseRandomEvent();
        ActivateRandomSwitch();
    }

    public void OnLightsTurnedOn()
    {
        lightsAreOff = false;
        lightsTurnedOffTime = 0f;
    }

    public void LightsOffTime()
    {
        if (endGame.endGameTimer < 60)
        {
            lightsOffTooLongTime = 20f;
        }
        else if (endGame.endGameTimer < 120)
        {
            lightsOffTooLongTime = 15f;
        }
        else
        {
            lightsOffTooLongTime = 10f;
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
}
