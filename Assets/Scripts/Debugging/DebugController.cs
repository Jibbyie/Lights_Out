#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

public class DebugController : MonoBehaviour
{
    private LightSwitchManager switchManager;
    private EndGame endGame;
    private JumpScares jumpScares;

    public Rounds rounds;

    [Header("Trigger Round By Name")]
    public int selectedRoundIndex = 0;
    public string[] roundMethodOptions;
    private List<string> roundMethodNames = new List<string>();
    private bool methodsInitialized = false;

    [Header("Manual Round Trigger (Fallback)")]
    public string roundMethodName = "";
    public bool triggerNamedRound = false;

    void Start()
    {
        DebugFlags.IsDebugging = true;
    }

    void Update()
    {
        RefreshReferences();

        if (rounds != null && !methodsInitialized)
            InitializeRoundMethodList();

        if (rounds == null || switchManager == null) return;

        // F1: Force lights ON
        if (Input.GetKeyDown(KeyCode.F1)) SimulateLightsOn();

        // F2: Force lights OFF
        if (Input.GetKeyDown(KeyCode.F2)) SimulateLightsOff();

        // F3: Trigger random horror event
        if (Input.GetKeyDown(KeyCode.F3)) rounds.ChooseRandomEvent();

        // F4: Increment lightsTurnedOffCounter manually
        if (Input.GetKeyDown(KeyCode.F4))
        {
            int currentCount = switchManager.GetLightsTurnedOffCount();
            int newCount = currentCount + 1;

            typeof(LightSwitchManager)
                .GetField("globalLightsTurnedOffCounter", BindingFlags.Public | BindingFlags.Instance)
                ?.SetValue(switchManager, newCount);

            Debug.Log($"DEBUG (F4): lightsTurnedOffCounter set to {newCount}");
        }

        // F5: Log current state
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Debug.Log($"DEBUG (F5): lightsOff = {switchManager.lightsAreOff}, lightsTurnedOffCounter = {switchManager.GetLightsTurnedOffCount()}");
        }

        // F6: Trigger jumpscare
        if (Input.GetKeyDown(KeyCode.F6)) jumpScares?.TriggerJumpScare();

        // F7: Trigger early jumpscare
        if (Input.GetKeyDown(KeyCode.F7)) jumpScares?.TriggerEarlyJumpScare();

        // F8: Trigger endgame manually
        if (Input.GetKeyDown(KeyCode.F8) && endGame != null)
        {
            typeof(EndGame)
                .GetMethod("TriggerGameEnd", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(endGame, null);
        }

        // Optional fallback
        if (triggerNamedRound && !string.IsNullOrEmpty(roundMethodName))
        {
            triggerNamedRound = false;
            TriggerRoundByName(roundMethodName);
        }
    }

    private void RefreshReferences()
    {
        if (rounds == null) rounds = FindObjectOfType<Rounds>();
        if (switchManager == null) switchManager = FindObjectOfType<LightSwitchManager>();
        if (jumpScares == null) jumpScares = FindObjectOfType<JumpScares>();
        if (endGame == null) endGame = FindObjectOfType<EndGame>();

        // In case of replay, reset method list
        if (rounds != null && roundMethodOptions == null)
        {
            methodsInitialized = false;
        }
    }

    private void InitializeRoundMethodList()
    {
        var actionsField = typeof(Rounds).GetField("actions", BindingFlags.NonPublic | BindingFlags.Instance);
        var actionList = actionsField?.GetValue(rounds) as List<System.Action>;

        if (actionList != null && actionList.Count > 0)
        {
            roundMethodNames = actionList
                .Where(a => a != null && a.Method != null)
                .Select(a => a.Method.Name)
                .Distinct()
                .ToList();

            roundMethodOptions = roundMethodNames.ToArray();
            methodsInitialized = true;

            Debug.Log("[DEBUG] Round methods loaded from actions list.");
        }
    }

    public void TriggerSelectedRound()
    {
        if (rounds == null || roundMethodNames.Count == 0) return;

        string methodName = roundMethodNames[selectedRoundIndex];
        TriggerRoundByName(methodName);
    }

    public void TriggerRoundByName(string methodName)
    {
        if (rounds == null) return;

        MethodInfo method = typeof(Rounds).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (method != null)
        {
            method.Invoke(rounds, null);
            Debug.Log($"[DEBUG] Triggered round: {methodName}");
        }
        else
        {
            Debug.LogWarning($"[DEBUG] Method '{methodName}' not found.");
        }
    }

    private void SimulateLightsOn()
    {
        var currentSwitch = GetCurrentActiveSwitch();
        if (currentSwitch != null)
        {
            typeof(LightSwitch)
                .GetMethod("TurnOnLights", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(currentSwitch, null);
        }
        else
        {
            Debug.LogWarning("DEBUG: No active LightSwitch found.");
        }
    }

    private void SimulateLightsOff()
    {
        var currentSwitch = GetCurrentActiveSwitch();
        if (currentSwitch != null)
        {
            currentSwitch.lightIsOn = false;
            RenderSettings.fog = true;
            currentSwitch.lightOn.SetActive(false);
            currentSwitch.switchTurnOff.Play();
            currentSwitch.ambience.Stop();

            switchManager.OnLightsTurnedOff();
        }
        else
        {
            Debug.LogWarning("DEBUG: No active LightSwitch found.");
        }
    }

    private LightSwitch GetCurrentActiveSwitch()
    {
        if (switchManager?.switches == null) return null;

        foreach (var sw in switchManager.switches)
        {
            if (sw != null && sw.gameObject.activeSelf)
                return sw;
        }
        return null;
    }
}
#endif
