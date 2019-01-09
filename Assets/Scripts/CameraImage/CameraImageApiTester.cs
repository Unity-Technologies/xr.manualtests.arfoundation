using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARExtensions;
using UnityEngine.XR.ARFoundation;

public class CameraImageApiTester : MonoBehaviour
{
    Texture2D m_Texture;

    static readonly TextureFormat[] m_Formats = new TextureFormat[]
    {
        TextureFormat.Alpha8,
        TextureFormat.RGB24,
        TextureFormat.RGBA32,
        TextureFormat.ARGB32,
        TextureFormat.BGRA32,
        TextureFormat.R8
    };

    static readonly CameraImageTransformation[] m_ImageTransformations = new CameraImageTransformation[]
    {
        CameraImageTransformation.None,
        CameraImageTransformation.MirrorX,
        CameraImageTransformation.MirrorY,
        CameraImageTransformation.MirrorX | CameraImageTransformation.MirrorY
    };

    static readonly int[] m_DownsampleFactors = new int[]
    {
        1, 2, 4, 13
    };

    void OnEnable()
    {
        ARSubsystemManager.cameraFrameReceived += OnCameraFrameReceived;
    }

    void OnDisable()
    {
        ARSubsystemManager.cameraFrameReceived -= OnCameraFrameReceived;
    }

    int m_DownsampleFactorIndex;
    int m_ImageTransformationIndex;
    int m_FormatIndex;

    unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);

        if (ImageTest())
        {
            if (++m_DownsampleFactorIndex == m_DownsampleFactors.Length)
            {
                m_DownsampleFactorIndex = 0;
                if (++m_ImageTransformationIndex == m_ImageTransformations.Length)
                {
                    m_ImageTransformationIndex = 0;
                    if (++m_FormatIndex == m_Formats.Length)
                    {
                        PlaneTest();
                        Debug.Log("=== Test Complete ===");
                        ARSubsystemManager.cameraFrameReceived -= OnCameraFrameReceived;
                    }
                }
            }
        }
    }

    unsafe bool ImageTest()
    {
        Debug.Log("\n\n=== Begin Test ===");
        var downsampleFactor = m_DownsampleFactors[m_DownsampleFactorIndex];
        var imageTransformation = m_ImageTransformations[m_ImageTransformationIndex];
        var format = m_Formats[m_FormatIndex];

        Debug.LogFormat("Downsample factor: {0}\nCameraImageTransformation: {1}\nTextureFormat: {2}",
            downsampleFactor, imageTransformation, format);

        var cameraSubsystem = ARSubsystemManager.cameraSubsystem;
        CameraImage image;
        if (cameraSubsystem.TryGetLatestImage(out image))
        {
            Debug.LogFormat("[X] TryGetImageInfo:\n\twidth: {0}\n\theight: {1}\n\tplaneCount: {2}",
                image.width, image.height, image.planeCount);
        }
        else
        {
            Debug.Log("[ ] TryGetImageInfo");
            return false;
        }

        var conversionParams = new CameraImageConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width / downsampleFactor, image.height / downsampleFactor),
            outputFormat = format,
            transformation = imageTransformation
        };

        int size = image.GetConvertedDataSize(conversionParams.outputDimensions, format);
        Debug.LogFormat("[X] GetConvertedDataSize ({0} {1}x{2} = {3})", format, conversionParams.outputDimensions.x, conversionParams.outputDimensions.y, size);

        var buffer = new NativeArray<byte>(size, Allocator.Temp);
        image.Convert(conversionParams, new IntPtr(buffer.GetUnsafePtr()), buffer.Length);
        buffer.Dispose();
        Debug.Log("[X] Convert");
        image.Dispose();

        return true;
    }

    unsafe void PlaneTest()
    {
        var cameraSubsystem = ARSubsystemManager.cameraSubsystem;
        string info = "\n\n=== Image Plane Test ===";

        CameraImage image;
        if (!ARSubsystemManager.cameraSubsystem.TryGetLatestImage(out image))
        {
            Debug.LogError("TryGetLatestImage failed.");
            return;
        }

        info += string.Format("\n[X] Image plane count = {0}", image.planeCount);
        for (int planeIndex = 0; planeIndex < image.planeCount; ++planeIndex)
        {
            var plane = image.GetPlane(planeIndex);
            info += string.Format("\n[X] Plane {0}:\n\tsize: {1}\n\trowStride: {2}\n\tpixelStride: {3}",
                planeIndex, plane.data.Length, plane.rowStride, plane.pixelStride);
        }

        image.Dispose();
        info += "\n=== COMPLETE ===";

        Debug.Log(info);
    }
}
