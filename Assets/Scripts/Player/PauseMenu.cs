using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using Unity.VisualScripting;

public class PauseMenu : MonoBehaviour
{
    // Stop all sounds
    private AudioSource[] allAudioSources;

    public bool isGamePaused = false;
    public GameObject pauseMenu;

    public void Start()
    {
        pauseMenu.SetActive(false);
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                ContinueGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        StopAllAudio();

        isGamePaused = true;
        pauseMenu.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void ContinueGame()
    {
        ResumeAllAudio();

        isGamePaused = false;
        pauseMenu.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    void StopAllAudio()
    {
        allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource audioS in allAudioSources)
        {
            if (audioS.gameObject.name.Contains("Ambience") || audioS.gameObject.name.Contains("Whisper1") || audioS.gameObject.name.Contains("Whisper2") || audioS.gameObject.name.Contains("Whisper3") || audioS.gameObject.name.Contains("Step")) 
            {
                audioS.Pause(); // Pause instead of stopping it
            }
        }
    }

    void ResumeAllAudio()
    {
        allAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
        foreach (AudioSource audioS in allAudioSources)
        {
            if (audioS.gameObject.name.Contains("Ambience") || audioS.gameObject.name.Contains("Whisper1") || audioS.gameObject.name.Contains("Whisper2") || audioS.gameObject.name.Contains("Whisper3") || audioS.gameObject.name.Contains("Step"))
            {
                audioS.UnPause(); 
            }
        }
    }

}
