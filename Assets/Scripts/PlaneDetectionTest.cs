using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;
public class PlaneDetectionTest : MonoBehaviour
{
    [SerializeField]
    private GameObject m_ObjectToPlace;

    private ARSessionOrigin m_Origin;
    private List<ARRaycastHit> m_RaycastHits = new List<ARRaycastHit>();

    // Start is called before the first frame update
    void Start()
    {
        m_Origin = GetComponent<ARSessionOrigin>();
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
                placedOjbect.transform.Translate(0, 0.2f, 0, Space.Self);
            }
        }
    }
}
