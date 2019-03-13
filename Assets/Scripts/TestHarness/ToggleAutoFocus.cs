using UnityEngine;
using UnityEngine.XR.ARExtensions;
using UnityEngine.XR.ARFoundation;

public class ToggleAutoFocus : MonoBehaviour
{

    public ARCameraOptions aRCameraOptions;

    void Start()
    {
        aRCameraOptions = GetComponent<ARCameraOptions>();
    }
    
    public void ToggleFocusButton(int option) 
    {

        if (aRCameraOptions != null)
        {
            aRCameraOptions.focusMode = (CameraFocusMode)option;
            Debug.Log("AutoFocus Button Hit");
        }
    }
}
