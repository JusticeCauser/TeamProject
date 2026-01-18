using UnityEngine;

public class printManager : MonoBehaviour
{
    public static printManager instance;
    public bool hasFingerprint = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
}
