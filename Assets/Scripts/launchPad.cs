using UnityEngine;

public class launchPad : MonoBehaviour
{

    [SerializeField] public float force;

    

    // BELOW NONFUNCTIONAL
    // ========================================
    //private void OnTriggerEnter(Collider other)
    //{
    //    playerController player = other.GetComponent<playerController>();
    //    if (player != null)
    //    {
    //        Vector3 launchDirection = transform.up;
    //        player.Launch(launchDirection, force);
    //    }
    //}

}
