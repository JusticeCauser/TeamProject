using UnityEngine;

public class Crouch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CharacterController controller;

    [Header("Crouch Settings")]
    [SerializeField] float crouchHeight = 1f;
    [SerializeField] float standHeight = -1f; // -1 means auto-detect from CharacterController
    //[SerializeField] float crouchSpeed = 2f;
    [SerializeField] float crouchTransitionSpeed = 10f;
    

    float currentHeight;

    bool isCrouching;
    public bool IsCrouching => isCrouching;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isCrouching = false;

        if (standHeight <= 0)
            standHeight = controller.height;

        currentHeight = standHeight;
    }

    // Update is called once per frame
    void Update()
    {
        handleInput();
        updateHeight();
    }

    void handleInput()
    {
        bool crouchToggle = Input.GetButtonDown("Crouch");
        
        //bool crouchReleased = Input.GetButtonDown("Crouch");

        if (crouchToggle)
        {
            if (!isCrouching)
            {
                isCrouching = true;

                if (ObjectiveManager.instance != null)
                    ObjectiveManager.instance.playerCrouched();
            }
            else if (canStand())
            {
                isCrouching = false;
            }
                
        }
    }
    void updateHeight()
    {
        float targetHeight = isCrouching ? crouchHeight : standHeight;
        currentHeight = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * crouchTransitionSpeed);

        controller.height = currentHeight;
    }
    bool canStand()
    {
        float checkHeight = standHeight - crouchHeight;
        Vector3 checkPos = transform.position + Vector3.up * crouchHeight;
        return !Physics.SphereCast(checkPos, controller.radius * 0.9f, Vector3.up, out _, checkHeight);
    }
}