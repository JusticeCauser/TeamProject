using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class PickupPersistenceProxy : MonoBehaviour
{
    string id;
    bool playerInRange;

    void Awake()
    {
        id = $"{SceneManager.GetActiveScene().name}:{GetHierarchyPath(transform)}:{Quantize(transform.position)}";
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (CollectedTracker.Instance != null && CollectedTracker.Instance.IsCollected(id))
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerInRange)
            return;

        bool pressed = (InputManager.instance != null && InputManager.instance.interactKeyPressed()) || Input.GetKeyDown(KeyCode.E);

        if (!pressed)
            return;

        if (CollectedTracker.Instance != null)
            CollectedTracker.Instance.MarkCollected(id);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    static string GetHierarchyPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }

    static string Quantize(Vector3 v)
    {
        float q = 0.01f;
        int x = Mathf.RoundToInt(v.x / q);
        int y = Mathf.RoundToInt(v.y / q);
        int z = Mathf.RoundToInt(v.z / q);
        return $"{x},{y},{z}";
    }
}
