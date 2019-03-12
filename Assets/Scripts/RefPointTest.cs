﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARReferencePointManager))]
[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
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

    private ARReferencePointManager m_RefManager;
    private ARRaycastManager m_RaycastManager;
    private ARPlaneManager m_PlaneManager;
    private List<ARReferencePoint> m_TrackableIds = new List<ARReferencePoint>();
    private List<ARRaycastHit> m_RaycastHits = new List<ARRaycastHit>();
    private Dictionary<ARReferencePoint, GameObject> m_pointDictionary = new Dictionary<ARReferencePoint, GameObject>();

    // Use this for initialization
    void Start()
    {
        m_RefManager = GetComponent<ARReferencePointManager>();
        m_RaycastManager = GetComponent<ARRaycastManager>();

        refPointDropDown.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(refPointDropDown);
        });

        m_PlaneManager = gameObject.GetComponent<ARPlaneManager>();

        refPointDropDown.RefreshShownValue();
        currentRefPointType = (ReferencePointType)refPointDropDown.value;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (m_RefManager == null)
            return;

        if (m_PoseObject == null || m_PlaneObject == null)
            return;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (m_RaycastManager.Raycast(Input.mousePosition, m_RaycastHits, TrackableType.PlaneWithinBounds))
            {
                Debug.LogFormat("Hit Position: {0}", m_RaycastHits[0].pose);

                switch (currentRefPointType)
                {
                    case ReferencePointType.Pose:
                        {
                            Debug.Log("Adding Pose Reference Point");

                            ARReferencePoint tempPoint = m_RefManager.AddReferencePoint(m_RaycastHits[0].pose);
                            m_pointDictionary.Add(tempPoint, Instantiate(m_PoseObject, m_RaycastHits[0].pose.position, m_RaycastHits[0].pose.rotation));
                            m_TrackableIds.Add(tempPoint);

                            return;
                        }
                    case ReferencePointType.Plane:
                        {
                            Debug.Log("Adding Plane Reference Point");

                            TrackableId tempId = m_RaycastHits[0].trackableId;
                            ARPlane tempPlane = m_PlaneManager.GetPlane(tempId);

                            ARReferencePoint tempPoint = m_RefManager.AttachReferencePoint(tempPlane, m_RaycastHits[0].pose);
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
            refPointRemoved = m_RefManager.RemoveReferencePoint(point);

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
