using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class GetawayDriver : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform arrivePoint;
    public Transform exitPoint;

    [Header("Timing")]
    public float driveInSpeed = 7f;
    public float idleSeconds = 3f;
    public float driveOutSpeed = 11f;

    [Header("Turning")]
    public bool rotateTowardTarget = true;
    public float turnSpeed = 6f;

    [Header("SceneTransition")]
    public string nextSceneName = "NextScene";
    public bool useFade = true;
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 0.6f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;

        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        yield return MoveTo(arrivePoint.position, driveInSpeed);
        yield return new WaitForSeconds(idleSeconds);
        yield return MoveTo(exitPoint.position, driveOutSpeed);

        if (useFade && fadeCanvasGroup != null)
            yield return Fade(1f, fadeDuration);

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
            fadeCanvasGroup.interactable = false;
            fadeCanvasGroup.gameObject.SetActive(false);
        }

        if (GameManager.instance != null)
            GameManager.instance.missionComplete();
    }

    IEnumerator MoveTo(Vector3 target, float speed)
    {
        while(Vector3.Distance(transform.position, target) > 0.15f)
        {
            Vector3 dir = (target - transform.position);
            dir.y = 0f;

            if (rotateTowardTarget && dir.sqrMagnitude > 0.001f)
            {
                Quaternion look = Quaternion.LookRotation(dir.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, turnSpeed * Time.deltaTime);
            } 

            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            yield return null;
        }
    }

    IEnumerator Fade(float targetAlpha, float duration)
    {
        float startAlpha = fadeCanvasGroup.alpha;
        float t = 0f;

        fadeCanvasGroup.blocksRaycasts = true;
        fadeCanvasGroup.interactable = false;

        while (t < duration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }
}
