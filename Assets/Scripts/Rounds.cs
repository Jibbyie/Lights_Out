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

        actions.Add(EventA);
        actions.Add(EventB);
        actions.Add(EventC);

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
            actions[randomIndex].Invoke(); // Randomly call horror function from actions list
            actions.RemoveAt(randomIndex); // Remove it so it doesn't happen twice
        }
        else
        {
            Debug.Log("No more horror functions to call!");
        }
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
