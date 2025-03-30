#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

public class DebugController : MonoBehaviour
{
    private LightSwitchManager switchManager;
    public Rounds rounds;

    [Header("Trigger Round By Name")]
    public int selectedRoundIndex = 0;
    public string[] roundMethodOptions;
    private List<string> roundMethodNames = new List<string>();

    [Header("Manual Round Trigger (Fallback)")]
    public string roundMethodName = "";
    public bool triggerNamedRound = false;

    void Start()
    {
        DebugFlags.IsDebugging = true;

        switchManager = FindObjectOfType<LightSwitchManager>();
        if (rounds == null) rounds = FindObjectOfType<Rounds>();

        // Collect private void methods from Rounds
        if (rounds != null)
        {
            var methods = typeof(Rounds)
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.ReturnType == typeof(void) && m.GetParameters().Length == 0)
                .Select(m => m.Name)
                .ToList();

            roundMethodNames = methods;
            roundMethodOptions = methods.ToArray();
        }
    }

    void Update()
    {
        // F1: Force lights ON
        if (Input.GetKeyDown(KeyCode.F1)) SimulateLightsOn();

        // F2: Force lights OFF
        if (Input.GetKeyDown(KeyCode.F2)) SimulateLightsOff();

        // F3: Trigger random horror event
        if (Input.GetKeyDown(KeyCode.F3)) rounds?.ChooseRandomEvent();

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
        if (Input.GetKeyDown(KeyCode.F6))
        {
            var js = FindObjectOfType<JumpScares>();
            js?.TriggerJumpScare();
        }

        // F7: Trigger early jumpscare
        if (Input.GetKeyDown(KeyCode.F7))
        {
            var js = FindObjectOfType<JumpScares>();
            js?.TriggerEarlyJumpScare();
        }

        // F8: Trigger endgame manually
        if (Input.GetKeyDown(KeyCode.F8))
        {
            var endGame = FindObjectOfType<EndGame>();
            typeof(EndGame)
                .GetMethod("TriggerGameEnd", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(endGame, null);
        }

        // Optional fallback string trigger
        if (triggerNamedRound && !string.IsNullOrEmpty(roundMethodName))
        {
            triggerNamedRound = false;
            TriggerRoundByName(roundMethodName);
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
        foreach (var sw in switchManager.switches)
        {
            if (sw != null && sw.gameObject.activeSelf)
                return sw;
        }
        return null;
    }
}
#endif
