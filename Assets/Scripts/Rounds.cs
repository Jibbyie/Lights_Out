using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class Rounds : MonoBehaviour
{
    private LightSwitchManager switchManager;
    private bool onLightsTurnedOn;

    private List<Action> actions = new List<Action>();

    [Header("Audio SFX")]
    public AudioSource doorKnockingSFX;
    public AudioSource behindYouSFX;

    [Header("Game Objects")]
    public GameObject entity;

    private int lastEventIndex = -1;
    private int lightsTurnedOffCounter;

    void Start()
    {
        entity.SetActive(true);
        entity.SetActive(false);
        switchManager = FindObjectOfType<LightSwitchManager>();

        PopulateActionList();

        if (switchManager != null)
        {
            lightsTurnedOffCounter = switchManager.GetLightsTurnedOffCount();
            onLightsTurnedOn = switchManager.GetLightsTurnedOn();
        }
        else
        {
            Debug.LogError("SwitchManager not found in the scene!");
        }

        LightSwitchManager.LightsTurnedOn += HandleLightsOn; // Subscribe to events in LightSwitchManager
        LightSwitchManager.LightsTurnedOff += HandleLightsOff;
    }

    private void HandleLightsOn()
    {

    }

    private void HandleLightsOff()
    {
        ChooseRandomEvent(); 
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
            } while (randomIndex == lastEventIndex && actions.Count > 1); // While randomIndex is equal to lastEventIndex, redo (also check we are above 1 to account for the last item)

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
        /*
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
        */

        // Rare Event (Only add once)
        actions.Add(MirrorSpawn); 
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
        Debug.Log("Door Knocking event triggered");
        if(doorKnockingSFX != null)
        {
            StartCoroutine(SFXDelay(1.0f, doorKnockingSFX));
        }
    }

    private void BehindYou()
    {
        Debug.Log("Whisper Behind You event triggered");
        if(behindYouSFX != null)
        {
            StartCoroutine(SFXDelay(2.0f, behindYouSFX));
        }
    }

    private void MirrorSpawn()
    {
        entity.SetActive(true);
        Debug.Log("Mirror spawned");
    }

}

//Debug.Log("Current Actions in List:: ");

//foreach (var action in actions)
//{
//    Debug.Log(action.Method.Name); // Print functions name
//}