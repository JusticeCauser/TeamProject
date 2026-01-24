using NUnit.Framework;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public static InventoryManager instance;

    [Header("Inventory UI")]
    [SerializeField] GameObject inventorySlot;
    [SerializeField] Transform inventoryBar;
    [SerializeField] Color selectedSlotColor = Color.yellow;
    [SerializeField] Color regularColor = Color.white;

    
    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
