using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using System.Reflection;

public class Rounds : MonoBehaviour
{
    private LightSwitchManager switchManager;
    private PauseMenu pauseMenu;
    private bool onLightsTurnedOn;
    private bool mirror1Active = false;
    private bool mirror2Active = false;

    private List<Action> actions = new List<Action>();

    [Header("Audio SFX")]
    private AudioSource[] allAudioSources;
    public AudioSource doorKnockingSFX;
    public AudioSource behindYouSFX;
    public AudioSource mirrorWhispersSFX;
    public AudioSource mirrorWhispers2SFX;
    public AudioSource footstepsSFX;

    [Header("Game Objects")]
    public GameObject mirror1;
    public GameObject mirror2;
    public GameObject roomProps;
    public GameObject table;

    private int lastEventIndex = -1;
    private int lightsTurnedOffCounter;

    void Start()
    {
        Debug.Log(table.transform.position);
        Debug.Log(table.transform.rotation);

        // Mirrors preloading
        mirror1.SetActive(true);
        mirror1.SetActive(false);
        mirror2.SetActive(true);
        mirror2.SetActive(false);

        switchManager = FindObjectOfType<LightSwitchManager>();
        pauseMenu = FindObjectOfType<PauseMenu>();

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

    private void Update()
    {
        if(pauseMenu.isGamePaused)
        {
            StopAllAudio();
        }
        else
        {
            ResumeAllAudio();
        }
    }

    private void HandleLightsOn()
    {
        if(mirror1Active && lightsTurnedOffCounter < 1)
        {
            mirrorWhispersSFX.Stop();
        }
        if(mirror2Active)
        {
            mirrorWhispers2SFX.Stop();
        }
    }

    private void HandleLightsOff()
    {
        lightsTurnedOffCounter = switchManager.GetLightsTurnedOffCount(); // Update before checking

        ChooseRandomEvent();
        if (mirror1Active)
        {
            mirrorWhispersSFX.Play();
        }
        if (lightsTurnedOffCounter > 2 && mirror1Active)
        {
            mirror1.SetActive(false);
            mirror1Active = false;
            mirrorWhispersSFX.Stop();

            mirror2.SetActive(true);
            mirror2Active = true;
        }
        if (mirror2Active)
        {
            StartCoroutine(Delay(0.1f));
            mirrorWhispers2SFX.Play();
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
        actions.Add(PutTableOnCeiling);
        //actions.Add(ShiftTheRoom);
        //actions.Add(Footsteps);
        //actions.Add(BehindYou);
        //actions.Add(DoorKnocking);
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
        //actions.Add(MirrorSpawn); 
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

    private void ShiftTheRoom()
    {
        roomProps.transform.rotation = Quaternion.Euler(0, -13.5f, 0);
    }

    private void PutTableOnCeiling()
    {
        table.transform.position = new Vector3(-1.54f, 2.80f, 4.05f);
        table.transform.rotation = new Quaternion(0.35849f, 0f, 0.93353f, 0f);
    }

    private void Footsteps()
    {
        Debug.Log("Footsteps behind you played");
        if (footstepsSFX != null)
        {
            StartCoroutine(SFXDelay(1.0f, footstepsSFX));
        }
}
    private void MirrorSpawn()
    {
        mirror1.SetActive(true);
        mirror1Active = true;
        mirrorWhispersSFX.Play();
        Debug.Log("Mirror spawned");
    }

    private IEnumerator SFXDelay(float delay, AudioSource sfx)
    {
        yield return new WaitForSeconds(delay);
        sfx.Play();
    }

    private IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    void StopAllAudio()
    {
        allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach(AudioSource audioS in allAudioSources)
        {
            audioS.Pause();
        }
    }

    void ResumeAllAudio()
    {
        allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach(AudioSource audioS in allAudioSources)
        {
            audioS.UnPause();
        }
    }


    //
    //
    // DEBUGGING
    //
    //

    public void TriggerRoundByName(string methodName)
    {
        var method = typeof(Rounds).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null)
        {
            method.Invoke(this, null);
            Debug.Log($"DEBUG: Triggered round '{methodName}'");
        }
        else
        {
            Debug.LogWarning($"DEBUG: Round method '{methodName}' not found.");
        }
    }

}

//Debug.Log("Current Actions in List:: ");

//foreach (var action in actions)
//{
//    Debug.Log(action.Method.Name); // Print functions name
//}

