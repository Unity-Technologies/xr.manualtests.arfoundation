using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSelectionScript : MonoBehaviour
{
    public enum MenuItem
    {
        SimpleAR,
        MotionTracking,
        MagicWindowSwitch,
        PlaneDetection,
        ARParticles,
        NavMesh,
        RefPointTest,
        PlaneVideoPlayer
    }

    public Canvas m_DescriptionCanvas;
    public Canvas m_MenuCanvas;
    public Text m_DescriptionTitle;
    public Text m_DescriptionBody;
    public string m_SceneName = string.Empty;

    public void LoadDescription(string title)
    {
        string description = string.Empty;
        string descriptionTitle = title;
        m_SceneName = title;

        switch(title)
        {
            case ("SimpleARTest"):
                description = "\n<size=40>Description:</size>" +
                    "\n View a simple game object in augmented reality." +
                    "\n\n" +
                    "\n<size=40>What to Verify:</size>" +
                    "\n 1) Moving the device updates the camera feed" +
                    "\n 2) Can walk around and view game object from different angles";
                break;
            case ("PoseTrackingTest"):
                description = "\n<size=40>Description:</size>" +
                    "\n Walk around with the phone and draw a trail indicating where you \n have traveled." +
                    "\n\n" +
                    "\n<size=40>What to Verify:</size>" +
                    "\n 1) Walk around an area and then point the device where you walked: \n you should see a general trail of where you have been" +
                    "\n 2) The trail will not be exact, but there should be no drift \n occuring (trail moves without you moving the phone)";
                break;
            case ("MagicWindowTest"):
                description = "\n<size=40>Description:</size>" +
                    "\n Switch between AR (camera background) and non-AR Mode \n (space skybox background)." +
                    "\n\n" +
                    "\n<size=40>What to Verify:</size>" +
                    "\n 1) Tapping on screen will switch between AR and non-AR mode" +
                    "\n 2) Game objects can be see in the same locations in both AR \n and non-AR" +
                    "\n 3) Rotate the device between taps and make sure the images on \n screen are clear" +
                    "\n 4) Rotate the device between taps and make sure moving the device \n moves the camera in the expected direction";
                break;
            case ("ARParticles"):
                description = "\n<size=40>Description:</size>" +
                    "\n Star particles fall across the screen wherever the device is pointed" +
                    "\n\n" +
                    "\n<size=40>What to Verify:</size>" +
                    "\n 1) Particle effects are seen on the screen" +
                    "\n 2) When the device is rotated, particles continue to fall from \n top of the screen" +
                    "\n 3) Tapping on the screen causes meteor gameobject/particles \n to spawn";
                break;
            case ("NavMeshTest"):
                description = "\n<size=40>Description:</size>" +
                    "\n Places a Navmesh onto planes found in the scene." +
                    "\n\n" +
                    "\n<size=40>What to Verify:</size>" +
                    "\n 1) Move the device around to find planes." +
                    "\n 2) Tap on a large plane to place a space invader game object." +
                    "\n 3) If gameobject does not spawn, try to enlarge the detected plane" +
                    "\n 4) Once space invader is placed, tapping on the plane causes it to \n move around.";                 
                break;
            case ("RefPointTest"):
                description = "\n<size=40>Description:</size>" +
                    "\n Add reference points to a scene through touch and delete them." +
                    "\n\n" +
                    "\n<size=40>What to Verify:</size>" +
                    "\n 1) Find a plane and tap to place Pose reference points" +
                    "\n 2) Use drop down to switch to Plane reference points and tap \n on plane to add" +
                    "\n 3) Press delete button to remove all reference points. If any \n remain, check log for errors.";
                break;
            case ("PlaneVideoPlayerTest"):
                description = "\n<size=40>Description:</size>" +
                    "\n Play a video on a detected plane." +
                    "\n\n" +
                    "\n<size=40>What to Verify:</size>" +
                    "\n 1) Detect a flat plane in the environment" +
                    "\n 2) Hit the button to place and play a video on the plane" +
                    "\n 3) If video is not scene, enlarge the plane" +
                    "\n 4) Video should only play on the plane; make sure it does not appear \n in front of the screen";
                break;
            case ("PlaneDetectionTest"):
                description = "\n<size=40>Description:</size>" +
                    "\n Find a plane and place game objects on them." +
                    "\n\n" +
                    "\n<size=40>What to Verify:</size>" +
                    "\n 1) Planes are detected" +
                    "\n 2) Tapping on plane places a game object that faces the screen";
                break;
            case ("CameraImageApiTester"):
                description = "\n<size=40>Description:</size>" +
                    "\n Automated CameraApiTest." +
                    "\n\n" +
                    "\n<size=40>Test is UNDER CONSTRUCTION and currently NON-FUNCTIONAL</size>";
                break;
            default:
                description = "\n<size=26>Description:</size>" +
                    "\n A Description for this test is not yet available";
                break;
        }

        m_MenuCanvas.gameObject.SetActive(false);
        m_DescriptionCanvas.gameObject.SetActive(true);
        m_DescriptionTitle.text = descriptionTitle;
        m_DescriptionBody.text = description;
    }

    public void LoadScene()
    {
        TestSceneLoader loader = Camera.main.GetComponent<TestSceneLoader>();
        loader.ChangeScene(m_SceneName);
    }
}
