using UnityEngine;

public class CinematicCameraPulse : MonoBehaviour
{
    public Camera cam;
    public float zoomFOV = 50f;
    public float speed = 1.5f;

    float originalFOV;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalFOV = cam.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoomFOV, Time.deltaTime * speed);
    }
}
