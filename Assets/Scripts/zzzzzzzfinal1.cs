using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using Joint = Microsoft.Azure.Kinect.BodyTracking.Joint;

public class zzzzzzzfinal1 : MonoBehaviour
{
    [Header("Rigged model")]
    [SerializeField]
    ModelJoint[] modelJoints;

    Dictionary<JointId, ModelJoint> jointsRigged = new Dictionary<JointId, ModelJoint>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < modelJoints.Length; i++)
        {
            modelJoints[i].baseRotOffset = modelJoints[i].bone.rotation;
            jointsRigged.Add(modelJoints[i].jointId, modelJoints[i]);   //modelJoints[i].jointType.TryGetMirrored()
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void updateTracker(BackgroundData trackerFrameData)
    {
        //this is an array in case you want to get the n closest bodies
        int closestBody = findClosestTrackedBody(trackerFrameData);

        // render the closest body
        Body skeleton = trackerFrameData.Bodies[closestBody];

        ProcessSkeleton(skeleton);
    }


    private int findClosestTrackedBody(BackgroundData trackerFrameData)
    {
        int closestBody = -1;
        const float MAX_DISTANCE = 5000.0f;
        float minDistanceFromKinect = MAX_DISTANCE;
        for (int i = 0; i < (int)trackerFrameData.NumOfBodies; i++)
        {
            var pelvisPosition = trackerFrameData.Bodies[i].JointPositions3D[(int)JointId.Pelvis];
            Vector3 pelvisPos = new Vector3((float)pelvisPosition.X, (float)pelvisPosition.Y, (float)pelvisPosition.Z);
            if (pelvisPos.magnitude < minDistanceFromKinect)
            {
                closestBody = i;
                minDistanceFromKinect = pelvisPos.magnitude;
            }
        }
        return closestBody;
    }

    void ProcessSkeleton(Body skeleton)
    {

        //Vector3 torsoPos = new Vector3(skeleton.JointPositions3D[2].X, -skeleton.JointPositions3D[2].Y, skeleton.JointPositions3D[2].Z);  //Vector3 torsoPos = Quaternion.Euler(0f, 180f, 0f) * (0.001f * skeleton.GetJoint(nuitrack.JointType.Torso).ToVector3());
        //transform.position = torsoPos;

        int jointNum;

        foreach (var riggedJoint in jointsRigged)
        {
            jointNum = (int)riggedJoint.Key;
            ModelJoint modelJoint = riggedJoint.Value;
            Quaternion jointOrient = new Quaternion(skeleton.JointRotations[jointNum].X, skeleton.JointRotations[jointNum].Y, skeleton.JointRotations[jointNum].Z, skeleton.JointRotations[jointNum].W) * GetKinectTPoseOrientationInverse(riggedJoint.Key) * modelJoint.baseRotOffset;
            modelJoint.bone.rotation = jointOrient;
        }

    }

    
    private static Quaternion GetKinectTPoseOrientationInverse(JointId jointId)
    {
        // Used this page as reference for T-pose orientations
        // https://docs.microsoft.com/en-us/azure/Kinect-dk/body-joints
        // Assuming T-pose as body facing Z+, with head at Y+. Same for target character
        // Coordinate system seems to be left-handed not right handed as depicted
        // Thus inverse T-pose rotation should align Y and Z axes of depicted local coords for a joint with body coords in T-pose
        switch (jointId)
        {
            case JointId.Pelvis:
            case JointId.SpineNavel:
            case JointId.SpineChest:
            case JointId.Neck:
            case JointId.Head:
            case JointId.HipLeft:
            case JointId.KneeLeft:
            case JointId.AnkleLeft:
                return Quaternion.AngleAxis(90, Vector3.forward) * Quaternion.AngleAxis(-90, Vector3.up);

            case JointId.FootLeft:
                return Quaternion.AngleAxis(-90, Vector3.up);

            case JointId.HipRight:
            case JointId.KneeRight:
            case JointId.AnkleRight:
                return Quaternion.AngleAxis(-90, Vector3.forward) * Quaternion.AngleAxis(-90, Vector3.up);

            case JointId.FootRight:
                return Quaternion.AngleAxis(180, Vector3.forward) * Quaternion.AngleAxis(-90, Vector3.up);

            case JointId.ClavicleLeft:
            case JointId.ShoulderLeft:
            case JointId.ElbowLeft:
                return Quaternion.AngleAxis(90, Vector3.right);

            case JointId.WristLeft:
                return Quaternion.AngleAxis(180, Vector3.right);

            case JointId.ClavicleRight:
            case JointId.ShoulderRight:
            case JointId.ElbowRight:
                return Quaternion.AngleAxis(-90, Vector3.right);

            case JointId.WristRight:
                return Quaternion.identity;

            default:
                return Quaternion.identity;
        }
    }

}
