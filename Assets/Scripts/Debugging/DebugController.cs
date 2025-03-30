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

        // Dynamically refresh method list when not yet initialized
        if (!methodsInitialized && rounds != null)
        {
            RefreshRoundMethodList();
        }

        if (rounds == null || switchManager == null) return;

        // Debug input bindings
        if (Input.GetKeyDown(KeyCode.F1)) SimulateLightsOn();
        if (Input.GetKeyDown(KeyCode.F2)) SimulateLightsOff();
        if (Input.GetKeyDown(KeyCode.F3)) rounds.ChooseRandomEvent();
        if (Input.GetKeyDown(KeyCode.F4)) IncrementLightsOffCounter();
        if (Input.GetKeyDown(KeyCode.F5)) LogLightStatus();
        if (Input.GetKeyDown(KeyCode.F6)) jumpScares?.TriggerJumpScare();
        if (Input.GetKeyDown(KeyCode.F8)) TriggerEndGame();

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
    }

    private void RefreshRoundMethodList()
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

            Debug.Log("[DEBUG] Round method list refreshed.");
        }
    }

    public void ForceRefreshMethodList()
    {
        methodsInitialized = false;
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
            try
            {
                method.Invoke(rounds, null);
                Debug.Log($"[DEBUG] Triggered round: {methodName}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[DEBUG] Failed to invoke '{methodName}': {ex.Message}");
            }
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
            currentSwitch.switchTurnOff?.Play();
            currentSwitch.ambience?.Stop();

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

    private void IncrementLightsOffCounter()
    {
        int currentCount = switchManager.GetLightsTurnedOffCount();
        int newCount = currentCount + 1;

        typeof(LightSwitchManager)
            .GetField("globalLightsTurnedOffCounter", BindingFlags.Public | BindingFlags.Instance)
            ?.SetValue(switchManager, newCount);

        Debug.Log($"DEBUG (F4): lightsTurnedOffCounter set to {newCount}");
    }

    private void LogLightStatus()
    {
        Debug.Log($"DEBUG (F5): lightsOff = {switchManager.lightsAreOff}, lightsTurnedOffCounter = {switchManager.GetLightsTurnedOffCount()}");
    }

    private void TriggerEndGame()
    {
        if (endGame != null)
        {
            typeof(EndGame)
                .GetMethod("TriggerGameEnd", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(endGame, null);
        }
    }
}
#endif
