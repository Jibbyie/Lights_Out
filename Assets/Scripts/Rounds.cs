using System.Collections.Generic;
using UnityEngine;
using System;

public class Rounds : MonoBehaviour
{
    private LightSwitchManager switchManager;

    private List<Action> actions = new List<Action>();

    public GameObject table;
    private int lightsTurnedOffCounter;

    void Start()
    {
        switchManager = FindObjectOfType<LightSwitchManager>();

        PopulateActionList();

        if (switchManager != null)
        {
            lightsTurnedOffCounter = switchManager.GetLightsTurnedOffCount();
        }
        else
        {
            Debug.LogError("SwitchManager not found in the scene!");
        }
    }

    void Update()
    {
        if (switchManager != null)
        {
            lightsTurnedOffCounter = switchManager.GetLightsTurnedOffCount();
        }
    }

    public void ChooseRandomEvent()
    {
        if (actions.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, actions.Count);
            Action selectedEvent = actions[randomIndex];
            selectedEvent.Invoke(); // Randomly call selected horror function from actions list

            actions.RemoveAll(a => a == selectedEvent); // Remove selected function and all instances of it
                                                        // to remove super common events
            Debug.Log("Current Actions in List:: ");

            foreach (var action in actions)
            {
                Debug.Log(action.Method.Name); // Print functions name
            }
        }
        else
        {
            Debug.Log("No more horror functions to call!");
        }
    }

    void PopulateActionList()
    {
        // Common Events (Added multiple times)
        actions.Add(EventA);
        actions.Add(EventA);
        actions.Add(EventA);
        actions.Add(EventA); // Higher chance of being picked

        actions.Add(EventB);
        actions.Add(EventB); // Medium chance

        // Rare Event (Only added once)
        actions.Add(EventC); // Lower chance of being picked
    }

    private void EventA()
    {
        Debug.Log("Horror Event A");
    }

    private void EventB()
    {
        Debug.Log("Horror Event B");
    }

    private void EventC()
    {
        Debug.Log("Horror Event C");
    }

}
