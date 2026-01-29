using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryPersistenceBridge : MonoBehaviour
{
    [SerializeField] private string hubScene = "theHub";
    private readonly List<itemStats> savedItems = new List<itemStats>();
    private int savedTotalValue;
    private bool savedHasPrimaryObjective;

    private PlayerController lastSnapshotFrom;
    private int lastCount = -1;
    private int lastValue = -1;
    private bool lastPrimary;

    private PlayerController lastRestoredTo;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        var pc = PlayerController.instance;
        if (pc == null)
            return;

        int count = pc.itemList != null ? pc.itemList.Count : 0;
        int value = pc.totalValue;
        bool primary = pc.hasPrimaryObjective;

        if (pc != lastSnapshotFrom || count != lastCount || value != lastValue || primary != lastPrimary)
        {
            SaveFrom(pc);
            lastSnapshotFrom = pc;
            lastCount = count;
            lastValue = value;
            lastPrimary = primary;
        }

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == hubScene)
        {
            ResetRunNow();
            return;
        }
        StartCoroutine(RestoreNextFrame());
    }

    System.Collections.IEnumerator RestoreNextFrame()
    {
        for (int i = 0; i < 10 && PlayerController.instance == null; i++)
            yield return null;
        RestoreTo(PlayerController.instance);
    }

    private void SaveFrom(PlayerController pc)
    {
        savedItems.Clear();
        if (pc.itemList != null)
            savedItems.AddRange(pc.itemList);

        savedTotalValue = pc.totalValue;
        savedHasPrimaryObjective = pc.hasPrimaryObjective;
    }

    private void RestoreTo(PlayerController pc)
    {
        if (pc == null)
            return;

        if (pc == lastRestoredTo)
            return;

        if (pc.itemList == null)
            pc.itemList = new List<itemStats>();
        else
            pc.itemList.Clear();

        pc.itemList.AddRange(savedItems);
        pc.totalValue = savedTotalValue;
        pc.hasPrimaryObjective = savedHasPrimaryObjective;

        lastRestoredTo = pc;
    }

    public void ResetRunNow()
    {
        savedItems.Clear();
        savedTotalValue = 0;
        savedHasPrimaryObjective = false;

        lastSnapshotFrom = null;
        lastRestoredTo = null;
        lastCount = -1;
        lastValue = -1;
        lastPrimary = false;

        var pc = PlayerController.instance;
        if (pc != null)
        {
            if (pc.itemList != null)
                pc.itemList.Clear();
            pc.totalValue = 0;
            pc.hasPrimaryObjective = false;
        }

        if (CollectedTracker.Instance != null)
            CollectedTracker.Instance.ClearAll();

        if (SaveState.Instance != null)
            SaveState.Instance.lastSpawnId = "Default";
    }
}
