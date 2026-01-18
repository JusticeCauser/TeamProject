using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using System.Threading;

public class HackMinigameUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text statusText;
    public TMP_Text progressText;
    public Button lockInButton;

    [Header("Bar Parts")]
    public RectTransform barRect;
    public RectTransform successZone;
    public RectTransform marker;

    [Header("Minigame Settings")]
    public int requiredLocks = 3;
    public float markerSpeed = 450f;
    public float failPenaltySeconds = 0.25f;

    [Header("Input")]
    public KeyCode lockInKey = KeyCode.Space;

    [Header("PlayerInteractor")]
    public Interactor playerInteractor;
    public PromptUI promptUI;

    private KeypadTarget target;
    private int locks;
    private float dir = 1f;
    private bool Running;
    private float pauseTimer;

    private void Awake()
    {
        gameObject.SetActive(false);

        if (lockInButton != null)
            lockInButton.onClick.AddListener(LockIn);
    }

    public void Open(KeypadTarget target)
    {
        this.target = target;
        locks = 0;
        dir = 1f;
        pauseTimer = 0f;
        Running = true;

        UpdateProgress();
        SetStatus("Align the marker and Lock In!");
        gameObject.SetActive(true);

        if (promptUI != null)
            promptUI.Hide();
        if (playerInteractor != null)
        {
            playerInteractor.uiLocked = true;
            playerInteractor.SetInputCooldown(0.15f);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Input.ResetInputAxes();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        target = null;
        Running = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerInteractor != null)
        {
            playerInteractor.uiLocked = false;
            playerInteractor.SetInputCooldown(0.15f);
        }

        Input.ResetInputAxes();
    }

    // Update is called once per frame
    void Update()
    {
        if(!gameObject.activeSelf || !Running)
            return;

        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            return;
        }

        MoveMarker();

        if (Input.GetKeyDown(lockInKey))
            LockIn();
    }

    private void MoveMarker()
    {
        if (barRect == null || marker == null)
            return;

        float barWidth = barRect.rect.width;
        float markerHalf = marker.rect.width * 0.5f;

        Vector2 pos = marker.anchoredPosition;
        pos.x += dir * markerSpeed * Time.deltaTime;

        float left = -barWidth * 0.5f + markerHalf;
        float right = barWidth * 0.5f - markerHalf;

        if (pos.x <= left)
        {
            pos.x = left;
            dir = 1f;
        }

        if (pos.x >= right)
        {
            pos.x = right;
            dir = -1f;
        }

        marker.anchoredPosition = pos;
    }

    public void LockIn()
    {
        if (target == null || barRect == null || successZone == null || marker == null)
            return;

        float markerX = marker.anchoredPosition.x;

        float zoneCenterX = successZone.anchoredPosition.x;
        float zoneHalfWidth = successZone.rect.width * 0.5f;

        float zoneLeft = zoneCenterX - zoneHalfWidth;
        float zoneRight = zoneCenterX + zoneHalfWidth;

        bool success = (markerX >= zoneLeft && markerX <= zoneRight);

        if (success)
        {
            locks++;
            UpdateProgress();
            SetStatus("LOCK SUCCESS!");

            if (locks >= requiredLocks)
            {
                bool ok = target.TrySubmit(target.correctCode);
                SetStatus(ok ? "HACK COMPLETE: ACCESS GRANTED" : "HACK COMPLETE: (but code rejected)");
                Invoke(nameof(Close), 0.35f);
            }
        }
        else
        {
            SetStatus("MISS! Try again.");
            pauseTimer = failPenaltySeconds;
        }
    }

    private void UpdateProgress()
    {
        if (progressText != null)
            progressText.text = $"{locks}/{requiredLocks}";
    }

    private void SetStatus(string msg)
    {
        if (statusText != null)
            statusText.text = msg;
    }
}
