using System.Collections;
using UnityEngine;
using TMPro;

public class CampingSwitch : MonoBehaviour
{
    public TMP_Text backAwayText;
    public TMP_Text backAwayText2;
    public float timeNearSwitch = 0f;
    private bool isShowingBackAwayText = false;
    private bool isShowingBackAwayText2 = false;
    private bool playerCamping = false;

    public AudioSource whisper1;
    public AudioSource whisper2;
    public AudioSource whisper3;

    public float campingTimeThreshold1 = 5f;
    public float campingTimeThreshold2 = 15f;
    public float maxCampingTime = 30f; 

    private void Update()
    {
        if (playerCamping)
        {
            timeNearSwitch += Time.deltaTime;

            if (timeNearSwitch >= campingTimeThreshold1 && !isShowingBackAwayText)
            {
                whisper1.Play();
                StartCoroutine(ShowBackAwayText());
            }

            if(timeNearSwitch >= campingTimeThreshold2 && !isShowingBackAwayText2)
            {
                whisper2.Play();
                StartCoroutine(ShowBackAwayText2());
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
        isShowingBackAwayText = true;

        float flickerDuration = 2f;
        float flickerInterval = 0.08f;
        float elapsedTime = 0f;

        while (elapsedTime < flickerDuration)
        {
            backAwayText.gameObject.SetActive(!backAwayText.gameObject.activeSelf);
            yield return new WaitForSeconds(flickerInterval);
            elapsedTime += flickerInterval;
        }

        backAwayText.gameObject.SetActive(false);
    }

    IEnumerator ShowBackAwayText2()
    {
        isShowingBackAwayText2 = true;

        float flickerDuration = 3f;
        float flickerInterval = 0.05f;
        float elapsedTime = 0f;

        while (elapsedTime < flickerDuration)
        {
            backAwayText2.gameObject.SetActive(!backAwayText2.gameObject.activeSelf);
            yield return new WaitForSeconds(flickerInterval);
            elapsedTime += flickerInterval;
        }

        backAwayText2.gameObject.SetActive(false);
    }

    private void TriggerCampingPunishment()
    {
        Debug.Log("Player camped too long! Trigger horror event here.");
    }
}
