using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Portal Link")]
    [SerializeField] Portal linkedPortal;

    [Header("Components")]
    public MeshRenderer portalScreen;
    public Collider portalTrigger;

    [Header("Rendering")]
    [SerializeField] int textureResolution = 1024;

    Camera portalCamera;
    RenderTexture portalTexture;
    Camera mainCamera;
    Material portalMaterial;

    Dictionary<Collider, TravelerData> travelersInPortal = new Dictionary<Collider, TravelerData>();

    struct TravelerData
    {
        public IPortalTraveler traveler;
        public float previousDot;
        public bool hasCrossed;
    }

    public Portal LinkedPortal => linkedPortal;
    public Camera PortalCamera => portalCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (portalScreen == null)
        {
            Transform screenT = transform.Find("PortalScreen");
            if (screenT != null)
                portalScreen = screenT.GetComponent<MeshRenderer>();
            if (portalScreen == null)
                portalScreen = GetComponentInChildren<MeshRenderer>();
        }

        if (portalTrigger == null)
        {
            Transform triggerT = transform.Find("PortalTrigger");
            if (triggerT != null)
                portalTrigger = triggerT.GetComponent<Collider>();
            if (portalTrigger == null)
                portalTrigger = GetComponentInChildren<Collider>();
        }

        if (portalScreen == null)
            Debug.LogError($"Portal {name}: No PortalScreen MeshRenderer found!");
        if (portalTrigger == null)
            Debug.LogError($"Portal {name}: No PortalTrigger Collider found!");

        SetupPortalCamera();
        SetupPortalTexture();

        StartCoroutine(RegisterWithManager());
    }

    System.Collections.IEnumerator RegisterWithManager()
    {
        yield return null;
        if (PortalManager.instance != null)
            PortalManager.instance.RegisterPortal(this);
    }

    void OnDestroy()
    {
        if (PortalManager.instance != null)
            PortalManager.instance.UnregisterPortal(this);

        if (portalTexture != null)
        {
            portalTexture.Release();
            Destroy(portalTexture);
        }

        if (portalMaterial != null)
            Destroy(portalMaterial);
    }

    void OnValidate()
    {
        if (linkedPortal == this)
        {
            linkedPortal = null;
        }

        if (linkedPortal != null && linkedPortal.linkedPortal != this)
        {
            linkedPortal.linkedPortal = this;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(linkedPortal);
#endif
        }
    }

    void SetupPortalCamera()
    {
        GameObject camObj = new GameObject("PortalCamera");
        camObj.transform.SetParent(transform);
        portalCamera = camObj.AddComponent<Camera>();
        portalCamera.enabled = false;

        if (mainCamera != null)
        {
            portalCamera.fieldOfView = mainCamera.fieldOfView;
            portalCamera.nearClipPlane = 0.1f;
            portalCamera.farClipPlane = mainCamera.farClipPlane;
            portalCamera.cullingMask = mainCamera.cullingMask;
        }
    }

    void SetupPortalTexture()
    {
        portalTexture = new RenderTexture(textureResolution, textureResolution, 24);
        portalTexture.Create();

        portalCamera.targetTexture = portalTexture;

        Shader portalShader = Shader.Find("Custom/PortalSurface");
        if (portalShader == null)
            portalShader = Shader.Find("Unlit/Texture");

        if (portalShader != null)
        {
            portalMaterial = new Material(portalShader);
            portalMaterial.mainTexture = portalTexture;
        }
        else
        {
            portalMaterial = new Material(Shader.Find("Standard"));
            portalMaterial.mainTexture = portalTexture;
        }

        if (portalScreen != null)
            portalScreen.material = portalMaterial;
    }

    void LateUpdate()
    {
        if (linkedPortal == null || portalCamera == null || mainCamera == null)
            return;

        if (!IsVisibleFrom(mainCamera))
            return;

        Render();
    }

    public void Render()
    {
        if (linkedPortal == null || portalCamera == null || mainCamera == null)
            return;

        UpdateCameraPosition();
        SetObliqueProjection();
        portalCamera.Render();
    }

    void UpdateCameraPosition()
    {
        Matrix4x4 m = linkedPortal.transform.localToWorldMatrix
                    * Matrix4x4.Rotate(Quaternion.Euler(0, 180, 0))
                    * transform.worldToLocalMatrix
                    * mainCamera.transform.localToWorldMatrix;

        portalCamera.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
    }

    void SetObliqueProjection()
    {
        Transform clipPlane = linkedPortal.transform;

        int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward,
            clipPlane.position - portalCamera.transform.position));

        Vector3 camSpacePos = portalCamera.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 camSpaceNormal = portalCamera.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;

        float camSpaceDist = -Vector3.Dot(camSpacePos, camSpaceNormal);
        if (Mathf.Abs(camSpaceDist) < 0.01f)
            return;

        Vector4 clipPlaneCamSpace = new Vector4(
            camSpaceNormal.x,
            camSpaceNormal.y,
            camSpaceNormal.z,
            camSpaceDist
        );

        Matrix4x4 projection = mainCamera.projectionMatrix;
        portalCamera.projectionMatrix = CalculateObliqueMatrix(projection, clipPlaneCamSpace);
    }

    Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane)
    {
        Vector4 q = projection.inverse * new Vector4(
            Mathf.Sign(clipPlane.x),
            Mathf.Sign(clipPlane.y),
            1f,
            1f
        );
        Vector4 c = clipPlane * (2f / Vector4.Dot(clipPlane, q));

        projection[2] = c.x - projection[3];
        projection[6] = c.y - projection[7];
        projection[10] = c.z - projection[11];
        projection[14] = c.w - projection[15];

        return projection;
    }

    public bool IsVisibleFrom(Camera cam)
    {
        if (cam == null) return false;
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        if (portalScreen != null)
            return GeometryUtility.TestPlanesAABB(planes, portalScreen.bounds);
        return GeometryUtility.TestPlanesAABB(planes, new Bounds(transform.position, Vector3.one * 2));
    }

    public Matrix4x4 GetTransformMatrix()
    {
        return linkedPortal.transform.localToWorldMatrix
             * Matrix4x4.Rotate(Quaternion.Euler(0, 180, 0))
             * transform.worldToLocalMatrix;
    }

    public void OnTravelerEnterTrigger(Collider other)
    {
        if (linkedPortal == null) return;

        IPortalTraveler traveler = other.GetComponent<IPortalTraveler>();
        if (traveler == null)
            traveler = other.GetComponentInParent<IPortalTraveler>();

        if (traveler == null) return;

        Debug.Log($"Portal: {name} - Traveler entered: {other.name}");

        Vector3 toTraveler = traveler.GetTransform().position - transform.position;
        float initialDot = Vector3.Dot(toTraveler, transform.forward);

        travelersInPortal[other] = new TravelerData
        {
            traveler = traveler,
            previousDot = initialDot,
            hasCrossed = false
        };

        traveler.OnPortalEnter(this);
    }

    public void OnTravelerStayTrigger(Collider other)
    {
        if (!travelersInPortal.TryGetValue(other, out TravelerData data))
            return;

        if (data.hasCrossed) return;

        Vector3 toTraveler = data.traveler.GetTransform().position - transform.position;
        float currentDot = Vector3.Dot(toTraveler, transform.forward);

        if (data.previousDot > 0 && currentDot <= 0)
        {
            Debug.Log($"Portal: {name} - Teleporting (front to back)");
            TeleportTraveler(data.traveler);
            data.hasCrossed = true;
            travelersInPortal[other] = data;
            return;
        }

        if (data.previousDot < 0 && currentDot >= 0)
        {
            Debug.Log($"Portal: {name} - Teleporting (back to front)");
            TeleportTraveler(data.traveler);
            data.hasCrossed = true;
            travelersInPortal[other] = data;
            return;
        }

        data.previousDot = currentDot;
        travelersInPortal[other] = data;
    }

    public void OnTravelerExitTrigger(Collider other)
    {
        if (travelersInPortal.TryGetValue(other, out TravelerData data))
        {
            Debug.Log($"Portal: {name} - Traveler exited: {other.name}");
            data.traveler.OnPortalExit(this);
            travelersInPortal.Remove(other);
        }
    }

    void TeleportTraveler(IPortalTraveler traveler)
    {
        Matrix4x4 m = GetTransformMatrix();

        Transform t = traveler.GetTransform();
        Vector3 newPos = m.MultiplyPoint3x4(t.position);
        Quaternion newRot = m.rotation * t.rotation;
        Vector3 newVel = m.MultiplyVector(traveler.GetVelocity());

        traveler.Teleport(newPos, newRot, newVel, this, linkedPortal);
    }

    public void SetLinkedPortal(Portal portal)
    {
        linkedPortal = portal;
    }

    public void SetBorderColor(Color color)
    {
        if (portalMaterial != null)
            portalMaterial.SetColor("_BorderColor", color);
    }
}
