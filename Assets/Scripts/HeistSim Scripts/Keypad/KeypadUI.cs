using UnityEngine;
using TMPro;

public class KeypadUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text displayText;
    public TMP_Text messageText;

    [Header("Settings")]
    public int maxDigits = 6;
    public bool maskInput = false;
    public bool allowKeyboardInput = true;

    [Header("Interactor")]
    public Interactor playerInteractor;
    public PromptUI promptUI;

    private string current = "";
    private KeypadTarget activeTarget;

    public bool IsOpen => gameObject.activeSelf;

    private void Awake()
    {
        gameObject.SetActive(false);
        current = "";
        ShowDisplayMode();
        RefreshDisplay();
    }

    private void Update()
    {
        if (!IsOpen || !allowKeyboardInput)
            return;

        HandleKeyboardInput();
    }

    private void ShowDisplayMode()
    {
        if (displayText)
            displayText.gameObject.SetActive(true);
        if (messageText)
            messageText.gameObject.SetActive(false);
    }

    private void ShowMessageMode(string msg)
    {
        if (displayText)
            displayText.gameObject.SetActive(false);

        if(messageText)
        {
            messageText.gameObject.SetActive(true);
            messageText.text = msg;
        }
    }

    public void Open(KeypadTarget target)
    {
        activeTarget = target;
        current = "";
        gameObject.SetActive(true);

        if (promptUI != null)
            promptUI.Hide();
        if (playerInteractor != null)
        {
            playerInteractor.uiLocked = true;
            playerInteractor.SetInputCooldown(0.15f);
        }

        ShowMessageMode("Enter code");
        Invoke(nameof(ReturnToDisplay), 0.6f);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Input.ResetInputAxes();
    }

    public void Close()
    {
        gameObject.SetActive(false);
        activeTarget = null;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (playerInteractor != null)
        {
            playerInteractor.uiLocked = false;
            playerInteractor.SetInputCooldown(0.15f);
        }

        Input.ResetInputAxes();
    }

    public void PressDigit(string digit)
    {
        if (!IsOpen || current.Length >= maxDigits)
            return;

        ShowDisplayMode();

        if (digit.Length != 1 || digit[0] < '0' || digit[0] > '9')
            return;

        current += digit;
        RefreshDisplay();
    }

    private void HandleKeyboardInput()
    {
        for (KeyCode key = KeyCode.Alpha0; key <= KeyCode.Alpha9; key++)
        {
            if (Input.GetKeyDown(key))
            {
                PressDigit(((int)(key - KeyCode.Alpha0)).ToString());
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
            RemoveLastDigit();

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            Submit();

        if (Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    private void RemoveLastDigit()
    {
        if (current.Length == 0)
            return;

        current = current.Substring(0, current.Length - 1);
        RefreshDisplay();
    }

    public void Clear()
    {
        if (!IsOpen)
            return;

        ResetState();
    }

    public void Submit()
    {
        if (!IsOpen || activeTarget == null)
            return;
        bool ok = activeTarget.TrySubmit(current);
        if(ok)
        {
            ShowMessageMode("ACCESS GRANTED");
            Invoke(nameof(Close), 0.25f);
        }
        else
        {
            ShowMessageMode("ACCESS DENIED");
            current = "";
            Invoke(nameof(ReturnToDisplay), 0.6f);
        }
    }

    private void ReturnToDisplay()
    {
        if (!IsOpen)
            return;
        ShowDisplayMode();
        RefreshDisplay();
    }

    public void PressClose() => Close();

    private void ResetState()
    {
        current = "";
        ShowDisplayMode();
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        if (displayText == null)
            return;

        if (current.Length == 0)
            displayText.text = new string('_', Mathf.Max(4, maxDigits));
        else
            displayText.text = maskInput ? new string('*', current.Length) : current;
    }
}
