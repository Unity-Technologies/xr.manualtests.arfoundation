using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ToggleAutoFocus : MonoBehaviour
{
    [SerializeField]
    ARCameraManager m_CameraManager;

    public void ToggleFocusButton(int option) 
    {
        if (m_CameraManager != null)
        {
            m_CameraManager.focusMode = (CameraFocusMode)option;
            Debug.Log("AutoFocus Button Hit");
        }
    }
}
