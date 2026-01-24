//using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{

    public static InventoryManager instance;

    [Header("Scenes Active for Inventory")]
    [SerializeField] string mansion = "Mansion";
    [SerializeField] string asylum = "Asylum";

    string currScene;

    [Header("Inventory UI")]
    public GameObject inventoryUI;
    [SerializeField] GameObject inventorySlot;
    [SerializeField] Transform inventoryBar;
    [SerializeField] Color selectedSlotColor = Color.yellow;
    [SerializeField] Color regularColor = Color.white;

    bool slotsCreated = false;
    List<InventorySlotsManager> itemSlots = new List<InventorySlotsManager>();
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        createItemSlots();
        slotsCreated = true;

        currScene = SceneManager.GetActiveScene().name;

        if(currScene == mansion || currScene == asylum)
        inventoryUI.SetActive(true);
        else
            inventoryUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        updateInventoryBar();

        if(Input.GetKeyDown(KeyCode.G))
            dropSelectedItem();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += onSceneLoad;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= onSceneLoad;
    }
    void onSceneLoad(Scene scene, LoadSceneMode mode)
    {
        currScene = scene.name;

        if (currScene == mansion || currScene == asylum)
            inventoryUI.SetActive(true);
        else
            inventoryUI.SetActive(false);
       
    }
    void createItemSlots()
    {
        Debug.Log("Creating slots..." + inventorySlot + "Inventory bar..." + inventoryBar);

        for (int i = 0; i < 7;  i++)
        {
            GameObject slotObj = Instantiate(inventorySlot, inventoryBar);
            Debug.Log("spawned " + i + " " + slotObj.name);
            InventorySlotsManager slot = slotObj.GetComponent<InventorySlotsManager>();
            itemSlots.Add(slot);
        }
        Debug.Log("Slots created " + itemSlots.Count);
    }
    void updateInventoryBar()
    {
        if (PlayerController.instance == null)
            return;

        for(int i = 0; i < itemSlots.Count; i++)
        {
            if(i < PlayerController.instance.itemList.Count)
            {
                itemStats item = PlayerController.instance.itemList[i];
                itemSlots[i].updateInventorySlot(item);
            }
            else
                itemSlots[i].clearInventorySlot();

            bool slotSelected = i == PlayerController.instance.itemListPos;
            itemSlots[i].highlightInventorySlot(slotSelected, selectedSlotColor, regularColor);
        }
        
    }

    void dropSelectedItem()
    {
        if(PlayerController.instance == null) //checking for player
            return;

        //checking player inventory and position in hotbar
        List<itemStats> itemList = PlayerController.instance.itemList;
        int currPos = PlayerController.instance.itemListPos;

        if (itemList.Count == 0 || currPos < 0 || currPos >= itemList.Count)
            return;

        itemStats curr = itemList[currPos];

        if(curr.itemInGame != null)
        {
            Transform dropped = PlayerController.instance.droppedItem;

            if(dropped != null)
                Instantiate(curr.itemInGame, dropped.position, Quaternion.identity);
        }
        itemList.RemoveAt(currPos); //remove item
        PlayerController.instance.totalValue -= curr.itemValue; //remove value of item
        
        if (currPos >= itemList.Count && currPos > 0)
            PlayerController.instance.itemListPos--;
    }
}
