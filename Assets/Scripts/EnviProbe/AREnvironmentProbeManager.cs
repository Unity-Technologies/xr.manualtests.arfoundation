using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.EnvironmentProbeSubsystem;
using UnityEngine.Experimental.XR;
using Object = UnityEngine.Object;

public class AREnvironmentProbeManager : MonoBehaviour
{
    public bool m_AutomaticProbePlacement = true;

    public GameObject m_EnvironmentProbePrefab;
    private readonly Dictionary<TrackableId, GameObject> m_IdToGameObject = new Dictionary<TrackableId, GameObject>();
    private readonly Stack<TrackableId> m_EnvironmentProbesToDelete = new Stack<TrackableId>();

    public FilterMode m_EnvironmentTextureFilterMode = FilterMode.Trilinear;

    // Start is called before the first frame update
    void Start()
    {
        if (ARSubsystemManager.environmentProbeSubsystem != null)
        {
            ARSubsystemManager.environmentProbeSubsystem.environmentProbeAdded += HandleEnvironmentProbeAdded;
            ARSubsystemManager.environmentProbeSubsystem.environmentProbeUpdated += HandleEnvironmentProbeUpdated;
            ARSubsystemManager.environmentProbeSubsystem.environmentProbeRemoved += HandleEnvironmentProbeRemoved;

            if (ARSubsystemManager.environmentProbeSubsystem.SubsystemDescriptor.supportsAutomaticPlacement)
            {
                ARSubsystemManager.environmentProbeSubsystem.automaticPlacement = m_AutomaticProbePlacement;
            }

            if (ARSubsystemManager.environmentProbeSubsystem.automaticPlacement)
            {
                Invoke("DoIterate", 15.0f);
            }
            else
            {
                if (ARSubsystemManager.environmentProbeSubsystem.SubsystemDescriptor.supportsManualPlacement)
                {
                    Invoke("DoCreate", 3.0f);
                }
                if (ARSubsystemManager.environmentProbeSubsystem.SubsystemDescriptor.supportsRemovalOfManual)
                {
                    Invoke("DoRemove", 20.0f);
                }
            }
        }
    }

    void OnDestroy()
    {
        if (ARSubsystemManager.environmentProbeSubsystem != null)
        {
            ARSubsystemManager.environmentProbeSubsystem.environmentProbeAdded -= HandleEnvironmentProbeAdded;
            ARSubsystemManager.environmentProbeSubsystem.environmentProbeUpdated -= HandleEnvironmentProbeUpdated;
            ARSubsystemManager.environmentProbeSubsystem.environmentProbeRemoved -= HandleEnvironmentProbeRemoved;
        }

        foreach (GameObject environmentProbeSurrogate in m_IdToGameObject.Values)
        {
            Object.Destroy(environmentProbeSurrogate);
        }
        m_IdToGameObject.Clear();
    }

    void DoIterate()
    {
        Debug.LogFormat("Iterating {0} environment probes ...", ARSubsystemManager.environmentProbeSubsystem.environmentProbesCount);
        int i = 0;
        foreach (XREnvironmentProbe environmentProbe in ARSubsystemManager.environmentProbeSubsystem.environmentProbes)
        {
            Debug.LogFormat("   Environment probe #{0}: {1}", ++i, environmentProbe.ToString());
        }
        Debug.LogFormat("Done iterating {0} environment probes.", ARSubsystemManager.environmentProbeSubsystem.environmentProbesCount);
        Invoke("DoIterate", 10.0f);
    }

    IEnumerator WaitForManualEnvironmentProbeAdd(XREnvironmentProbePromise addRequestPromise)
    {
        XREnvironmentProbeResponse response = addRequestPromise.response;
        while (!response.status.Done())
        {
            Debug.LogFormat("Waiting for {0} : {1}", addRequestPromise.trackableId.ToString(), response.ToString());
            yield return null;
            response = addRequestPromise.response;
        }

        Debug.LogFormat("Creation of {0} : {1}", addRequestPromise.trackableId.ToString(), response.ToString());

        if (response.status == XREnvironmentProbeResponseStatus.SuccessAndActive)
        {
            m_EnvironmentProbesToDelete.Push(addRequestPromise.trackableId);
        }
    }

    void DoCreate()
    {
        Vector3 scale = new Vector3(1.0f, 1.0f, 1.0f);
        Quaternion rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f));
        Vector3 position1 = new Vector3(0.0f, 0.0f, 0.4f);
        Pose pose1 = new Pose(position1, rotation);
        Vector3 size = new Vector3(3.0f, 3.0f, 3.0f);

        XREnvironmentProbeRequest environmentProbeRequest1 = new XREnvironmentProbeRequest(scale, pose1, size);
        XREnvironmentProbePromise requestPromise1 = ARSubsystemManager.environmentProbeSubsystem.AddEnvironmentProbe(environmentProbeRequest1);
        Debug.LogFormat("request to add probe : {0}", requestPromise1.trackableId.ToString());
        StartCoroutine(WaitForManualEnvironmentProbeAdd(requestPromise1));

        Vector3 position2 = new Vector3(0.2f, 0.0f, 0.4f);
        Pose pose2 = new Pose(position2, rotation);

        XREnvironmentProbeRequest environmentProbeRequest2 = new XREnvironmentProbeRequest(scale, pose2, size);
        XREnvironmentProbePromise requestPromise2 = ARSubsystemManager.environmentProbeSubsystem.AddEnvironmentProbe(environmentProbeRequest2);
        Debug.LogFormat("request to add probe : {0}", requestPromise2.trackableId.ToString());
        StartCoroutine(WaitForManualEnvironmentProbeAdd(requestPromise2));

        Vector3 position3 = new Vector3(-0.2f, 0.0f, 0.4f);
        Pose pose3 = new Pose(position3, rotation);

        XREnvironmentProbeRequest environmentProbeRequest3 = new XREnvironmentProbeRequest(scale, pose3, size);
        XREnvironmentProbePromise requestPromise3 = ARSubsystemManager.environmentProbeSubsystem.AddEnvironmentProbe(environmentProbeRequest3);
        Debug.LogFormat("request to add probe : {0}", requestPromise3.trackableId.ToString());
        StartCoroutine(WaitForManualEnvironmentProbeAdd(requestPromise3));

        Vector3 position4 = new Vector3(0.0f, 0.2f, 0.4f);
        Pose pose4 = new Pose(position4, rotation);

        XREnvironmentProbeRequest environmentProbeRequest4 = new XREnvironmentProbeRequest(scale, pose4, size);
        XREnvironmentProbePromise requestPromise4 = ARSubsystemManager.environmentProbeSubsystem.AddEnvironmentProbe(environmentProbeRequest4);
        Debug.LogFormat("request to add probe : {0}", requestPromise4.trackableId.ToString());
        StartCoroutine(WaitForManualEnvironmentProbeAdd(requestPromise4));

        Vector3 position5 = new Vector3(0.0f, -0.2f, 0.4f);
        Pose pose5 = new Pose(position5, rotation);

        XREnvironmentProbeRequest environmentProbeRequest5 = new XREnvironmentProbeRequest(scale, pose5, size);
        XREnvironmentProbePromise requestPromise5 = ARSubsystemManager.environmentProbeSubsystem.AddEnvironmentProbe(environmentProbeRequest5);
        Debug.LogFormat("request to add probe : {0}", requestPromise5.trackableId.ToString());
        StartCoroutine(WaitForManualEnvironmentProbeAdd(requestPromise5));
    }

    void DoRemove()
    {
        if (m_EnvironmentProbesToDelete.Count > 1)
        {
            TrackableId id = m_EnvironmentProbesToDelete.Pop();
            bool status = ARSubsystemManager.environmentProbeSubsystem.RemoveEnvironmentProbe(id);
            Debug.LogFormat("Removing {0} : {1}", id.ToString(), (status ? "SUCCESS" : "FAILURE"));
            Invoke("DoRemove", 2.0f);
        }
        else
        {
            TrackableId id = TrackableId.InvalidId;
            bool status = ARSubsystemManager.environmentProbeSubsystem.RemoveEnvironmentProbe(id);
            Debug.LogFormat("Removing {0} : {1}", id.ToString(), (status ? "SUCCESS" : "FAILURE"));
        }
    }

    void ApplyEnvironmentProbe(XREnvironmentProbe environmentProbe, GameObject surrogate)
    {
        surrogate.transform.localScale = environmentProbe.scale;
        surrogate.transform.localRotation = environmentProbe.pose.rotation;
        surrogate.transform.localPosition = environmentProbe.pose.position;

        if (environmentProbe.environmentTextureData.valid)
        {
            ReflectionProbe rp = surrogate.GetComponent<ReflectionProbe>();
            if (rp != null)
            {
                Cubemap cubemap = Cubemap.CreateExternalTexture(environmentProbe.environmentTextureData.width, environmentProbe.environmentTextureData.format,
                                                                (environmentProbe.environmentTextureData.mipmapCount > 1), environmentProbe.environmentTextureData.nativeTexture);
                cubemap.filterMode = m_EnvironmentTextureFilterMode;

                if (rp.customBakedTexture != null)
                {
                    Object.Destroy(rp.customBakedTexture);
                }
                rp.customBakedTexture = cubemap;
                rp.center = environmentProbe.pose.position;
                rp.size = environmentProbe.size;
            }
        }
    }

    void HandleEnvironmentProbeAdded(XREnvironmentProbeAddedEventArgs environmentProbeAddedEventArgs)
    {
        Debug.LogFormat("Handle {0}", environmentProbeAddedEventArgs.ToString());

        GameObject surrogate = Instantiate<GameObject>(m_EnvironmentProbePrefab, gameObject.transform);
        ApplyEnvironmentProbe(environmentProbeAddedEventArgs.environmentProbe, surrogate);
        m_IdToGameObject.Add(environmentProbeAddedEventArgs.environmentProbe.trackableId, surrogate);
    }

    void HandleEnvironmentProbeUpdated(XREnvironmentProbeUpdatedEventArgs environmentProbeUpdatedEventArgs)
    {
        Debug.LogFormat("Handle {0}", environmentProbeUpdatedEventArgs.ToString());

        GameObject surrogate = null;
        if (m_IdToGameObject.TryGetValue(environmentProbeUpdatedEventArgs.environmentProbe.trackableId, out surrogate))
        {
            ApplyEnvironmentProbe(environmentProbeUpdatedEventArgs.environmentProbe, surrogate);
        }
        else
        {
            Debug.Assert(false, "updating an environment probe that was never added");
        }
    }

    void HandleEnvironmentProbeRemoved(XREnvironmentProbeRemovedEventArgs environmentProbeRemovedEventArgs)
    {
        Debug.LogFormat("Handle {0}", environmentProbeRemovedEventArgs.ToString());

        GameObject surrogate = null;

        if (m_IdToGameObject.TryGetValue(environmentProbeRemovedEventArgs.trackableId, out surrogate))
        {
            m_IdToGameObject.Remove(environmentProbeRemovedEventArgs.trackableId);
            Object.Destroy(surrogate);
        }
        else
        {
            Debug.Assert(false, "deleting an environment probe that was never added");
        }
    }
}
