using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    private bool isGamePaused = false;
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
                Debug.Log("Game paused");
            }
            else
            {
                PauseGame();
                Debug.Log("Game not paused");
            }
        }
    }

    private void PauseGame()
    {
        isGamePaused = true;
        pauseMenu.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void ContinueGame()
    {
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
}
