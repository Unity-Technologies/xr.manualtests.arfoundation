using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARExtensions;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

[RequireComponent(typeof(SubrectSelector))]
public class CameraImageApiTestSuite : MonoBehaviour
{
    [SerializeField] RawImage m_FullRawImage;
    [SerializeField] RawImage m_SubrectRawImage;
    [SerializeField] Text m_InfoText;
    [SerializeField] int m_FramesBetweenTests = 100;

    Texture2D m_FullTexture;
    Texture2D m_SubrectTexture;

    enum Mode
    {
        Synchronous,
        AsyncRequest,
        AsyncCallback
    }

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

    static readonly Mode[] m_Modes = new Mode[]
    {
        Mode.Synchronous,
        Mode.AsyncRequest,
        Mode.AsyncCallback,
    };

    static readonly int[] m_DownsampleFactors = new int[]
    {
        1, 2, 4, 8
    };

    int m_DownsampleFactorIndex;

    int m_ImageTransformationIndex;

    int m_FormatIndex;

    int m_ModeIndex;

    int m_FramesUntilNextTest;

    SubrectSelector m_SubrectSelector;

    void Awake()
    {
        m_SubrectSelector = GetComponent<SubrectSelector>();
    }

    void OnEnable()
    {
        ARSubsystemManager.cameraFrameReceived += OnCameraFrameReceived;
    }

    void OnDisable()
    {
        ARSubsystemManager.cameraFrameReceived -= OnCameraFrameReceived;
    }

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeRight;
        m_FullTexture = new Texture2D(1, 1, TextureFormat.Alpha8, false);
        m_SubrectTexture = new Texture2D(1, 1, TextureFormat.Alpha8, false);
        m_FullRawImage.texture = m_FullTexture;
        m_SubrectRawImage.texture = m_SubrectTexture;
        m_FramesUntilNextTest = m_FramesBetweenTests;
    }

    void ResizeTextureIfNecessary(ref Texture2D texture, Vector2Int dimensions, TextureFormat format)
    {
        if (texture.width != dimensions.x || texture.height != dimensions.y || texture.format != format)
            texture = new Texture2D(dimensions.x, dimensions.y, format, false);
        m_FullRawImage.texture = m_FullTexture;
        m_SubrectRawImage.texture = m_SubrectTexture;
    }

    void AdvanceToNextTest()
    {
        if (--m_FramesUntilNextTest <= 0)
        {
            m_FramesUntilNextTest = m_FramesBetweenTests;
            if (++m_DownsampleFactorIndex == m_DownsampleFactors.Length)
            {
                m_DownsampleFactorIndex = 0;
                if (++m_ImageTransformationIndex == m_ImageTransformations.Length)
                {
                    m_ImageTransformationIndex = 0;
                    if (++m_FormatIndex == m_Formats.Length)
                    {
                        m_FormatIndex = 0;
                        if (++m_ModeIndex == m_Modes.Length)
                        {
                            PlaneTest();
                            Debug.Log("=== Test Complete ===");
                            ARSubsystemManager.cameraFrameReceived -= OnCameraFrameReceived;
                        }
                    }
                }
            }
        }
    }

    unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);

        if (m_Modes[m_ModeIndex] == Mode.Synchronous)
        {
            if (RunTest(m_DownsampleFactors[m_DownsampleFactorIndex], m_Formats[m_FormatIndex], m_ImageTransformations[m_ImageTransformationIndex]))
                AdvanceToNextTest();
        }

        DoSubrect();
    }

    unsafe void DoSubrect()
    {
        var downsampleFactor = 1;// (m_DownsampleFactorIndex < m_DownsampleFactors.Length) ? m_DownsampleFactors[m_DownsampleFactorIndex] : 1;
        var format = (m_FormatIndex < m_Formats.Length) ? m_Formats[m_FormatIndex] : TextureFormat.RGBA32;
        var imageTransformation = (m_ImageTransformationIndex < m_ImageTransformations.Length) ? m_ImageTransformations[m_ImageTransformationIndex] : CameraImageTransformation.MirrorY;

        var cameraSubsystem = ARSubsystemManager.cameraSubsystem;
        CameraImage image;
        if (!cameraSubsystem.TryGetLatestImage(out image))
            return;

        var rect = m_SubrectSelector.GetCameraImageRect(image);
        var conversionParams = new CameraImageConversionParams
        {
            inputRect = rect,
            outputDimensions = new Vector2Int(rect.width / downsampleFactor, rect.height / downsampleFactor),
            outputFormat = format,
            transformation = imageTransformation
        };

        var size = image.GetConvertedDataSize(conversionParams.outputDimensions, conversionParams.outputFormat);
        var data = new NativeArray<byte>(size, Allocator.Temp);

        image.Convert(conversionParams, new IntPtr(data.GetUnsafePtr()), data.Length);
        image.Dispose();

        ResizeTextureIfNecessary(ref m_SubrectTexture, conversionParams.outputDimensions, format);
        m_SubrectTexture.LoadRawTextureData(data);
        m_SubrectTexture.Apply();
        var screenRect = m_SubrectSelector.screenRect;
        m_SubrectRawImage.rectTransform.anchoredPosition = new Vector2(screenRect.x, screenRect.y);
        m_SubrectRawImage.rectTransform.sizeDelta = new Vector2(screenRect.width, screenRect.height);
        data.Dispose();
    }

    AsyncCameraImageConversion m_AsyncRequest;
    bool m_LastRequestComplete = true;

    void Update()
    {
        Screen.orientation = ScreenOrientation.LandscapeRight;
        m_FullRawImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

        if (m_ModeIndex >= m_Modes.Length)
            return;

        switch (m_Modes[m_ModeIndex])
        {
            case Mode.AsyncRequest:
                if (m_AsyncRequest.status.IsDone())
                {
                    if (ProcessAsyncRequest(m_AsyncRequest))
                        AdvanceToNextTest();

                    m_AsyncRequest = StartAsyncTest(m_DownsampleFactors[m_DownsampleFactorIndex], m_Formats[m_FormatIndex], m_ImageTransformations[m_ImageTransformationIndex]);
                }
                break;
            case Mode.AsyncCallback:
                if (m_LastRequestComplete)
                {
                    m_LastRequestComplete = false;
                    StartCallbackTest(m_DownsampleFactors[m_DownsampleFactorIndex], m_Formats[m_FormatIndex], m_ImageTransformations[m_ImageTransformationIndex]);
                }
                break;
        }
            
    }

    bool ProcessAsyncRequest(AsyncCameraImageConversion request)
    {
        var status = request.status;

        if (status == AsyncCameraImageConversionStatus.Disposed)
        {
            // Probably the first one
            return true;
        }
        else if (status.IsError())
        {
            m_InfoText.text += string.Format("\n[ ] AsyncRequest {0}", status);
            request.Dispose();
            return false;
        }

        var data = request.GetData<byte>();
        ResizeTextureIfNecessary(
            ref m_FullTexture,
            request.conversionParams.outputDimensions,
            request.conversionParams.outputFormat);

        m_FullTexture.LoadRawTextureData(data);
        m_FullTexture.Apply();
        request.Dispose();
        return true;
    }

    string GetInfo(CameraImage image)
    {
        return string.Format(
            "\n[X] TryGetLatestImage:\n\twidth: {0}\n\theight: {1}\n\tplaneCount: {2}\n\ttimestamp: {3}\n\tformat: {4}",
            image.width, image.height, image.planeCount, image.timestamp, image.format);
    }

    void StartCallbackTest(int downsampleFactor, TextureFormat format, CameraImageTransformation imageTransformation)
    {
        string info = string.Format("=== AsyncCallback Test ===\nDownsample factor: {0}\nTexture format: {1}\nImage Transformation: {2}",
            downsampleFactor, format, GetImageTransformationString(imageTransformation));

        var cameraSubsystem = ARSubsystemManager.cameraSubsystem;
        CameraImage image;
        if (cameraSubsystem.TryGetLatestImage(out image))
        {
            info += GetInfo(image);
        }
        else
        {
            info += "\n[ ] TryGetImageInfo";
            m_InfoText.text = "FAIL\n" + info;
            return;
        }

        m_InfoText.text = info;

        var conversionParams = new CameraImageConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width / downsampleFactor, image.height / downsampleFactor),
            outputFormat = format,
            transformation = imageTransformation
        };

        image.ConvertAsync(conversionParams, ProcessAsyncCallback);
        image.Dispose();
    }

    void ProcessAsyncCallback(AsyncCameraImageConversionStatus status, CameraImageConversionParams conversionParams, NativeArray<byte> data)
    {
        m_LastRequestComplete = true;
        if (status == AsyncCameraImageConversionStatus.Ready)
        {
            ResizeTextureIfNecessary(ref m_FullTexture, conversionParams.outputDimensions, conversionParams.outputFormat);
            m_FullTexture.LoadRawTextureData(data);
            m_FullTexture.Apply();
            m_InfoText.text += string.Format("\n[X] ProcessAsyncCallback: {0} bytes", data.Length);
            AdvanceToNextTest();
        }
        else
        {
            m_InfoText.text += string.Format("\n[ ] ProcessAsyncCallback failed with status {0}", status);
        }
    }

    AsyncCameraImageConversion StartAsyncTest(int downsampleFactor, TextureFormat format, CameraImageTransformation imageTransformation)
    {
        string info = string.Format("=== AsyncRequest Test ===\nDownsample factor: {0}\nTexture format: {1}\nImage Transformation: {2}",
            downsampleFactor, format, GetImageTransformationString(imageTransformation));

        var cameraSubsystem = ARSubsystemManager.cameraSubsystem;
        CameraImage image;
        if (cameraSubsystem.TryGetLatestImage(out image))
        {
            info += GetInfo(image);
        }
        else
        {
            info += "\n[ ] TryGetImageInfo";
            m_InfoText.text = "FAIL\n" + info;
            return default(AsyncCameraImageConversion);
        }

        m_InfoText.text = info;

        var conversionParams = new CameraImageConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width / downsampleFactor, image.height / downsampleFactor),
            outputFormat = format,
            transformation = imageTransformation
        };

        var request = image.ConvertAsync(conversionParams);
        image.Dispose();
        return request;
    }

    string GetImageTransformationString(CameraImageTransformation imageTransformation)
    {
        if (imageTransformation == CameraImageTransformation.None)
            return "None";

        string text = "";
        if ((imageTransformation & CameraImageTransformation.MirrorX) != CameraImageTransformation.None)
            text += "MirrorX";
        if ((imageTransformation & CameraImageTransformation.MirrorY) != CameraImageTransformation.None)
            text += ((text.Length > 0) ? " | " : "") + "MirrorY";
        return text;
    }

    unsafe bool RunTest(int downsampleFactor, TextureFormat format, CameraImageTransformation imageTransformation)
    {
        string info = string.Format("=== Synchronous Test ===\nDownsample factor: {0}\nTexture format: {1}\nImage Transformation: {2}",
            downsampleFactor, format, GetImageTransformationString(imageTransformation));

        var cameraSubsystem = ARSubsystemManager.cameraSubsystem;


        CameraImage image;
        if (cameraSubsystem.TryGetLatestImage(out image))
        {
            info += GetInfo(image);
        }
        else
        {
            info += "\n[ ] TryGetLatestImage";
            m_InfoText.text = "FAIL\n" + info;
            return false;
        }

        var dimensions = new Vector2Int(image.dimensions.x / downsampleFactor, image.dimensions.y / downsampleFactor);
        int size = image.GetConvertedDataSize(dimensions, format);
        info += string.Format("\n[X] GetConvertedDataSize ({0} {1}x{2} = {3})", format, dimensions.x, dimensions.y, size);

        var rect = new RectInt(0, 0, image.width, image.height);
        ResizeTextureIfNecessary(ref m_FullTexture, dimensions, format);

#if UNITY_2018_2_OR_NEWER
        var data = m_FullTexture.GetRawTextureData<byte>();
#else
        var data = new NativeArray<byte>(size, Allocator.Temp);
#endif

        var conversionParams = new CameraImageConversionParams
        {
            inputRect = rect,
            outputDimensions = dimensions,
            outputFormat = format,
            transformation = imageTransformation
        };

        image.Convert(conversionParams, new IntPtr(data.GetUnsafePtr()), data.Length);
#if !UNITY_2018_2_OR_NEWER
        m_FullTexture.LoadRawTextureData(data.ToArray());
        data.Dispose();
#endif

        image.Dispose();
        m_FullTexture.Apply();
        m_InfoText.text = "PASS\n" + info;
        return true;
    }

    unsafe void PlaneTest()
    {
        string info = "=== Image Plane Test ===\n";

        CameraImage image;
        if (!ARSubsystemManager.cameraSubsystem.TryGetLatestImage(out image))
        {
            m_InfoText.text = "TryGetLatestImage failed.";
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
        m_InfoText.text = info;
        m_FullRawImage.enabled = false;
    }
}
