using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARPlaneManager))]
public class PlaneVideoTest : MonoBehaviour
{
    ARPlaneManager m_PlaneManager;
    VideoPlayer m_VideoPlayer;
    GameObject m_ARRig;

    // Use this for initialization
    void Start ()
    {
        m_PlaneManager = GetComponent<ARPlaneManager>();
        m_ARRig = GameObject.Find("AR Rig");
        m_VideoPlayer = m_ARRig.GetComponent<VideoPlayer>();
    }

    public void AttachVideoPlayer()
    {
        if (!m_VideoPlayer.enabled)
        {
            var planes = m_PlaneManager.trackables;
            Debug.Log(planes.count);

            ARPlane foundPlane = null;
            foreach (var plane in planes)
            {
                foundPlane = plane;
                break;
            }

            if (foundPlane != null)
            {
                m_VideoPlayer.targetMaterialRenderer = foundPlane.GetComponent<MeshRenderer>();
                m_VideoPlayer.targetMaterialProperty = "_MainTex";
                if (m_VideoPlayer.targetMaterialRenderer != null)
                {
                    m_VideoPlayer.enabled = true;
                }
            }
        }
    }
}
