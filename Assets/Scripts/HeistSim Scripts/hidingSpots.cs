using UnityEngine;

public class HidingSpots : MonoBehaviour
{

    bool isHiding;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void hide(PlayerController player)
    {
        isHiding = true;
        player.gameObject.SetActive(false);
    }
    void exitHide(PlayerController player)
    {
        isHiding = false;
        player.gameObject.SetActive(true);
    }
}
