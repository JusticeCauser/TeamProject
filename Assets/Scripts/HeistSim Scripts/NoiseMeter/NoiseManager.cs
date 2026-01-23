using UnityEngine;

public class NoiseManager : MonoBehaviour
{
    [Header("Noise Value (0-100)")]
    [Range(0, 100)] public float noise = 0f;

    [Header("Smoothing")]
    public float riseSpeed = 120f;
    public float fallSpeed = 60f;

    [Header("Movement Targets")]
    public float idleNoise = 0f;
    public float crouchWalkNoise = 15f;
    public float walkNoise = 30f;
    public float sprintNoise = 65f;
    public float jumpLandSpike = 10f;

    [Header("Speed Sampling")]
    public CharacterController characterController;
    public float walkSpeedRef = 4f;
    public float sprintSpeedRef = 7f;

    [Header("State Inputs")]
    public bool isCrouching;
    public bool isSprinting;
    public bool isGrounded = true;

    [Header("HUD Fade")]
    public float fadeQuietThreshold = 10f;
    public float fadeDelay = 1.5f;
    public float fadedAlpha = 0.2f;
    public float fadeInSpeed = 8f;
    public float fadeOutSpeed = 3f;

    [Header("Debug")]
    public bool debugEnabled = true;
    public string lastSource = "None";
    public float lastSpikeAmount = 0f;

    public float HudAlpha01 {  get; private set; }
    public float TargetNoise {  get; private set; }

    float quietTimer = 0f;
    bool wasGroundedLastFrame = true;

    Vector3 lastPos;

    public static NoiseManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        lastPos = transform.position;
        HudAlpha01 = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        TargetNoise = ComputeTargetFromMovement();

        if (TargetNoise > noise)
            noise = Mathf.MoveTowards(noise, TargetNoise, riseSpeed * dt);
        else
            noise = Mathf.MoveTowards(noise, TargetNoise, fallSpeed * dt);

        noise = Mathf.Clamp(noise, 0f, 100f);

        if (!wasGroundedLastFrame && isGrounded)
        {
            AddSpike(jumpLandSpike, "Landing");
        }
        wasGroundedLastFrame = isGrounded;

        UpdateFade(dt);
    }

    float ComputeTargetFromMovement()
    {
        float speed = GetHorizontalSpeed();
        if (!isCrouching && speed > walkSpeedRef * 1.05f)
            isSprinting = true;
        float speed01 = 0f;

        float refMax = isSprinting ? sprintSpeedRef : walkSpeedRef;
        if (refMax > 0.01f)
            speed01 = Mathf.Clamp01(speed / refMax);

        if (speed < 0.05)
            return idleNoise;

        if (isCrouching)
        {
            return Mathf.Lerp(idleNoise, crouchWalkNoise, speed01);
        }

        float baseMove = Mathf.Lerp(idleNoise, walkNoise, speed01);
        if (isSprinting)
            return Mathf.Lerp(walkNoise, sprintNoise, speed01);

        return baseMove;
    }

    float GetHorizontalSpeed()
    {
        if (characterController != null)
        {
            Vector3 v = characterController.velocity;
            v.y = 0f;
            float ccSpeed = v.magnitude;

            if (ccSpeed > 0.01d)
                return ccSpeed;
        }

        float dt = Time.deltaTime; ;
        if (dt <= 0.0001f)
            return 0f;

        Vector3 delta = transform.position - lastPos;
        delta.y = 0f;

        lastPos = transform.position;
        return delta.magnitude / dt;
    }

    void UpdateFade(float dt)
    {
        if (noise < fadeQuietThreshold)
            quietTimer += dt;
        else
            quietTimer = 0f;

        float targetAlpha = (quietTimer >= fadeDelay) ? fadedAlpha : 1f;

        float speed = (targetAlpha > HudAlpha01) ? fadeInSpeed : fadeOutSpeed;
        HudAlpha01 = Mathf.MoveTowards(HudAlpha01, targetAlpha, speed * dt);
    }

    public void AddSpike(float amount, string source = "Spike")
    {
        noise = Mathf.Clamp(noise + amount, 0f, 100f);

        if (debugEnabled)
        {
            lastSource = source;
            lastSpikeAmount = amount;
        }
    }
}
