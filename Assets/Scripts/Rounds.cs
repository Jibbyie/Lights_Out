using UnityEngine;

public class Rounds : MonoBehaviour
{
    private LightSwitchManager switchManager;
    private int lightsTurnedOffCounter;

    void Start()
    {
        switchManager = FindObjectOfType<LightSwitchManager>();

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
}
