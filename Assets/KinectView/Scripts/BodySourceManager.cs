using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System;

public class BodySourceManager : MonoBehaviour 
{
    private KinectSensor i_sensor;
    private BodyFrameReader i_reader;
    private Body[] i_bodies = null;
    public double tiltRadians, tiltDegrees;
    public static BodySourceManager instance {
        get
        {
            if(i_instance == null)
            {
                i_instance = FindObjectOfType<BodySourceManager>();
            }
            return i_instance;
        }
    }
    private static BodySourceManager i_instance;

    public Body firstTrackedBody
    {
        get
        {
            if (i_bodies == null) return null;
            foreach (Body bd in i_bodies)
            {
                if (bd.IsTracked) return bd;
            }
            return null;
        }
    }

    public Body[] bodies
    {
        get
        {
            return i_bodies;
        }
    }

    
    public Body[] GetData()
    {
        return i_bodies;
    }
    

    void Start () 
    {
        i_sensor = KinectSensor.GetDefault();

        if (i_sensor != null)
        {
            if (!i_sensor.IsOpen)
            {
                i_sensor.Open();
            }

            i_reader = i_sensor.BodyFrameSource.OpenReader();
        }   
    }
    
    void Update () 
    {
        if (i_reader != null)
        {
            var frame = i_reader.AcquireLatestFrame();
            if (frame != null)
            {
                if (i_bodies == null)
                {
                    i_bodies = new Body[i_sensor.BodyFrameSource.BodyCount];
                }

                CalculateTilt(frame);

                frame.GetAndRefreshBodyData(i_bodies);
                frame.Dispose();
                frame = null;
            }
        }    
    }

    bool i_isHandsClose = false;
    float i_closeHandsTimeThreshold = 0.5f;
    float i_closeHandsThresh = 0.1f;
    float i_openHandsThresh = 0.2f;
    float i_curTime = 0.0f;
    void ProcessEvents(Body[] a_body)
    {
        if (i_curTime == 0.0f)
        {

        }
        if (!i_isHandsClose)
        {
            
        }

    }

    void CalculateTilt(BodyFrame f)
    {
        try
        {
            Windows.Kinect.Vector4 floorClipPlane;
            floorClipPlane = f.FloorClipPlane;
            tiltRadians = Mathf.Atan(floorClipPlane.Z / floorClipPlane.Y);
            tiltDegrees = tiltRadians * 180 / Mathf.PI;
        }
        catch (Exception e)
        {
        }
    }

    void OnApplicationQuit()
    {
        if (i_reader != null)
        {
            i_reader.Dispose();
            i_reader = null;
        }
        
        if (i_sensor != null)
        {
            if (i_sensor.IsOpen)
            {
                i_sensor.Close();
            }
            
            i_sensor = null;
        }
    }

    public static Body GetFirstTrackedBody(Body[] a_bodies)
    {
        if (a_bodies == null) throw new System.ArgumentNullException("Bodies array is null");
        foreach (Body bd in a_bodies)
        {
            if (bd != null)
            {
                if (bd.IsTracked) return bd;
            }
        }
        return null;
    }

    public static Vector3 CSPtoVector3(CameraSpacePoint a_csp)
    {
        return new Vector3(a_csp.X, a_csp.Y, a_csp.Z);
    }

    public static Quaternion JointOrientationToQuatertnion(JointOrientation a_jointOrient)
    {
        return new Quaternion(a_jointOrient.Orientation.X, a_jointOrient.Orientation.Y,
                                                 a_jointOrient.Orientation.Z, a_jointOrient.Orientation.W);
    }

    public float DistanceBtwnJoints(JointType a_firstJoint, JointType a_secondJoint)
    {
        try
        {
            Vector3 dif = DiffBetweenJoints(a_firstJoint, a_secondJoint);
            return dif.magnitude;
        }
        catch (System.Exception e)
        {
            throw e;
        }
    }

    public Vector3 DiffBetweenJoints(JointType a_firstJoint, JointType a_secondJoint)
    {
        Body body = firstTrackedBody;

        if (body == null)
        {
            throw new System.Exception("no tracked bodies");
        }

        var jointsMap = body.Joints;

        CameraSpacePoint fJointCoords = jointsMap[a_firstJoint].Position;
        CameraSpacePoint sJointCoords = jointsMap[a_secondJoint].Position;
        Vector3 dif = new Vector3(sJointCoords.X - fJointCoords.X,
                                  sJointCoords.Y - fJointCoords.Y,
                                  sJointCoords.Z - fJointCoords.Z);
        return dif;
    }
}
