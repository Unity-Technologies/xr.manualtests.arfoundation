using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARExtensions;
using UnityEngine.XR.ARFoundation;

public class TestSimpleCameraImage : MonoBehaviour
{
    GUIStyle m_LabelStyle;

    [SerializeField]
    TextureFormat m_Format = TextureFormat.RGB24;

    [SerializeField]
    RawImage m_RawImage;

    Texture2D m_Texture;

    unsafe void OnCameraFrameUpdate(ARCameraFrameEventArgs eventArgs)
    {
        CameraImage cameraImage;
        if (!ARSubsystemManager.cameraSubsystem.TryGetLatestImage(out cameraImage))
            return;

        var conversionParams = new CameraImageConversionParams(cameraImage, m_Format, CameraImageTransformation.MirrorY);
        if (m_Texture == null || m_Texture.width != cameraImage.width || m_Texture.height != cameraImage.height)
            m_Texture = new Texture2D(cameraImage.width, cameraImage.height, m_Format, false);

        var textureBuffer = m_Texture.GetRawTextureData<byte>();
        try
        {
            cameraImage.Convert(conversionParams, new IntPtr(textureBuffer.GetUnsafePtr()), textureBuffer.Length);
        }
        finally
        {
            cameraImage.Dispose();
        }

        m_Texture.Apply();
        m_RawImage.texture = m_Texture;
    }

    void OnGUI()
    {
        if (m_LabelStyle == null)
        {
            m_LabelStyle = new GUIStyle(GUI.skin.label);
            m_LabelStyle.fontSize = 30;
        }

        if (m_Texture != null)
        {
            var msg = string.Format("CameraImage dimensions {0} x {1}", m_Texture.width, m_Texture.height);
            GUI.Label(new Rect(0, Screen.height / 2, Screen.width, Screen.height / 2), msg, m_LabelStyle);
        }
    }

    void OnEnable()
    {
        m_RawImage.enabled = true;
        ARSubsystemManager.cameraFrameReceived += OnCameraFrameUpdate;
    }

    void OnDisable()
    {
        m_RawImage.enabled = false;
        ARSubsystemManager.cameraFrameReceived -= OnCameraFrameUpdate;
    }
}
