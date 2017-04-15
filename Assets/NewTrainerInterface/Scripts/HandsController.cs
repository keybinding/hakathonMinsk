using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class HandsController : MonoBehaviour {

    public float holdTimer = 1.2f;

    bool i_wasControlling = false;
    bool i_wasHoldingRight = false;
    bool i_wasHoldingLeftt = false;
    bool i_idle = false;
    float i_holdTimer = 0.0f;
    BodySourceManager i_bodyManager = null;

	// Use this for initialization
	void Start () {
        i_bodyManager = BodySourceManager.instance;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (i_idle) return;

        bool l_isControlling = isControlling();
        if (i_wasControlling && l_isControlling)
        {
            //continues controll

        }
        else if(!i_wasControlling && l_isControlling)
        {
            //started controlling
        }
        else if(i_wasControlling && !l_isControlling)
        {
            //stoped controlling
        }
        else
        {
            //does't controll
            Reset();
        }        
	}

    void Reset()
    {
        i_wasControlling = false;
        i_wasHoldingRight = false;
        i_wasHoldingLeftt = false;
        i_holdTimer = 0.0f;
    }

    bool isRightHandUp()
    {
        Body l_body = null;
        if (!isTrackingValid(ref l_body)) return false;
        if (l_body.Joints[JointType.HandRight].TrackingState != TrackingState.Tracked || l_body.Joints[JointType.ElbowRight].TrackingState != TrackingState.Tracked) return false;
        return (l_body.Joints[JointType.WristRight].Position.Z - l_body.Joints[JointType.ElbowRight].Position.Z) > 0.0f;
    }

    bool isLeftHandUp()
    {
        Body l_body = null;
        if (!isTrackingValid(ref l_body)) return false;
        if (l_body.Joints[JointType.HandLeft].TrackingState != TrackingState.Tracked || l_body.Joints[JointType.ElbowLeft].TrackingState != TrackingState.Tracked) return false;
        return (l_body.Joints[JointType.HandLeft].Position.Z - l_body.Joints[JointType.ElbowLeft].Position.Z) > 0.0f;
    }

    bool isControlling() { return isRightHandUp() || isLeftHandUp(); }

    bool isHolding()
    {
        Body l_body = null;
        if (!isTrackingValid(ref l_body)) return false;
        return (l_body.HandLeftConfidence == TrackingConfidence.High && l_body.HandLeftState == HandState.Closed ||
                l_body.HandRightConfidence == TrackingConfidence.High && l_body.HandRightState == HandState.Closed);
    }

    bool isTrackingValid(ref Body a_body)
    {
        if (i_bodyManager == null) return false;
        Body l_body = i_bodyManager.firstTrackedBody;
        if (l_body == null) return false;
        a_body = l_body;
        return true;
    }

    public void Stop()
    {
        Reset();
        i_idle = true;
    }

    public void Run()
    {
        i_idle = false;
    }
}
