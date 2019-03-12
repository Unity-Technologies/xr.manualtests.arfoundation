using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class NavMeshTest : MonoBehaviour
{
    [SerializeField]
    private GameObject m_ObjectToPlace;

    ARRaycastManager m_RaycastManager;

    private List<ARRaycastHit> m_RaycastHits = new List<ARRaycastHit>();
    private bool objectSpawned = false;
    private GameObject spawnedObject;
    private ARPlaneManager planeManager;

    void OnEnable()
    {
        planeManager = GetComponent<ARPlaneManager>();
        m_RaycastManager = GetComponent<ARRaycastManager>();

        if (planeManager != null)
            RegisterPlaneCallbacks();
    }

    void OnDisable()
    {
        if (planeManager != null)
            UnregisterPlaneCallbacks();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_RaycastManager == null)
            return;

        if (m_ObjectToPlace == null)
            return;

        if (Input.GetMouseButton(0))
        {
            if (m_RaycastManager.Raycast(Input.mousePosition, m_RaycastHits, TrackableType.PlaneWithinBounds))
            {
                if (!objectSpawned)
                {
                    Debug.Log(m_RaycastHits[0].pose.ToString());
                    spawnedObject = Instantiate(m_ObjectToPlace, m_RaycastHits[0].pose.position, m_RaycastHits[0].pose.rotation);

                    NavMeshHit closestHit;
                    if (NavMesh.SamplePosition(m_RaycastHits[0].pose.position, out closestHit, 1.0f, NavMesh.AllAreas))
                    {
                        Debug.Log("Spawning and placing character on NavMesh");

                        spawnedObject.transform.position = closestHit.position;
                        spawnedObject.AddComponent<NavMeshAgent>();
                        //spawnedObject.GetComponent<NavMeshAgent>().radius = .1f;
                        spawnedObject.GetComponent<NavMeshAgent>().Warp(m_RaycastHits[0].pose.position);
                        if (spawnedObject.GetComponent<NavMeshAgent>().isOnNavMesh == true)
                        {
                            objectSpawned = true;
                            spawnedObject.GetComponent<NavMeshAgent>().speed = 15f;
                        }
                        else
                        {
                            DestroyImmediate(spawnedObject);
                        }
                    }
                    else
                    {
                        Debug.LogError("No appropriate navmesh position found");
                        DestroyImmediate(spawnedObject);
                    }
                }
                else
                {
                    spawnedObject.GetComponent<NavMeshAgent>().destination = m_RaycastHits[0].pose.position;
                    Debug.Log(string.Format("Moving to: {0}", m_RaycastHits[0].pose.position));
                }
            }
        }
    }
    void RegisterPlaneCallbacks()
    {
        Debug.Log("RegisterPlaneCallback");
        planeManager.planesChanged += PlanesChangedHandler;
    }

    void UnregisterPlaneCallbacks()
    {
        Debug.Log("UnregisterPlaneCallback");
        planeManager.planesChanged -= PlanesChangedHandler;
    }

    void PlanesChangedHandler(ARPlanesChangedEventArgs eventArgs)
    {
        foreach (var plane in eventArgs.added)
        {
            plane.gameObject.AddComponent<NavMeshSurface>();
            plane.gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
            Debug.Log(plane.gameObject.GetComponent<NavMeshSurface>().navMeshData.sourceBounds.ToString());
        }

        foreach (var plane in eventArgs.updated)
        {
            plane.gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }
}
