using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class EndGame : MonoBehaviour
{
    public float endGameTime = 180f; 
    public float endGameTimer = 0f;

    private JumpScares jumpScares;
    private bool hasGameStarted = false;

    private void Start()
    {
        jumpScares = FindObjectOfType<JumpScares>(); 
    }

    private void Update()
    {
        if (hasGameStarted) 
        {
            endGameTimer += Time.deltaTime;
            if (endGameTimer >= endGameTime)
            {
                TriggerGameEnd();
            }
        }
    }

    public void StartGameTimer()
    {
        if (!hasGameStarted)
        {
            hasGameStarted = true;
            Debug.Log("Game timer started!");
        }
    }

    private void TriggerGameEnd()
    {
        SceneManager.LoadScene("YouSurvived");
    }
}
