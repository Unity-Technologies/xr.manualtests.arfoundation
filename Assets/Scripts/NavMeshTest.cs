using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARSessionOrigin))]
public class NavMeshTest : MonoBehaviour
{
    [SerializeField]
    private GameObject m_ObjectToPlace;

    private ARSessionOrigin m_Origin;
    private List<ARRaycastHit> m_RaycastHits = new List<ARRaycastHit>();
    private bool objectSpawned = false;
    private GameObject spawnedObject;
    private ARPlaneManager planeManager;

    // Use this for initialization
    void Start()
    {
        m_Origin = GetComponent<ARSessionOrigin>();
    }

    void OnEnable()
    {
        planeManager = GetComponent<ARPlaneManager>();

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
        if (m_Origin == null)
            return;

        if (m_Origin.camera == null)
            return;

        if (m_ObjectToPlace == null)
            return;

        if (Input.GetMouseButton(0))
        {
            if (m_Origin.Raycast(Input.mousePosition, m_RaycastHits, TrackableType.PlaneWithinBounds))
            {
                if (!objectSpawned)
                {
                    Debug.Log(m_RaycastHits[0].pose.ToString());
                    spawnedObject = Instantiate(m_ObjectToPlace, m_RaycastHits[0].pose.position, m_RaycastHits[0].pose.rotation);
                    spawnedObject.transform.localScale = new Vector3(.2f, .2f, .2f);

                    NavMeshHit closestHit;
                    if (NavMesh.SamplePosition(m_RaycastHits[0].pose.position, out closestHit, 1.0f, NavMesh.AllAreas))
                    {
                        Debug.Log("Spawning and placing character on NavMesh");

                        spawnedObject.transform.position = closestHit.position;
                        spawnedObject.AddComponent<NavMeshAgent>();
                        //spawnedObject.GetComponent<NavMeshAgent>().radius = .01f;
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
        Debug.Log("RegisterPlaneCallbacks");
        planeManager.planeAdded += PlaneAddedHandler;
        planeManager.planeUpdated += PlaneUpdatedHandler;
        planeManager.planeRemoved += PlaneRemovedHandler;
    }

    void UnregisterPlaneCallbacks()
    {
        Debug.Log("UnregisterPlaneCallbacks");
        planeManager.planeAdded -= PlaneAddedHandler;
        planeManager.planeUpdated -= PlaneUpdatedHandler;
        planeManager.planeRemoved -= PlaneRemovedHandler;
    }

    void PlaneAddedHandler(ARPlaneAddedEventArgs args)
    {
        UpdatePlane(args.plane.gameObject);
        args.plane.gameObject.AddComponent<NavMeshSurface>();
        //args.plane.gameObject.GetComponent<NavMeshSurface>().overrideVoxelSize = true;
        //args.plane.gameObject.GetComponent<NavMeshSurface>().voxelSize = 6;
        args.plane.gameObject.GetComponent<NavMeshSurface>().overrideTileSize = true;
        args.plane.gameObject.GetComponent<NavMeshSurface>().tileSize = 64;
        Debug.Log(string.Format("NavMesh Voxel Size: {0}", args.plane.gameObject.GetComponent<NavMeshSurface>().voxelSize));
        Debug.Log(string.Format("NavMesh Tile Size: {0}", args.plane.gameObject.GetComponent<NavMeshSurface>().tileSize));
        args.plane.gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
        Debug.Log(string.Format("Plane Scale: {0}", args.plane.gameObject.transform.localScale));
        Debug.Log(string.Format("Plane Pose: {0}", args.plane.gameObject.transform.position));
        Debug.Log(string.Format("NavMesh Source Bounds: {0}", args.plane.gameObject.GetComponent<NavMeshSurface>().navMeshData.sourceBounds.ToString()));
    }

    void PlaneUpdatedHandler(ARPlaneUpdatedEventArgs args)
    {
        UpdatePlane(args.plane.gameObject);
        args.plane.gameObject.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    void PlaneRemovedHandler(ARPlaneRemovedEventArgs args)
    {

    }

    void UpdatePlane(GameObject plane)
    {

    }
}
