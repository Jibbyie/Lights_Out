using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class Rounds : MonoBehaviour
{
    private LightSwitchManager switchManager;

    private List<Action> actions = new List<Action>();

    [Header("Audio SFX")]
    public AudioSource doorKnockingSFX;
    public AudioSource behindYouSFX;

    private int lastEventIndex = -1;
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
            int randomIndex;

            // Ensure a different event than last time (checking for common events)
            do
            {
                randomIndex = UnityEngine.Random.Range(0, actions.Count);
            } while (randomIndex == lastEventIndex && actions.Count > 1);

            lastEventIndex = randomIndex; // Store the new event index
            Action selectedEvent = actions[randomIndex];
            selectedEvent.Invoke(); 

            // Remove only ONE instance of DoNothing if chosen
            if (selectedEvent == DoNothing)
            {
                actions.Remove(selectedEvent);
            }
            else
            {
                actions.RemoveAll(a => a == selectedEvent); // Fully remove normal events
            }
        }
        else
        {
            Debug.Log("No more horror functions to call!");
        }
    }

    void PopulateActionList()
    {
        // Do Nothing Event (Most Common)
        actions.Add(DoNothing);
        actions.Add(DoNothing);
        actions.Add(DoNothing);
        actions.Add(DoNothing);
        actions.Add(DoNothing);

        // Common Events (Add multiple times)
        actions.Add(DoorKnocking);
        actions.Add(DoorKnocking);
        actions.Add(DoorKnocking); 

        // Uncommon Events
        actions.Add(BehindYou);
        actions.Add(BehindYou);

        // Rare Event (Only add once)
        actions.Add(EventC); 
    }

    private IEnumerator SFXDelay(float delay, AudioSource sfx)
    {
        yield return new WaitForSeconds(delay);
        sfx.Play();
    }

    private void DoNothing()
    {
        Debug.Log("Nothing happens");
    }

    private void DoorKnocking()
    {
        Debug.Log("Horror Event A triggered");
        if(doorKnockingSFX != null)
        {
            StartCoroutine(SFXDelay(1.0f, doorKnockingSFX));
        }
    }

    private void BehindYou()
    {
        Debug.Log("Horror Event B triggered");
        if(behindYouSFX != null)
        {
            StartCoroutine(SFXDelay(2.0f, behindYouSFX));
        }
    }

    private void EventC()
    {
        Debug.Log("Horror Event C triggered");
    }

}

//Debug.Log("Current Actions in List:: ");

//foreach (var action in actions)
//{
//    Debug.Log(action.Method.Name); // Print functions name
//}