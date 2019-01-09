using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class PlaneRemover : MonoBehaviour
{

    public ARPlaneManager planeManager;
    public Text status;

    public void PlaneRemoveButton() 
    {
        if (planeManager != null)
        {
            planeManager.enabled = !planeManager.enabled;
            status.text = "Plane enabaled: " + planeManager.enabled.ToString();
            Debug.Log("PlaneRemove button hit");
        }
    }


}
