using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotsManager : MonoBehaviour
{
    [Header("Slots Manager")]
    [SerializeField] Image slotsBackground;
    [SerializeField] TMP_Text itemText;
   

    public void updateInventorySlot(itemStats item)
    {
        if(itemText.text != null)
            itemText.text = item.itemName;
    }

    public void clearInventorySlot()
    {
        if (itemText != null)
            itemText.text = "";
    }

    public void highlightInventorySlot(bool selectedSlot, Color selectedSLotColor, Color regularColor)
    {
        if(slotsBackground != null)
            slotsBackground.color = selectedSlot ? selectedSLotColor : regularColor;
    }
}
