using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null && playerPrefab != null)
            player = Instantiate(playerPrefab);

        if (player == null)
            return;

        var save = SaveState.Instance;
        if (save == null)
            return;

        var points = Object.FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);
        foreach (var p in points)
        {
            if (p.id == save.lastSpawnId)
            {
                player.transform.SetPositionAndRotation(p.transform.position, p.transform.rotation);
                return;
            }
        }

        foreach (var p in points)
        {
            if (p.id == "Default")
            {
                player.transform.SetPositionAndRotation(p.transform.position, p.transform.rotation);
                return;
            }
        }
    }
}
