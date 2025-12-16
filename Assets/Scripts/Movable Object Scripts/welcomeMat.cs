using UnityEngine;
using UnityEngine.InputSystem;

public class welcomeMat : MonoBehaviour
{
    [SerializeField] movableObject connectedObject;

    keyFunction key;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        key = connectedObject.GetKey();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameManager.instance.playerScript.useKey(key))
            {
                switch (key.GetKeyType())
                {
                    case keyType.moveKey:
                        connectedObject.SetIsMoving(true);
                        break;

                    case keyType.revealKey:
                        connectedObject.gameObject.SetActive(true);
                        break;
                }

                gameObject.SetActive(false);
            }
        }
    }
}
