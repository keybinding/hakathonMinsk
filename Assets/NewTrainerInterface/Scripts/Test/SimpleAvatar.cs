using UnityEngine;
using System.Collections.Generic;
using Windows.Kinect;
using System.Collections;

public class SimpleAvatar : MonoBehaviour {

    public BodySourceManager skeletonSource = null;

    public GameObject joint = null;
    public GameObject bone = null;

    public bool isFirsFrameArrived {
        get { return i_isFirstFrameArrived; }
    }

    private bool i_isFirstFrameArrived = false;

    public static SimpleAvatar instance
    {
        get
        {
            if (i_instance == null)
            {
                i_instance = FindObjectOfType<SimpleAvatar>();
            }
            return i_instance;
        }
    }
    private static SimpleAvatar i_instance;

    public void OnVtPlayerFrameChanged(Frame a_newFrame)
    {
        if(a_newFrame != null)
        {
            if(!i_isFirstFrameArrived) i_isFirstFrameArrived = true;
            UpdateJoints(a_newFrame);
            UpdateBones(a_newFrame);
        }
    }

    List<JointType> i_jointTypes = new List<JointType>()
    {
        JointType.AnkleLeft,
        JointType.AnkleRight,
        JointType.ElbowLeft,
        JointType.ElbowRight,
        JointType.FootLeft,
        JointType.FootRight,
        JointType.HandLeft,
        JointType.HandRight,
        //JointType.HandTipLeft,
        //JointType.HandTipRight,
        JointType.Head,
        JointType.HipLeft,
        JointType.HipRight,
        JointType.KneeLeft,
        JointType.KneeRight,
        //JointType.Neck,
        JointType.ShoulderLeft,
        JointType.ShoulderRight,
        JointType.SpineBase,
        JointType.SpineMid,
        JointType.SpineShoulder,
        //JointType.ThumbLeft,
        //JointType.ThumbRight,
        JointType.WristLeft,
        JointType.WristRight
    };

    List<JointType[]> i_bones = new List<JointType[]>()
    {
        new JointType[] {JointType.FootLeft, JointType.AnkleLeft},
        new JointType[] {JointType.AnkleLeft, JointType.KneeLeft},
        new JointType[] {JointType.KneeLeft, JointType.HipLeft},
        new JointType[] {JointType.HipLeft, JointType.SpineBase},
        new JointType[] {JointType.FootRight, JointType.AnkleRight},
        new JointType[] {JointType.AnkleRight, JointType.KneeRight},
        new JointType[] {JointType.KneeRight, JointType.HipRight},
        new JointType[] {JointType.HipRight, JointType.SpineBase},
        new JointType[] {JointType.HandLeft, JointType.WristLeft},
        new JointType[] {JointType.WristLeft, JointType.ElbowLeft},
        new JointType[] {JointType.ElbowLeft, JointType.ShoulderLeft},
        new JointType[] {JointType.ShoulderLeft, JointType.SpineShoulder},
        new JointType[] {JointType.HandRight, JointType.WristRight},
        new JointType[] {JointType.WristRight, JointType.ElbowRight},
        new JointType[] {JointType.ElbowRight, JointType.ShoulderRight},
        new JointType[] {JointType.ShoulderRight, JointType.SpineShoulder},
        new JointType[] {JointType.SpineBase, JointType.SpineMid},
        new JointType[] {JointType.SpineMid, JointType.SpineShoulder},
        new JointType[] {JointType.SpineShoulder, JointType.Head}
    };

    public Dictionary<JointType[], GameObject> bonesMap { get { return i_bonesMap; } }
    public Dictionary<JointType, GameObject> jointsMap { get { return i_jointsMap; } }

    Dictionary<JointType, GameObject> i_jointsMap = new Dictionary<JointType, GameObject>();
    Dictionary<JointType[], GameObject> i_bonesMap = new Dictionary<JointType[], GameObject>();

    

	// Use this for initialization
	void Awake () {
        //create joints
	    foreach(var l_jt in i_jointTypes)
        {
            i_jointsMap.Add(l_jt, Instantiate(joint));
            if (l_jt == JointType.Head)
            {
                i_jointsMap[l_jt].transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
            i_jointsMap[l_jt].transform.SetParent(gameObject.transform, true);
            i_jointsMap[l_jt].name = l_jt.ToString();
        }
        //create bones
        foreach(var l_bone in i_bones)
        {
            i_bonesMap.Add(l_bone, Instantiate(bone));
            i_bonesMap[l_bone].transform.SetParent(gameObject.transform, true);
        }
	}

    void UpdateBones(Body a_body)
    {
        foreach(var l_bone in i_bonesMap)
        {
            Vector3 boneStartPos = BodySourceManager.CSPtoVector3(a_body.Joints[l_bone.Key[0]].Position);
            Vector3 boneEndPos = BodySourceManager.CSPtoVector3(a_body.Joints[l_bone.Key[1]].Position);
            Vector3 dif = boneEndPos - boneStartPos;            
            l_bone.Value.transform.forward = dif.normalized;
            l_bone.Value.transform.localScale = new Vector3(bone.transform.localScale.x, bone.transform.localScale.y, dif.magnitude);
            l_bone.Value.transform.position = (boneStartPos + boneEndPos) / 2.0f;
        }
    }

    void UpdateJoints(Body a_body)
    {
        foreach (var l_joint in i_jointsMap)
        {
            i_jointsMap[l_joint.Key].transform.position = BodySourceManager.CSPtoVector3(a_body.Joints[l_joint.Key].Position);
            i_jointsMap[l_joint.Key].transform.rotation = BodySourceManager.JointOrientationToQuatertnion(a_body.JointOrientations[l_joint.Key]);
        }
    }

    void UpdateJoints(Frame a_frame)
    {
        if (a_frame == null) return;
        int j = 0;
        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++, j++)
        {
            MyVector pos = a_frame.bones[j].Clone();
            if (i_jointsMap.ContainsKey(jt))
            {
                i_jointsMap[jt].transform.position = new Vector3(a_frame.bones[j].x, a_frame.bones[j].y, a_frame.bones[j].z);
            }
        }
    }

    void UpdateBones(Frame a_frame)
    {
        foreach (var l_bone in i_bonesMap)
        {
            Vector3 boneStartPos = i_jointsMap[l_bone.Key[0]].transform.position;
            Vector3 boneEndPos = i_jointsMap[l_bone.Key[1]].transform.position;
            Vector3 dif = boneEndPos - boneStartPos;
            l_bone.Value.transform.forward = dif.normalized;
            l_bone.Value.transform.localScale = new Vector3(bone.transform.localScale.x, bone.transform.localScale.y, dif.magnitude);
            l_bone.Value.transform.position = (boneStartPos + boneEndPos) / 2.0f;
        }
    }
}
