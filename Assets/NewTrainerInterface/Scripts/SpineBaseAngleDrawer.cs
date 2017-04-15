using UnityEngine;
using System.Collections.Generic;

public class SpineBaseAngleDrawer : JointsAngleDrawer
{
    protected override Vector3 Joint1Pos
    {
        get
        {
            return simpleAvatar.jointsMap[Windows.Kinect.JointType.SpineMid].transform.position;
        }
    }

    protected override Vector3 Joint2Pos
    {
        get
        {
            return (simpleAvatar.jointsMap[Windows.Kinect.JointType.KneeLeft].transform.position 
                 + simpleAvatar.jointsMap[Windows.Kinect.JointType.KneeRight].transform.position) / 2f;
        }
    }

    protected override Vector3 ArcJointPos
    {
        get
        {
            return simpleAvatar.jointsMap[Windows.Kinect.JointType.SpineBase].transform.position ;
        }
    }
    // Use this for initialization
    void Start () {
        i_mf = GetComponent<MeshFilter>();
        gameObject.SetActive(false);
        angleLable.gameObject.SetActive(false);
        i_ArcJointGo = simpleAvatar.jointsMap[Windows.Kinect.JointType.SpineBase].transform;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 l_joint1End = ArcJointPos + (Joint1Pos - ArcJointPos).normalized * angleSideSize;
        Vector3 l_joint2End = ArcJointPos + (Joint2Pos - ArcJointPos).normalized * angleSideSize;
        List<Vector3> segmentEndsPos = GetVertices(l_joint1End
                                                 , l_joint2End
                                                 , ArcJointPos, 5f);
        i_mf.mesh = GenerateArcMesh(segmentEndsPos);


        if (angleLable != null)
        {
            Vector3 l_jointsMid = (Joint1Pos + Joint2Pos) / 2f;
            float l_frontal = isFrontal ? -1f : 1f;
            Vector3 l_axis = Vector3.Cross(l_joint1End - ArcJointPos, l_joint2End - ArcJointPos);
            float l_sign = Mathf.Sign(Vector3.Dot(i_ArcJointGo.forward * l_frontal, l_axis));
            Vector3 l_lablePos = outPutCamera.WorldToScreenPoint(ArcJointPos + (l_jointsMid - ArcJointPos).normalized * lableOffset * l_sign);
            angleLable.text = i_angle.ToString();
            angleLable.rectTransform.anchoredPosition = new Vector2(l_lablePos.x, l_lablePos.y);
        }

        
    }
}
