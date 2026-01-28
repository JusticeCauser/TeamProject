using UnityEngine;
using UnityEngine.UI;

public class ScrollManager : MonoBehaviour
{

    [Header("Scroll Settings")]
    [SerializeField] GameObject backButton;
    [SerializeField] RectTransform text;
    [SerializeField] float speed = 75f;
    [SerializeField] float wait = 1f;
    [SerializeField] float yStart = -1600f;
    [SerializeField] float yEnd = 900f;

    float timer = 0f;

    bool creditsOver = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     if(text == null)
            text = GetComponent<RectTransform>();

        text.anchoredPosition = new Vector2(0, yStart);

        timer = wait;

        if(backButton != null)
            backButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (creditsOver)
            return;

        if(timer > 0)
        {
            timer -= Time.unscaledDeltaTime;
            return; 
        }

        float currY = text.anchoredPosition.y;
        currY += speed * Time.unscaledDeltaTime;
        text.anchoredPosition = new Vector2(0, currY);

        if(currY >= yEnd)
        {
            creditsOver = true;

            if(backButton != null)
                backButton.SetActive(true);
        }
    }

    public void resetCredits()
    {
        creditsOver = false;
        timer = wait;
        text.anchoredPosition = new Vector2(0, yStart);

        if(backButton != null)
            backButton.SetActive(false);
    }
}
