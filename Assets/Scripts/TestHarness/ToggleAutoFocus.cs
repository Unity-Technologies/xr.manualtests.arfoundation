using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARExtensions;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;


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
