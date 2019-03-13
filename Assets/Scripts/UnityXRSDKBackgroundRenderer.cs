using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Renders the device's camera as a background to the attached Unity camera component.
/// </summary>
public class UnityXRSDKBackgroundRenderer : MonoBehaviour
{
    public Camera m_Camera;

    public UnityEngine.XR.ARFoundation.ARCameraBackground m_BackgroundComponent;

    public CameraClearFlags clearFlag = CameraClearFlags.Skybox;

    void OnEnable()
    {
        if (Application.isEditor)
        {
            enabled = false;
            return;
        }

        m_BackgroundComponent = GetComponent<UnityEngine.XR.ARFoundation.ARCameraBackground>();

        if (m_BackgroundComponent == null)
        {
            Debug.LogError("ARBackgroundRender:: Could not retrieve ARBackgroundRenderer Component");
            return;
        }

        ARSubsystemManager.cameraFrameReceived += OnCameraFrameReceived;
    }

    void Update()
    {
        if (m_BackgroundComponent == null)
        {
            return;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {

            Debug.Log("Touch Detected");

            if (m_BackgroundComponent.enabled == true)
            {
                Debug.Log("Switching to Standard Background");
                m_BackgroundComponent.enabled = false;
                m_Camera.clearFlags = clearFlag;

                ARSubsystemManager.cameraSubsystem.Camera = UnityEngine.Camera.current;
            }
            else
            {
                Debug.Log("Switching to Material Background");
                m_BackgroundComponent.enabled = true;
            }
        }
    }
    void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
    }
}

