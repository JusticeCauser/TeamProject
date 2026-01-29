using System.Collections.Generic;
using UnityEngine;

public class CollectedTracker : MonoBehaviour
{
    public static CollectedTracker Instance;

    readonly HashSet<string> collected = new HashSet<string>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool IsCollected(string id) => !string.IsNullOrEmpty(id) && collected.Contains(id);
    public void MarkCollected(string id)
    {
         if (!string.IsNullOrEmpty(id))
             collected.Add(id);
    }

    public void ClearAll()
    {
        collected.Clear();
    }
}
