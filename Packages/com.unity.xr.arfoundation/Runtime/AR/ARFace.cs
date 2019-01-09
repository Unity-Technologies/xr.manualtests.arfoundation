using System;
using System.Collections.Generic;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.FaceSubsystem;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Represents a face detected by an AR device.
    /// </summary>
    /// <remarks>
    /// Generated by the <see cref="ARFaceManager"/> when an AR device detects
    /// a face in the environment.
    /// </remarks>
    [DisallowMultipleComponent]
    public sealed class ARFace : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("If true, this component's GameObject will be removed immediately when the plane is removed.")]
        bool m_DestroyOnRemoval = true;

        /// <summary>
        /// If true, this component's <c>GameObject</c> will be removed immediately when the face is removed.
        /// </summary>
        /// <remarks>
        /// Setting this to false will keep the face's <c>GameObject</c> around. You may want to do this, for example,
        /// if you have custom removal logic, such as a fade out.
        /// </remarks>
        public bool destroyOnRemoval
        {
            get { return m_DestroyOnRemoval; }
            set { m_DestroyOnRemoval = value; }
        }

        /// <summary>
        /// The <c>XRFace</c> data struct which defines this <see cref="ARFace"/>.
        /// </summary>
        public XRFace xrFace
        {
            get { return m_XRFace; }

            internal set
            {
                m_XRFace = value;

                lastUpdatedFrame = Time.frameCount;

                var pose = m_XRFace.pose;
                transform.localPosition = pose.position;
                transform.localRotation = pose.rotation;
                m_TrackingState = m_XRFace.isTracked ? UnityEngine.Experimental.XR.TrackingState.Tracking : UnityEngine.Experimental.XR.TrackingState.Unavailable;

                if (updated != null)
                {
                    updated(this);
                }
            }
        }

        /// <summary>
        /// Gets the current <c>TrackingState</c> of this <see cref="ARFace"/>.
        /// </summary>
        public UnityEngine.Experimental.XR.TrackingState trackingState
        {
            get
            {
                if (!m_TrackingState.HasValue || ARSubsystemManager.faceSubsystem == null)
                {
                    m_TrackingState = UnityEngine.Experimental.XR.TrackingState.Unknown;
                }
 
                return m_TrackingState.Value;
            }
        }

        /// <summary>
        /// The last frame on which this plane was updated.
        /// </summary>
        public int lastUpdatedFrame { get; private set; }

        /// <summary>
        /// Invoked whenever the face updates
        /// </summary>
        public event Action<ARFace> updated;

        /// <summary>
        /// Invoked just before the face is about to be removed.
        /// </summary>
        public event Action<ARFace> removed;

        /// <summary>
        /// Attempts to fill in the List of Vector3 with the face mesh vertex positions in face space.
        /// </summary>
        /// <param name="verticesOut">If successful, the contents are replaced with the <see cref="ARFace"/>'s mesh vertex positions.</param>
        /// <returns>True if the  mesh vertex positions were successfully retrieved.</returns>
        public bool TryGetFaceMeshVertices(List<Vector3> verticesOut)
        {
            return ARSubsystemManager.faceSubsystem.TryGetFaceMeshVertices(m_XRFace.trackableId, verticesOut);
        }

        /// <summary>
        /// Attempt to fill in the List of Vector2 that are the face mesh texture coordinates.
        /// </summary>
        /// <param name="uvsOut">If successful, the contents are replaced with the <see cref="ARFace"/>'s mesh texture coordinates.</param>
        /// <returns>True if the mesh texture coordinates were successfully retrieved.</returns>
        public bool TryGetFaceMeshUVs(List<Vector2> uvsOut)
        {
            return ARSubsystemManager.faceSubsystem.TryGetFaceMeshUVs(m_XRFace.trackableId, uvsOut);
        }

        /// <summary>
        /// Fills in the List of int that are the face mesh triangle indices.
        /// </summary>
        /// <param name="indicesOut">If successful, the contents are replaced with the <see cref="ARFace"/>'s mesh triangle indices.</param>
        /// <returns>True if the mesh triangle indices were successfully retrieved.</returns>
        public bool TryGetFaceMeshIndices(List<int> indicesOut)
        {
            return ARSubsystemManager.faceSubsystem.TryGetFaceMeshIndices(m_XRFace.trackableId, indicesOut);
        }

        internal void OnRemove()
        {
            if (removed != null)
            {
                removed(this);
            }

            if (destroyOnRemoval)
            {
                Destroy(gameObject);
            }
        }

        XRFace m_XRFace;

        UnityEngine.Experimental.XR.TrackingState? m_TrackingState;
    }
}
