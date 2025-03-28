using UnityEngine;

public class EndGame : MonoBehaviour
{
    public float endGameTime = 180f; 
    public float endGameTimer = 0f; 

    private JumpScares jumpScares; 

    private void Start()
    {
        jumpScares = FindObjectOfType<JumpScares>(); 
    }

    private void Update()
    {
        endGameTimer += Time.deltaTime;

        if (endGameTimer >= endGameTime)
        {
            TriggerGameEnd();
        }
    }

    private void TriggerGameEnd()
    {
        if (jumpScares != null)
        {
            jumpScares.TriggerJumpScare();
        }

        Debug.Log("Game Over! Final Jumpscare Triggered.");
    }
}
