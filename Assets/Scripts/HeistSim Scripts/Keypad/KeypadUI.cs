using UnityEngine;
using TMPro;
using UnityEditor.Rendering;

public class KeypadUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text displayText;
    public TMP_Text messageText;

    [Header("Settings")]
    public int maxDigits = 6;
    public bool maskInput = false;
    public bool allowKeyboardInput = true;

    private string current = "";
    private KeypadTarget activeTarget;

    public bool IsOpen => gameObject.activeSelf;

    private void Awake()
    {
        gameObject.SetActive(false);
        RefreshDisplay();
    }

    private void Update()
    {
        if (!IsOpen || !allowKeyboardInput)
            return;

        HandleKeyboardInput();
    }

    public void Open(KeypadTarget target)
    {
        activeTarget = target;
        ResetState();
        SetMessage("Enter code");
        gameObject.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        activeTarget = null;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void PressDigit(string digit)
    {
        if (!IsOpen || current.Length >= maxDigits)
            return;

        if (digit.Length != 1 || digit[0] < '0' || digit[0] > '9')
            return;

        current += digit;
        SetMessage("");
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
            SetMessage("ACCESS GRANTED");
            Invoke(nameof(Close), 0.25f);
        }
        else
        {
            SetMessage("ACCESS DENIED");
            current = "";
            RefreshDisplay();
        }
    }

    public void PressClose() => Close();

    private void ResetState()
    {
        current = "";
        SetMessage("");
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        if (displayText == null)
            return;

        if (current.Length == 0)
            displayText.text = "____";
        else
            displayText.text = maskInput ? new string('*', current.Length) : current;
    }

    private void SetMessage(string msg)
    {
        if (messageText != null)
            messageText.text = msg;
    }
}
