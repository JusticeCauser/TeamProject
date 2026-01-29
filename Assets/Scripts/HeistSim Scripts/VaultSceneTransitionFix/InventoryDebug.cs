using UnityEngine;

public class InventoryDebug : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8) && PlayerController.instance != null)
        {
            Debug.Log("Items: " + PlayerController.instance.itemList.Count + " | Value: " + PlayerController.instance.totalValue);
        }
    }
}
