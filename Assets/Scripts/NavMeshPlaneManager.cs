using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.XR;

public class NavMeshPlaneManager : MonoBehaviour
{
	[SerializeField]
	GameObject m_PlanePrefab;

	Dictionary<TrackableId, GameObject> m_Planes = new Dictionary<TrackableId, GameObject>();

    XRPlaneSubsystem m_XRPlane;
    public XRPlaneSubsystem PlaneSubsystem
	{
		get { return m_XRPlane; }
		set
		{
			if (m_XRPlane != value)
			{
				if (m_XRPlane != null && enabled)
				{
					UnregisterPlaneCallbacks();
				}

				m_XRPlane = value;

				if (m_XRPlane != null && enabled)
				{
					RegisterPlaneCallbacks();
				}
			}
		}
	}
    void OnEnable()
    {
        if (PlaneSubsystem != null)
            RegisterPlaneCallbacks();
    }

    void OnDisable()
    {
        if (PlaneSubsystem != null)
            UnregisterPlaneCallbacks();
    }

    void RegisterPlaneCallbacks()
    {
        Debug.Log("RegisterPlaneCallbacks");
    }

    void UnregisterPlaneCallbacks()
    {
        Debug.Log("UnregisterPlaneCallbacks");
    }

	void UpdatePlaneGameObject(BoundedPlane plane)
	{
		var planeGameObject = m_Planes[plane.Id];
		planeGameObject.transform.localPosition = plane.Center;
        planeGameObject.transform.localRotation = plane.Pose.rotation;
		planeGameObject.transform.localScale = new Vector3(2f, 1.5f, 2f);
        planeGameObject.AddComponent<NavMeshSurface>();
        planeGameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
        Debug.Log(planeGameObject.GetComponent<NavMeshSurface>().navMeshData.sourceBounds.ToString());
	}

    GameObject InstantiatePlanePrefab()
    {
        return null;
    }

    void PlaneAddedHandler(PlaneAddedEventArgs args)
    {
        var planeGameObject = InstantiatePlanePrefab();
        m_Planes.Add(args.Plane.Id, planeGameObject);
        UpdatePlaneGameObject(args.Plane);
    }

    void PlaneUpdatedHandler(PlaneUpdatedEventArgs args)
    {
        UpdatePlaneGameObject(args.Plane);
    }

    void PlaneRemovedHandler(PlaneRemovedEventArgs args)
    {
        var id = args.Plane.Id;
        Destroy(m_Planes[id]);
        m_Planes.Remove(id);
    }
}
