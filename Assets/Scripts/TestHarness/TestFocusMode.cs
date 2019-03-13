using UnityEngine;
using UnityEngine.XR.ARExtensions;
using UnityEngine.XR.ARFoundation;

public class TestFocusMode : MonoBehaviour
{
    [SerializeField]
    ARCameraOptions m_ARCameraOptions;

    public void DoGUI()
    {
        ButtonManager.AddButton("Focus Mode: " + m_ARCameraOptions.focusMode, () =>
        {
            if (m_ARCameraOptions.focusMode == CameraFocusMode.Fixed)
                m_ARCameraOptions.focusMode = CameraFocusMode.Auto;
            else
                m_ARCameraOptions.focusMode = CameraFocusMode.Fixed;
        });
    }
}
