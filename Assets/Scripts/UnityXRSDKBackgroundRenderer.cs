//-----------------------------------------------------------------------
// <copyright file="ARCoreBackgroundRenderer.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR;

//// TODO (mtsmall): Consider if this component is the best way to expose background rendering and discuss approach
//// with Unity.

/// <summary>
/// Renders the device's camera as a background to the attached Unity camera component.
/// </summary>
public class UnityXRSDKBackgroundRenderer : MonoBehaviour
{
    /// <summary>
    /// A material used to render the AR background image.
    /// </summary>
    [Tooltip("A material used to render the AR background image.")]
    //public Material BackgroundMaterial;

    public Camera m_Camera;

    public UnityEngine.XR.ARFoundation.ARCameraBackground m_BackgroundComponent;

    public CameraClearFlags clearFlag = CameraClearFlags.Skybox;

    private void OnEnable()
    {
        if (Application.isEditor)
        {
            enabled = false;
            return;
        }

        //if (BackgroundMaterial == null)
        //{
        //    Debug.LogError("ArCameraBackground:: No material assigned.");
        //    return;
        //}

        m_BackgroundComponent = GetComponent<UnityEngine.XR.ARFoundation.ARCameraBackground>();

        if (m_BackgroundComponent == null)
        {
            Debug.LogError("ARBackgroundRender:: Could not retrieve ARBackgroundRenderer Component");
            return;
        }
    }

    private void Update()
    {
        if (m_BackgroundComponent == null)
        {
            return;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {

            Debug.Log("Touch Detected");

            if (m_BackgroundComponent.enabled == true)
            {
                Debug.Log("Switching to Standard Background");
                m_BackgroundComponent.enabled = false;
                m_Camera.clearFlags = clearFlag;
            }
            else
            {
                Debug.Log("Switching to Material Background");
                m_BackgroundComponent.enabled = true;
            }
        }
    }
}

