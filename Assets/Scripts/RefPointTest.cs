using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

[RequireComponent(typeof(ARReferencePointManager))]
public class RefPointTest : MonoBehaviour
{
    [SerializeField]
    private GameObject m_ObjectToPlace;
    [SerializeField]
    private Material m_BadRefPointMaterial;

    private ARReferencePointManager m_RefManager;
    private ARSessionOrigin m_Origin;
    private List<ARReferencePoint> m_TrackableIds = new List<ARReferencePoint>();
    private List<ARRaycastHit> m_RaycastHits = new List<ARRaycastHit>();
    private List<Pose> m_raycastHitPoses = new List<Pose>();

    // Use this for initialization
    void Start()
    {
        m_RefManager = GetComponent<ARReferencePointManager>();
        m_Origin = GetComponent<ARSessionOrigin>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (m_RefManager == null)
            return;

        if (m_Origin.camera == null)
            return;
        
        if (m_ObjectToPlace == null)
            return;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            // if (m_Session.Raycast(ray, m_RaycastHits, TrackableType.PlaneWithinPolygon))
            if (m_Origin.Raycast(Input.mousePosition, m_RaycastHits, TrackableType.PlaneWithinBounds))
            {
                Debug.LogFormat("Hit Position: {0}", m_RaycastHits[0].pose);

                m_TrackableIds.Add(m_RefManager.TryAddReferencePoint(m_RaycastHits[0].pose));
                m_raycastHitPoses.Add(m_RaycastHits[0].pose);

                //m_ObjectToPlace.transform.position = m_RaycastHits[0].Pose.position;

                //var pos = new Vector3(m_ObjectToPlace.transform.position.x, m_ObjectToPlace.transform.position.y + 0.05f,
                //    m_ObjectToPlace.transform.position.z);

                //m_ObjectToPlace.transform.position = pos;
                //m_ObjectToPlace.transform.rotation = m_RaycastHits[0].Pose.rotation;
            }
        }
    }

    public void TestReferencePoints()
    {
        List<ARReferencePoint> referencePoints = new List<ARReferencePoint>();
        m_RefManager.GetAllReferencePoints(referencePoints);

        bool match = false;
        foreach (ARReferencePoint reference in referencePoints)
        {
            match = false;
            Debug.LogFormat("ReferencePoint Position: {0}", reference.transform.position.ToString());

            foreach(Pose raycastPose in m_raycastHitPoses)
            {
                Debug.LogFormat("RaycastHit Position: {0}", raycastPose.position.ToString());
                

                if(reference.transform.position == raycastPose.position)
                {
                    Instantiate(m_ObjectToPlace, reference.transform.position, reference.transform.rotation);
                    match = true;
                    break;
                }
            }

            if(!match)
            {
                GameObject badRefObject = Instantiate(m_ObjectToPlace, reference.transform.position, reference.transform.rotation);
                badRefObject.GetComponent<Renderer>().material = m_BadRefPointMaterial;
            }
        }
    }
}
