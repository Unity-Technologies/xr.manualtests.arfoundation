using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARExtensions;
using UnityEngine.XR.ARFoundation;

public class CameraImageExample : MonoBehaviour
{
    Texture2D m_Texture;

    void OnEnable()
    {
        ARSubsystemManager.cameraFrameReceived += OnCameraFrameReceived;
    }

    void OnDisable()
    {
        ARSubsystemManager.cameraFrameReceived -= OnCameraFrameReceived;
    }

    unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        // Get the image in the ARSubsystemManager.cameraFrameReceived callback
        var cameraSubsystem = ARSubsystemManager.cameraSubsystem;

        CameraImage image;
        if (!cameraSubsystem.TryGetLatestImage(out image))
            return;

        var conversionParams = new CameraImageConversionParams
        {
            // Get the entire image
            inputRect = new RectInt(0, 0, image.width, image.height),

            // Downsample by 2
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

            // Choose RGBA format
            outputFormat = TextureFormat.RGBA32,

            // Flip across the vertical axis (mirror image)
            transformation = CameraImageTransformation.MirrorY
        };

        // See how many bytes we need to store the final image.
        int size = image.GetConvertedDataSize(conversionParams);

        // Allocate a buffer to store the image
        var buffer = new NativeArray<byte>(size, Allocator.Temp);

        // Extract the image data
        image.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);

        // The image was converted to RGBA32 format and written into the provided buffer
        // so we can dispose of the CameraImage. We must do this or it will leak resources.
        image.Dispose();

        // At this point, we could process the image, pass it to a computer vision algorithm, etc.
        // In this example, we'll just apply it to a texture to visualize it.

        // We've got the data; let's put it into a texture so we can visualize it.
        m_Texture = new Texture2D(conversionParams.outputDimensions.x, conversionParams.outputDimensions.y, conversionParams.outputFormat, false);
        m_Texture.LoadRawTextureData(buffer);
        m_Texture.Apply();

        // Done with our temporary data
        buffer.Dispose();
    }
}
