using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TestFocusMode : MonoBehaviour
{
    [SerializeField]
    ARCameraManager m_ARCameraManager;

    public void DoGUI()
    {
        ButtonManager.AddButton("Focus Mode: " + m_ARCameraManager.focusMode, () =>
        {
            if (m_ARCameraManager.focusMode == CameraFocusMode.Fixed)
                m_ARCameraManager.focusMode = CameraFocusMode.Auto;
            else
                m_ARCameraManager.focusMode = CameraFocusMode.Fixed;
        });
    }
}
