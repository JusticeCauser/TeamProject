using UnityEngine;
using TMPro;
using System.Collections;

public class secCam : MonoBehaviour
{
    [Header("Sweep Settings")]
    public float sweepSpeed = 30f;
    public float sweepAngle = 60f;

    [Header("Angle Settings")]
    public float downwardTilt = 30f;

    [Header("Light")]
    public GameObject camLight;
    public Color activeColor = Color.green;
    public Color disabledColor = Color.red;

    [Header("Interaction")]
    [SerializeField] TMP_Text promptText;
    public float jamDuration = 60f;

    private bool isSweeping = true;
    private bool isJammed = false;
    private bool playerInRange = false;
    private float currentAngle = 0f;
    private float jamTimer = 0f;
    private Renderer lightRenderer;
    private float initialYRotation;

    void Start()
    {
        initialYRotation = transform.localEulerAngles.y;

        currentAngle = sweepAngle;

        if (camLight != null)
            lightRenderer = camLight.GetComponent<Renderer>();

        if (promptText != null)
            promptText.gameObject.SetActive(false);

        UpdateLightColor();
    }

    void Update()
    {
        // sweeping
        if(isSweeping && !isJammed)
        {
            currentAngle += sweepSpeed * Time.deltaTime;
            float yRotation = initialYRotation + (Mathf.PingPong(currentAngle, sweepAngle * 2) - sweepAngle);
            transform.localRotation = Quaternion.Euler(downwardTilt, yRotation, 0);
        }

        // jamming
        if(isJammed)
        {
            jamTimer -= Time.deltaTime;

            if(jamTimer <= 0)
            {
                // reactiveate
                isJammed = false;
                isSweeping = true;
                StartCoroutine(RaiseCamera());
                //transform.localRotation = Quaternion.Euler(30, 0, 0); // raise back up
                UpdateLightColor();
            }
        }    
        // player interaction
        if(playerInRange && !isJammed && Input.GetKeyDown(KeyCode.E))
        {
            JamCamera();
        }
    }

    IEnumerator RaiseCamera()
    {
        Quaternion startRot = transform.localRotation;
        Quaternion endRot = Quaternion.Euler(downwardTilt, initialYRotation, 0);
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.localRotation = Quaternion.Lerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = endRot;
    }

    void JamCamera()
    {
        isJammed = true;
        isSweeping = false;
        jamTimer = jamDuration;

        StartCoroutine(DroopCamera());

        UpdateLightColor();
        //// droop down
        //transform.localRotation = Quaternion.Euler(80, transform.localEulerAngles.y, 0);

        //UpdateLightColor();

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    IEnumerator DroopCamera()
    {
        Quaternion startRot = transform.localRotation;
        Quaternion endRot = Quaternion.Euler(80, transform.localEulerAngles.y, 0);
        float duration = 0.5f;
        float elapsed = 0f;

        while(elapsed < duration)
        {
            transform.localRotation = Quaternion.Lerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = endRot;
    }    

    void UpdateLightColor()
    {
        if(lightRenderer != null)
        {
            lightRenderer.material.color = isJammed ? disabledColor : activeColor;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isJammed)
        {
            playerInRange = true;
            if(promptText != null)
            {
                promptText.text = "Press E to jam Camera";
                promptText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptText != null)
                promptText.gameObject.SetActive(false);
            
        }
    }
}
