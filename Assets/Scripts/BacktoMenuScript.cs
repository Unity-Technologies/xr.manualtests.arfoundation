using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class BacktoMenuScript : MonoBehaviour {

    float holdTime = 1.0f;
    float acumTime = 0;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            GetComponentInChildren<ARCameraBackground>().enabled = false;
            Debug.Log("Loading Menu Scene");
            SceneManager.LoadScene("TestSelection", LoadSceneMode.Single);
        }

        if (Input.touchCount > 0)
        {
            acumTime += Input.GetTouch(0).deltaTime;

            if (acumTime >= holdTime)
            {
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
