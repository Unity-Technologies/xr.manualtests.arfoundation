using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Video;

public class PlaneVideoTest : MonoBehaviour
{

    ARPlaneManager planeManager;
    VideoPlayer vPlayer;
    GameObject arRig;
    
    List<ARPlane> foundPlanes = new List<ARPlane>();

	// Use this for initialization
	void Start () {
        planeManager = GetComponent<ARPlaneManager>();
        arRig = GameObject.Find("AR Rig");
        vPlayer = arRig.GetComponent<VideoPlayer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void AttachVideoPlayer()
    {
        if (!vPlayer.enabled)
        {
            planeManager.GetAllPlanes(foundPlanes);
            Debug.Log(planeManager.planeCount);

            vPlayer.targetMaterialRenderer = foundPlanes[0].GetComponent<MeshRenderer>();
            vPlayer.targetMaterialProperty = "_MainTex";
            if (vPlayer.targetMaterialRenderer != null)
            {
                vPlayer.enabled = true;
            }
        }
    }
}
