using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public enum TestMode
{
    CameraImage,
    CameraConfigs
}

[RequireComponent(typeof(TestSimpleCameraImage))]
[RequireComponent(typeof(TestCameraConfigs))]
public class TestUI : MonoBehaviour
{
    public TestMode mode = TestMode.CameraImage;

    List<MonoBehaviour> m_Behaviours;


    ARCameraBackground m_ARCameraBackground;


    void Start()
    {
        m_Behaviours = new List<MonoBehaviour>();
        m_Behaviours.Add(GetComponent<TestSimpleCameraImage>());
    }

    void OnGUI()
    {
        ButtonManager.OnGUI();

        var testNames = TestMode.GetNames(typeof(TestMode));

        for (int i = 0; i < testNames.Length; ++i)
        {
            var testMode = (TestMode)i;
            ButtonManager.AddButton(testMode.ToString(), () =>
            {
                mode = testMode;
            });
        }
        


        if (mode == TestMode.CameraConfigs)
            GetComponent<TestCameraConfigs>().DoGUI();

    }

    void EnableOnly<T>() where T : MonoBehaviour
    {
        var typeToEnable = typeof(T);
        foreach(var behaviour in m_Behaviours)
        {
            behaviour.enabled = (behaviour.GetType() == typeToEnable);
        }
    }

    void Update()
    {
        switch (mode)
        {

            case TestMode.CameraImage:
                EnableOnly<TestSimpleCameraImage>();
                break;

            case TestMode.CameraConfigs:
                EnableOnly<TestCameraConfigs>();
                break;

        }
    }
}
