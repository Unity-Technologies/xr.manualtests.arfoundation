using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class NavMeshPlaneManager : MonoBehaviour
{
	[SerializeField]
	private GameObject m_PlanePrefab;

	//private SessionComponent m_Session;

	private Dictionary<TrackableId, GameObject> m_Planes = new Dictionary<TrackableId, GameObject>();

    private XRPlaneSubsystem m_XRPlane;
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

	void Start()
	{
  //      m_Session = null;
  //      //m_Session = GetComponent<SessionComponent>();
		//if (m_Session != null)
		//	m_Session.SessionConnected += SessionConnectedHandler;
	}

	void SessionConnectedHandler()
	{
        //PlaneSubsystem = m_Session.PlaneSubsystem;
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

        //PlaneSubsystem.PlaneAdded += PlaneAddedHandler;
        //PlaneSubsystem.PlaneUpdated += PlaneUpdatedHandler;
        //PlaneSubsystem.PlaneRemoved += PlaneRemovedHandler;
    }

    void UnregisterPlaneCallbacks()
    {
        Debug.Log("UnregisterPlaneCallbacks");

        //PlaneSubsystem.PlaneAdded -= PlaneAddedHandler;
        //PlaneSubsystem.PlaneUpdated -= PlaneUpdatedHandler;
        //PlaneSubsystem.PlaneRemoved -= PlaneRemovedHandler;
    }

	void UpdatePlaneGameObject(BoundedPlane plane)
	{
		var planeGameObject = m_Planes[plane.Id];
		planeGameObject.transform.localPosition = plane.Center;
        planeGameObject.transform.localRotation = plane.Pose.rotation;
		planeGameObject.transform.localScale = new Vector3(2f, 1.5f, 2f);
        //planeGameObject.transform.localScale.Normalize();
        planeGameObject.AddComponent<NavMeshSurface>();
        planeGameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
        Debug.Log(planeGameObject.GetComponent<NavMeshSurface>().navMeshData.sourceBounds.ToString());
	}

    GameObject InstantiatePlanePrefab()
    {
        //if (m_Session.TrackablesParent == null)
        //{
        //    return Instantiate(m_PlanePrefab);
        //}
        //else
        //{
        //    return Instantiate(m_PlanePrefab, m_Session.TrackablesParent.transform);
        //}

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
