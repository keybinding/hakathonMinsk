using UnityEngine;
using System.Collections;

public class ShoulderAngleDrawer : JointsAngleDrawer
{
    protected override Vector3 Joint1Pos
    {
        get
        {
            return simpleAvatar.jointsMap[ArcJoint].transform.position + new Vector3(0, -1, 0);
        }
    }

    protected override Vector3 ArcJointPos
    {
        get
        {
            return simpleAvatar.jointsMap[ArcJoint].transform.position;
        }
    }

    protected override Vector3 Joint2Pos
    {
        get
        {
            return simpleAvatar.jointsMap[Joint1].transform.position;
        }
    }

    // Use this for initialization
    void Start () {
        AngleDrawerStart();
	}
	
	// Update is called once per frame
	void Update () {
        AngleDrawerUpdate();
	}
}
