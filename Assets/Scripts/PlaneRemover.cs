using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class PlaneRemover : MonoBehaviour
{

    public ARPlaneManager planeManager;
    public Text status;

    public void PlaneRemoveButton() 
    {
        if (planeManager != null)
        {
            planeManager.enabled = !planeManager.enabled;
            status.text = "Plane enabled: " + planeManager.enabled.ToString();
            Debug.Log("PlaneRemove button hit");
        }
    }
}
