using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class BackCatcher : MonoBehaviour
{

    float holdTime = 1.0f;
    float acumTime = 0;

    public void Update()
    {

        if (Application.platform == RuntimePlatform.Android)
        {

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Screen.orientation = ScreenOrientation.AutoRotation;
                //quit application on return button
                GetComponentInChildren<ARCameraBackground>().enabled = false;
                SceneManager.LoadScene("TestSelection");

                return;

            }
        }

        if (Input.touchCount > 0)
        {
            acumTime += Input.GetTouch(0).deltaTime;

            if (acumTime >= holdTime)
            {
                Screen.orientation = ScreenOrientation.AutoRotation;
                GetComponentInChildren<ARCameraBackground>().enabled = false;
                Debug.Log("Loading Menu Scene");
                SceneManager.LoadScene("TestSelection", LoadSceneMode.Single);
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                acumTime = 0;
            }
        }
    }
}
