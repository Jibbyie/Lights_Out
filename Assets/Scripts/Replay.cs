using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;

public class Replay : MonoBehaviour
{
    public AudioSource switchTurnOn;
    public AudioSource ambience;

    public GameObject loadingScreen;
    public GameObject playButton;
    public GameObject quitButton;
    public GameObject textUI;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void ReplayGame()
    {
        StartCoroutine(PreloadAndStartGame());
    }

    public void QuitGame()
    {
       Application.Quit();
    }

    IEnumerator PreloadAndStartGame()
    {
        if (playButton != null)
        {
            playButton.SetActive(false); 
        }

        if (quitButton != null)
        {
            quitButton.SetActive(false);
        }

        if (textUI != null)
        {
            textUI.SetActive(false);
        }

        loadingScreen.SetActive(true); // Show loading screen immediately

        yield return new WaitForSeconds(0.5f); // Allow UI to update before freeze

        // Preload audio by playing and pausing
        switchTurnOn.Play();
        switchTurnOn.Pause();
        ambience.Play();
        ambience.Pause();

        yield return new WaitForSeconds(1f); // Allow Unity to process preloading

        Debug.Log("Audio Preloaded - Now Loading Game Scene...");

        SceneManager.LoadScene("Game");
    }
}
