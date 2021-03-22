using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;

[System.Serializable]
class ModelJoint
{
    public Transform bone;
    public JointId jointId;
    [HideInInspector] public Quaternion baseRotOffset;
}