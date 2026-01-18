using UnityEngine;
public class pickupItems : MonoBehaviour
{
    [SerializeField] itemStats item;
   
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
           player.grabItem(item);
        }
    }
}
