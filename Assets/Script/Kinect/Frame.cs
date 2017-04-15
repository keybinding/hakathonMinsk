using UnityEngine;
using System.Collections;
using System;
using Windows.Kinect;
[Serializable]
public class Frame{
    public float deltaTime;
    public MyVector[] bones;
    public Frame(Body body, float deltaTime_)
    {
        bones = new MyVector[body.Joints.Count];
        int i = 0;
        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++, i++)
            bones[i] = Frame.CameraSpacePosintToMyVector(body.Joints[jt].Position);
        this.deltaTime = deltaTime_;
    }

    public Frame(double angle, Body body, float deltaTime_)
    {
        float currentAngle = (float)angle;
        float cos = Mathf.Cos(currentAngle);
        float sin = Mathf.Sin(currentAngle);

        bones = new MyVector[body.Joints.Count];
        int i = 0;
        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++, i++)
        {
            MyVector pos = Frame.CameraSpacePosintToMyVector(body.Joints[jt].Position);
            float newY = pos.y * cos + pos.z * sin;
            float newZ = pos.z * cos - pos.y * sin;
            pos.y = newY;
            pos.z = newZ;
            bones[i] = pos;
        }
        this.deltaTime = deltaTime_;
    }

    public static Vector3 CameraSpacePointToVector3(CameraSpacePoint csp)
    {
        return new Vector3(csp.X, csp.Y, csp.Z);
    }

    public static Vector3 ColorSpacePointToVector3(ColorSpacePoint csp)
    {
        return new Vector3(csp.X, csp.Y, 0);
    }

    public static Vector3 MyVectorToVector3(MyVector myvector)
    {
        return new Vector3(myvector.x, myvector.y, myvector.z);
    }

    public static CameraSpacePoint MyVectorToCameraSpacePoint(MyVector myvector)
    {
        CameraSpacePoint point = new CameraSpacePoint();
        point.X = myvector.x;
        point.Y = myvector.y;
        point.Z = myvector.z;
        return point;
    }

    public static MyVector Vector3ToMyVector(Vector3 vector)
    {
        return new MyVector(vector.x, vector.y, vector.z);
    }

    public static MyVector CameraSpacePosintToMyVector(CameraSpacePoint csp)
    {
        return new MyVector(csp.X, csp.Y, csp.Z);
    }

    public Vector3 GetPositionInVector3(int id)
    {
        return new Vector3(bones[id].x, bones[id].y, bones[id].z); 
    }

}
