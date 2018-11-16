using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARExtensions;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(LineRenderer))]
public class SubrectSelector : MonoBehaviour
{
    [SerializeField] Camera m_TargetCamera;

    LineRenderer m_LineRenderer;

    public RectInt screenRect { get; set; }

    public RectInt GetCameraImageRect(CameraImage image)
    {
        return new RectInt(
            screenRect.x * image.width / Screen.width,
            screenRect.y * image.height / Screen.height,
            screenRect.width * image.width / Screen.width,
            screenRect.height * image.height / Screen.height);
    }

    void Awake()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
        m_LineRenderer.widthMultiplier = .001f;
    }

    void SetPosition(Vector3 position)
    {
        var width = Screen.width / 2;
        var height = Screen.height / 2;
        screenRect = new RectInt(
            Mathf.Clamp((int)position.x - width / 2, 0, Screen.width - width),
            Mathf.Clamp((int)position.y - height / 2, 0, Screen.height - height),
            width, height);
    }

    private void Start()
    {
        SetPosition(Vector3.zero);
    }
	
	Vector3 m_Position;

    void Update()
    {
        if (Input.GetMouseButton(0))
			m_Position = Input.mousePosition;
		
		SetPosition(m_Position);
        DrawRect();
    }

    void DrawRect()
    {
        var camera = m_TargetCamera;

        m_LineRenderer.positionCount = 4;
        m_LineRenderer.loop = true;

        var z = camera.nearClipPlane + .001f;
        var r = screenRect;
        m_LineRenderer.SetPosition(0, camera.ScreenToWorldPoint(new Vector3(r.x, r.y, z)));
        m_LineRenderer.SetPosition(1, camera.ScreenToWorldPoint(new Vector3(r.x + r.width, r.y, z)));
        m_LineRenderer.SetPosition(2, camera.ScreenToWorldPoint(new Vector3(r.x + r.width, r.y + r.height, z)));
        m_LineRenderer.SetPosition(3, camera.ScreenToWorldPoint(new Vector3(r.x, r.y + r.height, z)));
    }
}
