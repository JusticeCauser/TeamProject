using UnityEngine;
using TMPro;
using System.Collections;

public class FeedbackUI : MonoBehaviour
{
    public static FeedbackUI instance;

    [SerializeField] TMP_Text feedbackText;
    [SerializeField] float displayDuration = 2f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (feedbackText == null)
            feedbackText.gameObject.SetActive(false);
    }

    public void ShowFeedback(string message)
    {
        if(feedbackText != null)
        {
            StopAllCoroutines();
            feedbackText.text = message;
            feedbackText.gameObject.SetActive(true);
            StartCoroutine(HideFeedback());
        }
    }

    IEnumerator HideFeedback()
    {
        yield return new WaitForSeconds(displayDuration);
        feedbackText.gameObject.SetActive(false);
    }
}
