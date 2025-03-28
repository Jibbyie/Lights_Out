using System.Collections;
using UnityEngine;
using TMPro;

public class CampingSwitch : MonoBehaviour
{
    public TMP_Text backAwayText;
    public float timeNearSwitch = 0f;
    private bool isShowingText = false;
    private bool playerCamping = false; 

    public float campingTimeThreshold1 = 5f; 
    public float maxCampingTime = 30f; 

    private void Update()
    {
        if (playerCamping)
        {
            timeNearSwitch += Time.deltaTime;

            if (timeNearSwitch >= campingTimeThreshold1 && !isShowingText)
            {
                StartCoroutine(ShowBackAwayText());
            }

            if (timeNearSwitch >= maxCampingTime)
            {
                TriggerCampingPunishment();
            }
        }
    }

    public void PlayerNearSwitch(bool isNear)
    {
        playerCamping = isNear;

        if (!isNear)
        {
            // Do not reset the timer, so cumulative camping is counted
        }
    }

    IEnumerator ShowBackAwayText()
    {
        isShowingText = true;

        float flickerDuration = 2f;
        float flickerInterval = 0.05f;
        float elapsedTime = 0f;

        while (elapsedTime < flickerDuration)
        {
            backAwayText.gameObject.SetActive(!backAwayText.gameObject.activeSelf);
            yield return new WaitForSeconds(flickerInterval);
            elapsedTime += flickerInterval;
        }

        backAwayText.gameObject.SetActive(false);
        isShowingText = false;
    }

    private void TriggerCampingPunishment()
    {
        Debug.Log("Player camped too long! Trigger horror event here.");
    }
}
