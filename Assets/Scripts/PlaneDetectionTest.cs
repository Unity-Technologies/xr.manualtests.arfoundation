using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]

public class PlaneDetectionTest : MonoBehaviour
{

    enum PlaneType
    {
        None,
        Horizontal,
        Vertical
    };

    [SerializeField]
    GameObject m_ObjectToPlace;

    [SerializeField]
    public Dropdown planeDropDown;

    ARSessionOrigin m_Origin;
    ARPlaneManager m_PlaneManager;
    PlaneType currentPlaneType = PlaneType.None;
    List<ARRaycastHit> m_RaycastHits = new List<ARRaycastHit>();
    List<GameObject> UnityLogoObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        m_Origin = GetComponent<ARSessionOrigin>();
        m_PlaneManager = GetComponent<ARPlaneManager>();
        m_PlaneManager.detectionFlags = UnityEngine.XR.ARExtensions.PlaneDetectionFlags.None;
        Debug.Log(m_PlaneManager.detectionFlags.ToString());

        planeDropDown.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(planeDropDown);
        });
        
        planeDropDown.RefreshShownValue();
        currentPlaneType = (PlaneType)planeDropDown.value;
    }

    // Update is called once per frame
    void Update()
    {
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

                m_ObjectToPlace.transform.position = m_RaycastHits[0].pose.position;

                var pos = new Vector3(m_ObjectToPlace.transform.position.x, m_ObjectToPlace.transform.position.y + 0.05f,
                    m_ObjectToPlace.transform.position.z);

                m_ObjectToPlace.transform.position = pos;

                GameObject placedOjbect = Instantiate(m_ObjectToPlace, m_RaycastHits[0].pose.position, m_RaycastHits[0].pose.rotation);
                placedOjbect.transform.Rotate(-90, 0, 90, Space.Self);

                UnityLogoObjects.Add(placedOjbect);
            }
        }
    }

    void DestroyGameObject()
    {
        foreach (GameObject logo in UnityLogoObjects)
        {
            Destroy(logo);
        }
    }

    void DropdownValueChanged(Dropdown change)
    {
        Debug.Log("Dropdown Value");
        currentPlaneType = (PlaneType)change.value;
        planeDropDown.RefreshShownValue();

        switch (currentPlaneType)
        {
            case PlaneType.Horizontal:
                {
                    Debug.Log("Changing plane flags to horizontal");
                    
                    m_PlaneManager.detectionFlags = UnityEngine.XR.ARExtensions.PlaneDetectionFlags.Horizontal;
                    Debug.Log(m_PlaneManager.detectionFlags.ToString());


                    break;
                }

            case PlaneType.Vertical:
                {
                    Debug.Log("Changing plane flags to vertical");

                    m_PlaneManager.detectionFlags = UnityEngine.XR.ARExtensions.PlaneDetectionFlags.Vertical;
                    Debug.Log(m_PlaneManager.detectionFlags.ToString());

                    break;
                }

            case PlaneType.None:
                {
                    Debug.Log("Changing plane flags to None");

                    m_PlaneManager.detectionFlags = UnityEngine.XR.ARExtensions.PlaneDetectionFlags.None;
                    Debug.Log(m_PlaneManager.detectionFlags.ToString());

                    break;
                }
        }
        DestroyGameObject();
        UnityLogoObjects.Clear();
    }
}


