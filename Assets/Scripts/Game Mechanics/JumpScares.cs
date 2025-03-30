using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.LowLevel;

public class JumpScares : MonoBehaviour
{
    public AudioSource jumpScareSFX;
    public AudioSource lightSwitchOff;
    public AudioSource neckSnapSFX;
    public AudioSource entityGrabbedYouSFX;
    public GameObject blackScreen;
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
                neckSnapSFX.PlayOneShot(neckSnapSFX.clip);
                StartCoroutine(LoadBlackScreenThenEndScreen("TooLongInTheDark", neckSnapSFX.clip.length));
            }

            if (campingSwitch.TimeNearSwitch() >= campingSwitch.MaxCampingTime())
            {
                isSceneLoading = true;
                lightSwitchOff.PlayOneShot(lightSwitchOff.clip);
                StartCoroutine(LoadBlackScreenThenEndScreen("DontCampTheSwitches", lightSwitchOff.clip.length));
            }
        }
    }

    public void TriggerJumpScare()
    {
       
        if(entityGrabbedYouSFX != null)
        {
            entityGrabbedYouSFX.PlayOneShot(entityGrabbedYouSFX.clip);
            StartCoroutine(LoadBlackScreenThenEndScreen("TheEntityGotYou", entityGrabbedYouSFX.clip.length));
        }
    }

    private IEnumerator LoadBlackScreenThenEndScreen(string sceneName, float blackScreenDelay)
    {

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var movement = player.GetComponent<PlayerMovement>();
            var look = player.GetComponent<MouseLook>();
            if (movement) movement.enabled = false;
            if (look) look.enabled = false;
        }

        blackScreen.SetActive(true);

        yield return new WaitForSeconds(blackScreenDelay + 1F);
        SceneManager.LoadScene(sceneName);
    }

}
