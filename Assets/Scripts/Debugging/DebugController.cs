#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;

public class DebugController : MonoBehaviour
{
    private LightSwitchManager switchManager;
    private Rounds rounds;

    void Start()
    {
        DebugFlags.IsDebugging = true;
        switchManager = FindObjectOfType<LightSwitchManager>();
        rounds = FindObjectOfType<Rounds>();
    }

    void Update()
    {
        // F1: Force lights ON
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("DEBUG (F1): Forcing Lights On");
            SimulateLightsOn();
        }

        // F2: Force lights OFF
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Debug.Log("DEBUG (F2): Simulating Lights Off");
            SimulateLightsOff();
        }

        // F3: Trigger random horror event
        if (Input.GetKeyDown(KeyCode.F3))
        {
            Debug.Log("DEBUG (F3): Triggering random horror event");
            rounds?.ChooseRandomEvent();
        }

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
            if (js != null)
            {
                Debug.Log("DEBUG (F6): Triggering normal jumpscare");
                js.TriggerJumpScare();
            }
        }

        // F7: Trigger early jumpscare
        if (Input.GetKeyDown(KeyCode.F7))
        {
            var js = FindObjectOfType<JumpScares>();
            if (js != null)
            {
                Debug.Log("DEBUG (F7): Triggering early jumpscare");
                js.TriggerEarlyJumpScare();
            }
        }

        // F8: Trigger endgame manually
        if (Input.GetKeyDown(KeyCode.F8))
        {
            var endGame = FindObjectOfType<EndGame>();
            if (endGame != null)
            {
                Debug.Log("DEBUG (F8): Triggering manual endgame");
                typeof(EndGame)
                    .GetMethod("TriggerGameEnd", BindingFlags.NonPublic | BindingFlags.Instance)
                    ?.Invoke(endGame, null);
            }
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
