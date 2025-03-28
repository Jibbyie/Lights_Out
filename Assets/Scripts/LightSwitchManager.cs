using UnityEngine;
using System.Collections.Generic;

public class LightSwitchManager : MonoBehaviour
{
    public LightSwitch[] switches; // Assign all switches in the Inspector
    private LightSwitch activeSwitch;
    public int globalLightsTurnedOffCounter = 0; // Global counter for all switches

    private void Start()
    {
        // Ensure all switches are deactivated at start
        foreach (LightSwitch switchObj in switches)
        {
            switchObj.gameObject.SetActive(false);
        }

        // Activate the first random switch
        ActivateRandomSwitch();
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

    // This method is called when any LightSwitch turns off
    public void OnLightsTurnedOff()
    {
        globalLightsTurnedOffCounter++; // Increment global counter
        Debug.Log("Global Lights Turned Off Counter: " + globalLightsTurnedOffCounter);

        ActivateRandomSwitch();
    }

    public int GetLightsTurnedOffCount()
    {
        return globalLightsTurnedOffCounter;
    }
}
