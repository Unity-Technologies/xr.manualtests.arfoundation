using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARReferencePointManager))]
public class RefPointTest : MonoBehaviour
{
    enum ReferencePointType
    {
        Pose,
        Plane,
        None
    };

    [SerializeField]
    GameObject m_PoseObject;

    [SerializeField]
    GameObject m_PlaneObject;

    [SerializeField]
    Dropdown refPointDropDown;

    ReferencePointType currentRefPointType = ReferencePointType.None;

    ARReferencePointManager m_RefManager;
    ARSessionOrigin m_Origin;
    ARPlaneManager m_planeManager;
    List<ARReferencePoint> m_TrackableIds = new List<ARReferencePoint>();
    List<ARRaycastHit> m_RaycastHits = new List<ARRaycastHit>();
    Dictionary<ARReferencePoint, GameObject> m_pointDictionary = new Dictionary<ARReferencePoint, GameObject>();

    // Use this for initialization
    void Start()
    {
        m_RefManager = GetComponent<ARReferencePointManager>();
        m_Origin = GetComponent<ARSessionOrigin>();

        refPointDropDown.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(refPointDropDown);
        });

        m_planeManager = gameObject.GetComponent<ARPlaneManager>();

        refPointDropDown.RefreshShownValue();
        currentRefPointType = (ReferencePointType)refPointDropDown.value;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (m_RefManager == null)
            return;

        if (m_Origin.camera == null)
            return;
        
        if (m_PoseObject == null || m_PlaneObject == null)
            return;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (m_Origin.Raycast(Input.mousePosition, m_RaycastHits, TrackableType.PlaneWithinBounds))
            {

                Debug.LogFormat("Hit Position: {0}", m_RaycastHits[0].pose);

                switch (currentRefPointType)
                {
                    case ReferencePointType.Pose:
                        {
                            Debug.Log("Adding Pose Reference Point");

                            ARReferencePoint tempPoint = m_RefManager.TryAddReferencePoint(m_RaycastHits[0].pose);
                            m_pointDictionary.Add(tempPoint, Instantiate(m_PoseObject, m_RaycastHits[0].pose.position, m_RaycastHits[0].pose.rotation));
                            m_TrackableIds.Add(tempPoint);

                            return;
                        }
                    case ReferencePointType.Plane:
                        {
                            Debug.Log("Adding Plane Reference Point");

                            TrackableId tempId = m_RaycastHits[0].trackableId;
                            ARPlane tempPlane = m_planeManager.TryGetPlane(tempId);

                            ARReferencePoint tempPoint = m_RefManager.TryAttachReferencePoint(tempPlane, m_RaycastHits[0].pose);
                            m_pointDictionary.Add(tempPoint, Instantiate(m_PlaneObject, m_RaycastHits[0].pose.position, m_RaycastHits[0].pose.rotation));

                            return;
                        }
                    case ReferencePointType.None:
                        {
                            Debug.Log("No ReferencePointType selected");
                            return;
                        }
                    default:
                        {
                            Debug.Log("ReferencePointType is unknown");
                            break;
                        }
                }
            }
        }
    }

    void DropdownValueChanged(Dropdown change)
    {
        currentRefPointType = (ReferencePointType)change.value;
        refPointDropDown.RefreshShownValue();
    }

    public void DeleteAllReferencePoints()
    {
        Debug.Log("Removing all reference points");

        bool refPointRemoved;
        List<ARReferencePoint> tempTrackableList = new List<ARReferencePoint>(m_TrackableIds);

        foreach(ARReferencePoint point in tempTrackableList)
        {
            refPointRemoved = m_RefManager.TryRemoveReferencePoint(point);

            if (!refPointRemoved)
            {
                Debug.Log("Reference Point not removed");
            }
            else
            {
                GameObject pointObject;
                if(m_pointDictionary.TryGetValue(point, out pointObject))
                {
                    m_TrackableIds.Remove(point);
                    Destroy(pointObject);
                }
                else
                {
                    Debug.Log("Could not find GameObject value using ARReferencePoint key");
                }
            }
        }
    }
}
