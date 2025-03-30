using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public AudioSource switchTurnOn;
    public AudioSource ambience;
    public GameObject loadingScreen;
    public GameObject playButton;
    public GameObject quitButton;

    public void StartGame()
    {
        StartCoroutine(PreloadAndStartGame());
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
