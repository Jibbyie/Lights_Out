using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using System.Reflection;
using JetBrains.Annotations;

public class Rounds : MonoBehaviour
{
    private LightSwitchManager switchManager;
    private PauseMenu pauseMenu;

    [Header("Booleans")]
    private bool onLightsTurnedOn;
    private bool mirror1Active = false;
    private bool mirror2Active = false;
    private bool roomEmptied = false;
    private bool lightsAreCurrentlyOff = false;
    private bool mirror1HasEverSpawned = false;
    private bool entityEmptyRoomSpawned = false;
    private bool entityActionAddedToList = false;

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
    public GameObject entityEmptyRoom;

    private int lastEventIndex = -1;
    private int lightsTurnedOffCounter;
    private void OnDestroy()
    {
        LightSwitchManager.LightsTurnedOn -= HandleLightsOn;
        LightSwitchManager.LightsTurnedOff -= HandleLightsOff;
    }

    private void OnDisable()
    {
        LightSwitchManager.LightsTurnedOn -= HandleLightsOn;
        LightSwitchManager.LightsTurnedOff -= HandleLightsOff;
    }

    void Start()
    {
        actions.Clear();
        PopulateActionList();
        // preloading
        mirror1.SetActive(true);
        mirror1.SetActive(false);
        mirror2.SetActive(true);
        mirror2.SetActive(false);
        mirror1Active = false;
        mirror2Active = false;
        roomEmptied = false;
        entityActionAddedToList = false;

        switchManager = FindObjectOfType<LightSwitchManager>();
        pauseMenu = FindObjectOfType<PauseMenu>();

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
        if (pauseMenu == null)
        {
            pauseMenu = FindObjectOfType<PauseMenu>();
            if (pauseMenu == null) return;
        }

        if (pauseMenu.isGamePaused)
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
        lightsAreCurrentlyOff = false;

        if (mirror1Active && mirrorWhispersSFX != null)
        {
            mirrorWhispersSFX.Stop();
        }

        if (mirror2Active && mirrorWhispers2SFX != null)
        {
            mirrorWhispers2SFX.Stop();
        }


        if (roomEmptied)
        {
            if (roomEmptied && entityEmptyRoomSpawned && entityEmptyRoom != null && !entityEmptyRoom.Equals(null))
            {
                entityEmptyRoom.SetActive(false);
            }
        }
    }


    private void HandleLightsOff()
    {
        lightsAreCurrentlyOff = true;
        lightsTurnedOffCounter = switchManager.GetLightsTurnedOffCount();
        ChooseRandomEvent();

        // If the mirror has NEVER spawned and we're past turn 2, spawn mirror 1 FIRST
        if (!mirror1HasEverSpawned && lightsTurnedOffCounter > 2)
        {
            MirrorSpawn(); // This will mark mirror1HasEverSpawned = true
            return;
        }

        // Transition from mirror 1 to mirror 2 if enough counters has passed
        if (mirror1Active && lightsTurnedOffCounter > 4)
        {
            mirror1.SetActive(false);
            mirror1Active = false;

            if (mirrorWhispersSFX != null) mirrorWhispersSFX.Stop();

            mirror2.SetActive(true);
            mirror2Active = true;

            if (mirrorWhispers2SFX != null) mirrorWhispers2SFX.Play();

            return;
        }

        // If mirror1 is active and we're staying on it
        if (mirror1Active && mirrorWhispersSFX != null)
        {
            mirrorWhispersSFX.Play();
        }

        // If already on mirror2 and lights go off again
        if (mirror2Active && mirrorWhispers2SFX != null)
        {
            mirrorWhispers2SFX.Play();
        }

        if (roomEmptied && !entityActionAddedToList)
        {
            actions.Add(EmptyRoomEntity);
            entityActionAddedToList = true;
        }

        if (roomEmptied && entityEmptyRoomSpawned && entityEmptyRoom != null && !entityEmptyRoom.Equals(null))
        {
            entityEmptyRoom.SetActive(true);
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
        // Do Nothing Event (Most Common)
        actions.Add(DoNothing);
        actions.Add(DoNothing);

        // Common Events (Add multiple times)
        actions.Add(DoorKnocking);
        actions.Add(DoorKnocking);
        actions.Add(BehindYou);
        actions.Add(BehindYou);
        actions.Add(Footsteps);
        actions.Add(Footsteps);

        // Uncommon Events
        actions.Add(MirrorSpawn);
        actions.Add(PutTableOnCeiling);

        // Rare Event (Only add once)
        actions.Add(EmptyRoom);
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

    private void EmptyRoom()
    {
        if (roomProps == null) return;
        roomProps.SetActive(false);
        roomEmptied = true;
        Debug.Log("Room has been emptied");
    }

    private void EmptyRoomEntity()
    {
        if (entityEmptyRoom == null) return;
        entityEmptyRoomSpawned = true;
        Debug.Log("Entity spawned");
    }

    private void PutTableOnCeiling()
    {
        if (table == null) return;

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
        if (mirror1 == null) return;
        mirror1.SetActive(true);
        mirror1Active = true;
        mirror1HasEverSpawned = true;

        mirror2.SetActive(false);
        mirror2Active = false;

        Debug.Log("Mirror 1 spawned");
    }


    private IEnumerator SFXDelay(float delay, AudioSource sfx)
    {
        yield return new WaitForSeconds(delay);
        if (sfx != null)
        {
            sfx.Play();
        }
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
