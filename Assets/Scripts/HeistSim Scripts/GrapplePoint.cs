using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    [Header("Grapple Point Settings")]
    [SerializeField] bool isActive = true;
    [SerializeField] float highlightRadius = 0.5f;

    [Header("Visual Feedback")]
    [SerializeField] GameObject visualIndicator;
    [SerializeField] Color inactiveColor = Color.gray;
    [SerializeField] Color activeColor = Color.white;
    [SerializeField] Color highlightColor = Color.yellow;

    private Renderer indicatorRenderer;
    private bool isHighlighted = false;

    private void Start()
    {
        // setup for visual indicator
        if(visualIndicator == null)
        {
            //sphereical indicator
            visualIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visualIndicator.transform.SetParent(transform);
            visualIndicator.transform.localPosition = Vector3.zero;
            visualIndicator.transform.localScale = Vector3.one * highlightRadius;

            // remove collider from indicator
            Collider col = visualIndicator.GetComponent<Collider>();
            if (col != null)
                Destroy(col);
        }

        indicatorRenderer = visualIndicator.GetComponent<Renderer>();

        if(indicatorRenderer != null)
        {
            // create new material, avoid modifying shared
            indicatorRenderer.material = new Material(indicatorRenderer.material);
            UpdateVisual();
        }
    }

    private void Update()
    {
        if (isHighlighted)
            UpdateVisual();
    }

    public void SetHighlight(bool highlight)
    {
        isHighlighted = highlight;
        UpdateVisual();
    }

    void UpdateVisual()
    {
        if (indicatorRenderer == null)
            return;

        if(!isActive)
        {
            indicatorRenderer.material.color = inactiveColor;
        }
        else if (isHighlighted)
        {
            // pulse when highlighted
            float pulse = (Mathf.Sin(Time.time * 5f) + 1f) / 2f;
            indicatorRenderer.material.color = Color.Lerp(activeColor, highlightColor, pulse);
        }
        else
        {
            indicatorRenderer.material.color = activeColor;
        }
    }    

    public bool IsActive()
    {
        return isActive;
    }

    public void SetActive(bool active)
    {
        isActive = active;
        UpdateVisual();
    }

    public Vector3 GetGrapplePosition()
    {
        return transform.position;
    }
}
