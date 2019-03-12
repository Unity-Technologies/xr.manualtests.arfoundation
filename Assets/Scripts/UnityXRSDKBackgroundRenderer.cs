
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Renders the device's camera as a background to the attached Unity camera component.
/// </summary>
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(ARCameraBackground))]
public class UnityXRSDKBackgroundRenderer : MonoBehaviour
{
    Camera m_Camera;

    ARCameraBackground m_BackgroundComponent;

    public CameraClearFlags clearFlag = CameraClearFlags.Skybox;

    private void OnEnable()
    {
        if (Application.isEditor)
        {
            enabled = false;
            return;
        }

        m_BackgroundComponent = GetComponent<ARCameraBackground>();
        m_Camera = GetComponent<Camera>();
    }

    private void Update()
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

                //ARSubsystemManager.cameraSubsystem.Camera = UnityEngine.Camera.current;
            }
            else
            {
                Debug.Log("Switching to Material Background");
                m_BackgroundComponent.enabled = true;
            }
        }
    }
}

