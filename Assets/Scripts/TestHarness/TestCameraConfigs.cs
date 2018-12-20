using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.XR.ARExtensions;
using UnityEngine.XR.ARFoundation;

[DisallowMultipleComponent]
public class TestCameraConfigs : MonoBehaviour
{
    void OnEnable()
    {
        
    }

    public void DoGUI()
    {
        var cameraSubsystem = ARSubsystemManager.cameraSubsystem;
        if (cameraSubsystem == null)
            return;

        Profiler.BeginSample("foreach_configs");
        var totalWidth = 0;
        foreach (var config in cameraSubsystem.Configurations())
            totalWidth += config.width;
        Profiler.EndSample();

        ButtonManager.NextLine();
        foreach (var config in cameraSubsystem.Configurations())
        {
            ButtonManager.AddButton(config.ToString(), () =>
            {
                cameraSubsystem.SetCurrentConfiguration(config);
            });
        }

        var msg = "Enumerated configs by indexer:";

        var configurations = cameraSubsystem.Configurations();
        for (int i = 0; i < configurations.count; i++)
            msg += string.Format("\nConfig {0} = {1}", i, configurations[i]);

        msg += string.Format("\nCurrent config: {0}", cameraSubsystem.GetCurrentConfiguration());

        GUI.Label(
            new Rect(0, Screen.height * 3 / 4, Screen.width, Screen.height / 4),
            msg,
            ButtonManager.labelStyle);
    }
}
