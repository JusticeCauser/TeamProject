using System.Transactions;
using UnityEngine;

public enum keyType
{
moveKey,
revealKey
}

public class keyFunction : MonoBehaviour
{
    [SerializeField] keyType setKeyType;

    float rotateSpeed = 45; // Degrees per second.

    bool rotate = true;

    Vector3 rotationAxis = Vector3.right;

    void Update()
    {
        if(rotate)
            transform.Rotate(rotationAxis.normalized * rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        IPickup pickup = other.GetComponent<IPickup>();

        if(other.CompareTag("Player") && pickup != null)
        {
            pickup.addKey(this);
            Destroy(gameObject);
            rotate = false;
        }
    }

    public keyType GetKeyType()
    {
        return setKeyType;
    }
}
