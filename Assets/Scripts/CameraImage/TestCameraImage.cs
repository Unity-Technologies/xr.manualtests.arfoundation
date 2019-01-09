using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
using UnityEngine.XR.ARExtensions;
using UnityEngine.XR.ARFoundation;

public class TestCameraImage : MonoBehaviour
{
    public class AsyncImage
    {
        public AsyncImage(TestCameraImage parentBehaviour, Rect rect, RawImage rawImage)
        {
            m_Rect = rect;
            m_RawImage = rawImage;
            m_ParentBehaviour = parentBehaviour;
        }

        public void Update(CameraImage image)
        {
            if (!m_AsyncConversion.status.IsDone())
                return;

            switch (m_AsyncConversion.status)
            {
                case AsyncCameraImageConversionStatus.Ready:
                    Process();
                    m_AsyncConversion.Dispose();
                    CreateRequest(image);
                    break;
                case AsyncCameraImageConversionStatus.Disposed:
                    CreateRequest(image);
                    break;
                default:
                    // Some error
                    Debug.LogErrorFormat("Async conversion failed with status {0}", m_AsyncConversion.status);
                    m_AsyncConversion.Dispose();
                    break;
            }
        }

        void Process()
        {
            m_Texture = CreateOrResizeTexture(
                m_Texture,
                m_AsyncConversion.conversionParams.outputDimensions,
                m_AsyncConversion.conversionParams.outputFormat);

            var data = m_AsyncConversion.GetData<byte>();
            m_Texture.LoadRawTextureData(data);
            m_Texture.Apply();
            m_RawImage.texture = m_Texture;
        }

        void CreateRequest(CameraImage image)
        {
            if (!image.valid)
                return;

            var inputRect = new RectInt(
                (int)(m_Rect.x * image.width),
                (int)(m_Rect.y * image.height),
                (int)(m_Rect.width * image.width),
                (int)(m_Rect.height * image.height));

            m_AsyncConversion = image.ConvertAsync(new CameraImageConversionParams
            {
                inputRect = inputRect,
                outputDimensions = new Vector2Int(inputRect.width, inputRect.height),
                outputFormat = m_ParentBehaviour.textureFormat,
                transformation = m_ParentBehaviour.imageTransformation
            });
        }

        Rect m_Rect;
        RawImage m_RawImage;
        Texture2D m_Texture;
        TestCameraImage m_ParentBehaviour;
        AsyncCameraImageConversion m_AsyncConversion;
    }

    [SerializeField] RawImage m_LowerLeftImage;
    [SerializeField] RawImage m_LowerRightImage;
    [SerializeField] RawImage m_UpperLeftImage;
    [SerializeField] RawImage m_UpperRightImage;

    [SerializeField] RawImage m_LowerLeftImageAsync;
    [SerializeField] RawImage m_LowerRightImageAsync;
    [SerializeField] RawImage m_UpperLeftImageAsync;
    [SerializeField] RawImage m_UpperRightImageAsync;

    [SerializeField] RawImage m_FullImageImage;
    [SerializeField] RawImage m_MiddleHalfImage;
    [SerializeField] RawImage m_AsyncImage;

    [SerializeField] TextureFormat m_TextureFormat;
    public TextureFormat textureFormat
    {
        get { return m_TextureFormat; }
        set { m_TextureFormat = value; }
    }
    [SerializeField] Text m_InfoText;

    [SerializeField] CameraImageTransformation m_ImageTransformation;
    public CameraImageTransformation imageTransformation
    {
        get { return m_ImageTransformation; }
        set { m_ImageTransformation = value; }
    }

    Texture2D m_LowerLeftTexture;
    Texture2D m_UpperLeftTexture;
    Texture2D m_LowerRightTexture;
    Texture2D m_UpperRightTexture;
    Texture2D m_FullTexture;
    Texture2D m_MiddleHalfTexture;
    Texture2D m_AsyncTexture;

    static readonly TextureFormat[] m_SupportedFormats = new TextureFormat[]
    {
        TextureFormat.Alpha8,
        TextureFormat.RGB24,
        TextureFormat.RGBA32,
        TextureFormat.ARGB32,
        TextureFormat.BGRA32,
        TextureFormat.R8
    };

    int m_FormatIndex;

    [SerializeField] float m_TimePerFormat = 2f;

    float m_LastFormatChangeTime;

    static readonly int[] m_DownsampleFactors = new int[]
    {
        1,
        2,
        3,
        4,
        8
    };

    int m_DownsampleFactor = 1;
    int m_DownsampleFactorIndex = 0;
    const int k_FramesPerDownsample = 60;
    int m_FramesUntilNextDownsample = k_FramesPerDownsample;

    int m_Offset;

    LineRenderer m_LineRenderer;

    RectInt m_Rect;

    List<AsyncImage> m_AsyncImages;

    void Awake()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
        m_LineRenderer.widthMultiplier = .001f;
        m_Rect = new RectInt(
            m_Rect.x = Screen.width / 4,
            m_Rect.y = Screen.height / 4,
            m_Rect.width = Screen.width / 2,
            m_Rect.height = Screen.height / 2);

        m_AsyncImages = new List<AsyncImage>();
        m_AsyncImages.Add(new AsyncImage(this, new Rect(0, 0, .5f, .5f), m_LowerLeftImageAsync));
        m_AsyncImages.Add(new AsyncImage(this, new Rect(.5f, 0, .5f, .5f), m_LowerRightImageAsync));
        m_AsyncImages.Add(new AsyncImage(this, new Rect(0, .5f, .5f, .5f), m_UpperLeftImageAsync));
        m_AsyncImages.Add(new AsyncImage(this, new Rect(.5f, .5f, .5f, .5f), m_UpperRightImageAsync));
    }

    void OnEnable()
    {
        ARSubsystemManager.cameraFrameReceived += OnCameraFrameReceived;
        Application.onBeforeRender += DrawRect;
    }

    void OnDisable()
    {
        ARSubsystemManager.cameraFrameReceived -= OnCameraFrameReceived;
        Application.onBeforeRender -= DrawRect;
    }

    void DrawRect()
    {
        var camera = Camera.main;

        m_LineRenderer.positionCount = 4;
        m_LineRenderer.loop = true;

        var z = camera.nearClipPlane + .001f;
        m_LineRenderer.SetPosition(0, camera.ScreenToWorldPoint(new Vector3(m_Rect.x, m_Rect.y, z)));
        m_LineRenderer.SetPosition(1, camera.ScreenToWorldPoint(new Vector3(m_Rect.x + m_Rect.width, m_Rect.y, z)));
        m_LineRenderer.SetPosition(2, camera.ScreenToWorldPoint(new Vector3(m_Rect.x + m_Rect.width, m_Rect.y + m_Rect.height, z)));
        m_LineRenderer.SetPosition(3, camera.ScreenToWorldPoint(new Vector3(m_Rect.x, m_Rect.y + m_Rect.height, z)));
    }

    void Update()
    {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        if (Input.GetMouseButton(0))
        {
            var position = Input.mousePosition;
            m_Rect.x = Mathf.Clamp((int)position.x - m_Rect.width / 2, 0, Screen.width - m_Rect.width);
            m_Rect.y = Mathf.Clamp((int)position.y - m_Rect.height / 2, 0, Screen.height - m_Rect.height);
        }
    }

    unsafe void LoadCameraImageIntoTexture(CameraImage image, ref Texture2D texture, RectInt rect)
    {
        Profiler.BeginSample("LoadCameraImageIntoTexture");
        var conversionParams = new CameraImageConversionParams
        {
            inputRect = rect,
            outputDimensions = new Vector2Int(rect.width / m_DownsampleFactor, rect.height / m_DownsampleFactor),
            outputFormat = m_TextureFormat,
            transformation = m_ImageTransformation
        };

        texture = CreateOrResizeTexture(texture, conversionParams.outputDimensions, conversionParams.outputFormat);
        var data = texture.GetRawTextureData<byte>();
        image.Convert(
            conversionParams,
            new IntPtr(data.GetUnsafePtr()),
            data.Length);
        texture.Apply();

        Profiler.EndSample();
    }

    unsafe void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {
        CameraImage image;
        if (!ARSubsystemManager.cameraSubsystem.TryGetLatestImage(out image))
            return;

        m_DownsampleFactor = m_DownsampleFactors[m_DownsampleFactorIndex];

        var timeElapsed = Time.realtimeSinceStartup - m_LastFormatChangeTime;
        if (timeElapsed > m_TimePerFormat)
        {
            m_FormatIndex = (m_FormatIndex + 1) % m_SupportedFormats.Length;
            m_LastFormatChangeTime = Time.realtimeSinceStartup;
        }
        m_TextureFormat = m_SupportedFormats[m_FormatIndex];

        m_InfoText.text = m_TextureFormat.ToString();
        m_InfoText.text += "\nMirrorX: " + (((m_ImageTransformation & CameraImageTransformation.MirrorX) != CameraImageTransformation.None) ? "On" : "Off");
        m_InfoText.text += "\nMirrorY: " + (((m_ImageTransformation & CameraImageTransformation.MirrorY) != CameraImageTransformation.None) ? "On" : "Off");

        var width = image.width;
        var height = image.height;

        LoadCameraImageIntoTexture(image, ref m_UpperLeftTexture, new RectInt(0, height / 2, width / 2, height / 2));
        LoadCameraImageIntoTexture(image, ref m_UpperRightTexture, new RectInt(width / 2, height / 2, width / 2, height / 2));
        LoadCameraImageIntoTexture(image, ref m_LowerLeftTexture, new RectInt(0, 0, width / 2, height / 2));
        LoadCameraImageIntoTexture(image, ref m_LowerRightTexture, new RectInt(width / 2, 0, width / 2, height / 2));
        LoadCameraImageIntoTexture(image, ref m_FullTexture, new RectInt(0, 0, width, height));

        var middleRect = new RectInt(
            m_Rect.x * width / Screen.width,
            m_Rect.y * height / Screen.height,
            m_Rect.width * width / Screen.width,
            m_Rect.height * height / Screen.height);

        // The rectangle can become invalid on the frame where the screen rotates
        // so validate it before calling Convert
        if ((middleRect.x + middleRect.width <= image.width) &&
            (middleRect.y + middleRect.height <= image.height))
        {
            LoadCameraImageIntoTexture(image, ref m_MiddleHalfTexture, middleRect);
        }

        foreach (var asyncImage in m_AsyncImages)
            asyncImage.Update(image);

        m_Offset = (m_Offset + 1) % (height / 4);

        m_UpperLeftImage.texture = m_UpperLeftTexture;
        m_UpperRightImage.texture = m_UpperRightTexture;
        m_LowerLeftImage.texture = m_LowerLeftTexture;
        m_LowerRightImage.texture = m_LowerRightTexture;
        m_FullImageImage.texture = m_FullTexture;
        m_MiddleHalfImage.texture = m_MiddleHalfTexture;

        if (--m_FramesUntilNextDownsample <= 0)
        {
            m_DownsampleFactorIndex = (m_DownsampleFactorIndex + 1) % m_DownsampleFactors.Length;
            m_FramesUntilNextDownsample = k_FramesPerDownsample;
        }

        m_InfoText.text += "\nDownsample factor: " + m_DownsampleFactor;

        m_InfoText.text += string.Format("\nPlane count: {0}", image.planeCount);
        for (int planeIndex = 0; planeIndex < image.planeCount; ++planeIndex)
        {
            var plane = image.GetPlane(planeIndex);
            m_InfoText.text += string.Format("\nPlane {0}:\n\trowStride: {1}\n\tpixelStride: {2}\n\tdataLength: {3}",
                planeIndex, plane.rowStride, plane.pixelStride, plane.data.Length);
        }
        m_InfoText.text += string.Format("\nCameraImage format: {0}", image.format);
        m_InfoText.text += string.Format("\nCameraImage timestamp: {0}", image.timestamp.ToString("0.0000"));
        if (eventArgs.time.HasValue)
            m_InfoText.text += string.Format("\nCameraFrame timestamp: {0}", eventArgs.time.Value);

        image.Dispose();
    }

    static Texture2D CreateOrResizeTexture(Texture2D texture, CameraImageConversionParams conversionParams)
    {
        return CreateOrResizeTexture(texture, conversionParams.outputDimensions, conversionParams.outputFormat);
    }

    static Texture2D CreateOrResizeTexture(Texture2D texture, Vector2Int dimensions, TextureFormat format)
    {
        if (texture == null)
            return new Texture2D(dimensions.x, dimensions.y, format, false);

        if (texture.width != dimensions.x || texture.height != dimensions.y || texture.format != format)
            texture.Resize(dimensions.x, dimensions.y, format, false);

        return texture;
    }

    public void OnMirrorX()
    {
        m_ImageTransformation = m_ImageTransformation ^ CameraImageTransformation.MirrorX;
    }

    public void OnMirrorY()
    {
        m_ImageTransformation = m_ImageTransformation ^ CameraImageTransformation.MirrorY;
    }

    [SerializeField] RawImage m_AsyncImageFromButton;
    Texture2D m_Texture;

    public void GetImageAsyncCoroutine()
    {
        // Get information about the camera image
        CameraImage image;
        if (ARSubsystemManager.cameraSubsystem.TryGetLatestImage(out image))
        {
            // If successful, launch a coroutine that waits for the image
            // to be ready, then apply it to a texture.
            StartCoroutine(ProcessImage(image));
            image.Dispose();
        }
    }

    public void GetImageAsyncCallback()
    {
        // Get information about the camera image
        CameraImage image;
        if (ARSubsystemManager.cameraSubsystem.TryGetLatestImage(out image))
        {
            // If successful, launch a coroutine that waits for the image
            // to be ready, then apply it to a texture.
            image.ConvertAsync(new CameraImageConversionParams
            {
                // Get the full image
                inputRect = new RectInt(0, 0, image.width, image.height),

                // Downsample by 2
                outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

                outputFormat = m_TextureFormat,

                // Mirror across Y axis
                transformation = m_ImageTransformation
            }, ProcessImage);

            image.Dispose();
        }
    }

    void ProcessImage(AsyncCameraImageConversionStatus status, CameraImageConversionParams conversionParams, NativeArray<byte> data)
    {
        if (status != AsyncCameraImageConversionStatus.Ready)
        {
            Debug.LogErrorFormat("ProcessImageWithCallback: Async request failed with status {0}", status);
            return;
        }

        // Create a texture if necessary
        m_Texture = CreateOrResizeTexture(m_Texture, conversionParams.outputDimensions, conversionParams.outputFormat);
        m_Texture.LoadRawTextureData(data);
        m_Texture.Apply();
        m_AsyncImageFromButton.texture = m_Texture;
    }

    IEnumerator ProcessImage(CameraImage image)
    {
        // Create the request
        var request = image.ConvertAsync(new CameraImageConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2),
            outputFormat = TextureFormat.RGB24,
            transformation = CameraImageTransformation.MirrorY
        });

        // Wait for it to complete
        while (!request.status.IsDone())
            yield return null;

        // Check status to see if it completed successfully.
        if (request.status != AsyncCameraImageConversionStatus.Ready)
        {
            // Something when wrong
            Debug.LogErrorFormat("Request failed with status {0}", request.status);

            // Dispose even if there is an error.
            request.Dispose();
            yield break;
        }

        // Image data is ready. Let's apply it to a Texture2D.
        var rawData = request.GetData<byte>();

        // Create a texture if necessary
        if (m_Texture == null)
            m_Texture = new Texture2D(
                request.conversionParams.outputDimensions.x,
                request.conversionParams.outputDimensions.y,
                request.conversionParams.outputFormat, false);

        // Make sure it matches the dimensions and format we requested.
        if (m_Texture.width != request.conversionParams.outputDimensions.x ||
            m_Texture.height != request.conversionParams.outputDimensions.y ||
            m_Texture.format != request.conversionParams.outputFormat)
        {
            m_Texture.Resize(
                request.conversionParams.outputDimensions.x,
                request.conversionParams.outputDimensions.y,
                request.conversionParams.outputFormat, false);
        }

        // Copy the image data into the texture
        m_Texture.LoadRawTextureData(rawData);
        m_Texture.Apply();

        // Need to dispose the request to delete resources associated
        // with the request, including the raw data.
        request.Dispose();

        m_AsyncImageFromButton.texture = m_Texture;
    }
}
