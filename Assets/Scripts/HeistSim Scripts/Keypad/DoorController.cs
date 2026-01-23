using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door Movement")]
    public Transform doorTransform;
    public Vector3 openLocalOffset = new Vector3(0f, 0f, 2f);
    public float openCloseSpeed = 4f;

    public bool startClosed = true;

    private Vector3 closedLocalPos;
    private Vector3 openLocalPos;
    private bool IsOpen;

    private void Awake()
    {
        if (doorTransform == null)
            doorTransform = transform;

        closedLocalPos = doorTransform.localPosition;
        openLocalPos = closedLocalPos + openLocalOffset;

        IsOpen = !startClosed;
        doorTransform.localPosition = isOpen ? openLocalPos : closedLocalPos;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 target = IsOpen ? openLocalPos : closedLocalPos;
        doorTransform.localPosition = Vector3.Lerp(doorTransform.localPosition, target, Time.deltaTime * openCloseSpeed);
    }

    public void Open() => IsOpen = true;
    public void Close() => IsOpen = false;
    public void Toggle() => IsOpen = !IsOpen;
    public bool isOpen => IsOpen;
}
