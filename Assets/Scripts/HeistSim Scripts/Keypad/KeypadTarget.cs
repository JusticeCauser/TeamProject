using UnityEngine;

public class KeypadTarget : MonoBehaviour
{
    [Header("Keypad Settings")]
    public string correctCode = "1234";
    public bool latchUnlocked = true;

    [Header("What this Keypad controls (Optional)")]
    public DoorController door;

    private bool unlocked;

    public bool TrySubmit(string entered)
    {
        if (unlocked && latchUnlocked)
            return true;

        bool ok = string.Equals(entered, correctCode);

        if(ok)
        {
            unlocked = true;
            if (door != null)
                door.Open();

            if (GameProgress.Instance != null)
                GameProgress.Instance.SetElevatorUnlocked(true);
        }
        return ok;
    }

    public bool IsUnlocked => unlocked;
}
