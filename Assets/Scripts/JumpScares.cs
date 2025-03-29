using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class JumpScares : MonoBehaviour
{
    public AudioSource jumpScareSFX;
    public AudioSource lightSwitchOff;
    private LightSwitchManager lightSwitchManager;
    private LightSwitch lightSwitch;
    private CampingSwitch campingSwitch;
    private bool isSceneLoading = false; // Prevents multiple scene load calls

    public void Start()
    {
        lightSwitch = FindObjectOfType<LightSwitch>();
        lightSwitchManager = FindObjectOfType<LightSwitchManager>();
        campingSwitch = FindObjectOfType<CampingSwitch>();
    }

    private void Update()
    {
        if (!isSceneLoading) // Ensure scene loading only happens once
        {
            if (lightSwitchManager.lightsAreOff &&
                lightSwitchManager.GetLightsTurnedOffTime() >= lightSwitchManager.GetLightsOffTooLongTime())
            {
                isSceneLoading = true; // Prevent multiple triggers
                lightSwitchOff.PlayOneShot(lightSwitchOff.clip);
                StartCoroutine(LoadSceneAfterSFX("TooLongInTheDark", lightSwitchOff.clip.length));
            }

            if (campingSwitch.TimeNearSwitch() >= campingSwitch.MaxCampingTime())
            {
                isSceneLoading = true;
                lightSwitchOff.PlayOneShot(lightSwitchOff.clip);
                StartCoroutine(LoadSceneAfterSFX("DontCampTheSwitches", lightSwitchOff.clip.length));
            }
        }
    }

    public void TriggerJumpScare()
    {
        if (jumpScareSFX != null)
        {
            jumpScareSFX.Play();
        }

        Debug.Log("Boo! Jumpscare triggered!");

    }

    public void TriggerEarlyJumpScare()
    {
        Debug.Log("Early jumpscare triggered! The lights were off too long! : " + lightSwitchManager.lightsTurnedOffTime);

        if (jumpScareSFX != null)
        {
            jumpScareSFX.Play();
        }

    }

    private IEnumerator LoadSceneAfterSFX(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay); 
        SceneManager.LoadScene(sceneName);
    }
}
