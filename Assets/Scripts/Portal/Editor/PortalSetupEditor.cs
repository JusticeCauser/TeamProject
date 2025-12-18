#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class PortalSetupEditor : EditorWindow
{
    [MenuItem("Tools/Portal System/Create Portal Pair")]
    static void CreatePortalPair()
    {
        GameObject pairObj = new GameObject("PortalPair");
        PortalPair pair = pairObj.AddComponent<PortalPair>();

        GameObject portalAObj = CreatePortalObject("Portal_A", new Vector3(-2, 1.5f, 0));
        GameObject portalBObj = CreatePortalObject("Portal_B", new Vector3(2, 1.5f, 0));
        portalBObj.transform.Rotate(0, 180, 0);

        portalAObj.transform.SetParent(pairObj.transform);
        portalBObj.transform.SetParent(pairObj.transform);

        Portal portalA = portalAObj.GetComponent<Portal>();
        Portal portalB = portalBObj.GetComponent<Portal>();

        portalA.SetLinkedPortal(portalB);
        portalB.SetLinkedPortal(portalA);

        pair.SetPortals(portalA, portalB);

        Selection.activeGameObject = pairObj;
        Undo.RegisterCreatedObjectUndo(pairObj, "Create Portal Pair");

        Debug.Log("Portal Pair created. Position the portals and make sure they face the direction players will approach from.");
    }

    [MenuItem("Tools/Portal System/Create Single Portal")]
    static void CreateSinglePortal()
    {
        GameObject portalObj = CreatePortalObject("Portal", Vector3.zero + Vector3.up * 1.5f);
        Selection.activeGameObject = portalObj;
        Undo.RegisterCreatedObjectUndo(portalObj, "Create Portal");
    }

    [MenuItem("Tools/Portal System/Create Portal Manager")]
    static void CreatePortalManager()
    {
        if (Object.FindFirstObjectByType<PortalManager>() != null)
        {
            EditorUtility.DisplayDialog("Portal Manager Exists", "A PortalManager already exists in the scene.", "OK");
            return;
        }

        GameObject managerObj = new GameObject("PortalManager");
        managerObj.AddComponent<PortalManager>();
        Selection.activeGameObject = managerObj;
        Undo.RegisterCreatedObjectUndo(managerObj, "Create Portal Manager");
        Debug.Log("PortalManager created.");
    }

    [MenuItem("Tools/Portal System/Setup Player for Portals")]
    static void SetupPlayerForPortals()
    {
        playerController player = Object.FindFirstObjectByType<playerController>();
        if (player == null)
        {
            EditorUtility.DisplayDialog("No Player Found", "Could not find a playerController in the scene.", "OK");
            return;
        }

        if (player.GetComponent<PortalTraveler>() == null)
        {
            Undo.AddComponent<PortalTraveler>(player.gameObject);
            Debug.Log("PortalTraveler added to player: " + player.gameObject.name);
            EditorUtility.DisplayDialog("Success", "PortalTraveler component added to player.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Already Setup", "Player already has PortalTraveler component.", "OK");
        }
    }

    static GameObject CreatePortalObject(string name, Vector3 localPosition)
    {
        GameObject portalObj = new GameObject(name);
        portalObj.transform.localPosition = localPosition;

        GameObject screen = GameObject.CreatePrimitive(PrimitiveType.Quad);
        screen.name = "PortalScreen";
        screen.transform.SetParent(portalObj.transform);
        screen.transform.localPosition = Vector3.zero;
        screen.transform.localRotation = Quaternion.identity;
        screen.transform.localScale = new Vector3(2f, 3f, 1f);

        Object.DestroyImmediate(screen.GetComponent<Collider>());

        GameObject trigger = new GameObject("PortalTrigger");
        trigger.transform.SetParent(portalObj.transform);
        trigger.transform.localPosition = Vector3.zero;
        trigger.transform.localRotation = Quaternion.identity;

        BoxCollider box = trigger.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(2f, 3f, 1f);
        box.center = Vector3.zero;

        Rigidbody rb = trigger.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        trigger.AddComponent<PortalTriggerZone>();

        Portal portal = portalObj.AddComponent<Portal>();
        portal.portalScreen = screen.GetComponent<MeshRenderer>();
        portal.portalTrigger = box;

        return portalObj;
    }
}
#endif
