using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;

public class RecordedPointMan : MonoBehaviour {

    public GameObject SpineBase;
    public GameObject SpineMid;
    public GameObject Neck;
    public GameObject Head;
    public GameObject ShoulderLeft;
    public GameObject ElbowLeft;
    public GameObject WristLeft;
    public GameObject HandLeft;
    public GameObject ShoulderRight;
    public GameObject ElbowRight;
    public GameObject WristRight;
    public GameObject HandRight;
    public GameObject HipLeft;
    public GameObject KneeLeft;
    public GameObject AnkleLeft;
    public GameObject FootLeft;
    public GameObject HipRight;
    public GameObject KneeRight;
    public GameObject AnkleRight;
    public GameObject FootRight;
    public GameObject SpineShoulder;
    public GameObject HandTipLeft;
    public GameObject ThumbLeft;
    public GameObject HandTipRight;
    public GameObject ThumbRight;

    private GameObject[] _bones; //internal handle for the bones of the model
    //private Vector4[] _bonePos; //internal handle for the bone positions from the kinect

    public int player = -1;

    public float scale = 1.0f;
    float timer = 0f;

    private Dictionary<JointType, GameObject> _BoneMap;
    private BodySourceManager bodySourceManager;
    RecordingController recordingController;
    LineRendererController lrc;
    public Vector3 offset;
    public bool isUsingWheelChair { get { return i_isUsingWheelChair; } }
    void Start()
    {
        bodySourceManager = GameObject.FindObjectOfType<BodySourceManager>();
        lrc = this.GetComponent<LineRendererController>();
        recordingController = RecordingController.Instance;
        //store bones in a list for easier access
       _BoneMap = new Dictionary<JointType, GameObject>()
        {
            { JointType.SpineBase, SpineBase},
	        { JointType.SpineMid, SpineMid},
            { JointType.Neck, Neck},
	        { JointType.Head, Head},
	        { JointType.ShoulderLeft, ShoulderLeft},
	        { JointType.ElbowLeft, ElbowLeft},
	        { JointType.WristLeft, WristLeft},
	        { JointType.HandLeft, HandLeft},
	        { JointType.ShoulderRight, ShoulderRight},
	        { JointType.ElbowRight, ElbowRight},
	        { JointType.WristRight, WristRight},
	        { JointType.HandRight, HandRight},
	        { JointType.HipLeft, HipLeft},
	        { JointType.KneeLeft, KneeLeft},
	        { JointType.AnkleLeft, AnkleLeft},
	        { JointType.FootLeft, FootLeft},
	        { JointType.HipRight, HipRight},
	        { JointType.KneeRight, KneeRight},
	        { JointType.AnkleRight, AnkleRight},
	        { JointType.FootRight, FootRight},
            { JointType.SpineShoulder, SpineShoulder},
            { JointType.HandTipLeft, HandTipLeft},
            { JointType.ThumbLeft, ThumbLeft},
            { JointType.HandTipRight, HandTipRight},
            { JointType.ThumbRight, ThumbRight}
        };
        //_bonePos = new Vector4[(int)BoneIndex.Num_Bones];

    }

    public float lerpSpeed = 30f;
    Vector3 noPlayerPosition = new Vector3(0, 0, 1000);
    List<JointType> i_CopiedJoints = new List<JointType>()
    {  JointType.HipLeft
      ,JointType.HipRight
      ,JointType.KneeLeft
      ,JointType.KneeRight
      ,JointType.AnkleLeft
      ,JointType.AnkleRight
      ,JointType.FootLeft
      ,JointType.FootRight };

    public void SetFrame(int frame)
    {
        if (recordingController.recordedClip.Count == 0)
        {
            for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
                _BoneMap[jt].transform.localPosition = noPlayerPosition;
            lrc.RefreshPoints();
            player = -1;
            return;
        }
        Frame skeletonFrame = recordingController.recordedClip[frame];
        if (skeletonFrame == null)
            return;
        float currentAngle = (float)bodySourceManager.tiltRadians;
        float cos = Mathf.Cos(currentAngle);
        float sin = Mathf.Sin(currentAngle);

        CameraSpacePoint[] points = new CameraSpacePoint[skeletonFrame.bones.Length];
        int j = 0;
        ColorSpacePoint[] cPoints = new ColorSpacePoint[skeletonFrame.bones.Length];

        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++, j++)
        {
            MyVector pos = skeletonFrame.bones[j].Clone();
            float newY = pos.y * cos - pos.z * sin;
            float newZ = pos.z * cos + pos.y * sin;
            pos.y = newY - offset.y;
            pos.z = newZ - offset.z;
            points[j] = Frame.MyVectorToCameraSpacePoint(pos);
        }

        

        KinectSensor.GetDefault().CoordinateMapper.MapCameraPointsToColorSpace(points, cPoints);
        j = 0;

        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++, j++)
        {
            Vector3 pos = Frame.ColorSpacePointToVector3(cPoints[j]);
            pos.x -= 960;
            pos.y = 540 - pos.y;

            if (_BoneMap[jt] != null && pos.y != Mathf.Infinity)
                _BoneMap[jt].transform.localPosition = Vector3.Lerp(_BoneMap[jt].transform.localPosition, pos, Time.deltaTime * lerpSpeed);
        }
        lrc.RefreshPoints();
    }


    bool i_isUsingWheelChair = false;

    public void UseWheelChair(bool a_condition) { i_isUsingWheelChair = a_condition; }

    public void SetFrame(Frame frame)
    {
        if (frame == null)
        {
            for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
                _BoneMap[jt].transform.localPosition = noPlayerPosition;
            lrc.RefreshPoints();
            player = -1;
            return;
        }

        float currentAngle = (float)bodySourceManager.tiltRadians;
        float cos = Mathf.Cos(currentAngle);
        float sin = Mathf.Sin(currentAngle);

        CameraSpacePoint[] points = new CameraSpacePoint[frame.bones.Length];
        int j = 0;
        ColorSpacePoint[] cPoints = new ColorSpacePoint[frame.bones.Length];

        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++, j++)
        {
            MyVector pos = frame.bones[j].Clone();
            float newY = pos.y * cos - pos.z * sin;
            float newZ = pos.z * cos + pos.y * sin;
            pos.y = newY - offset.y;
            pos.z = newZ - offset.z;
            pos.x -= offset.x;
            points[j] = Frame.MyVectorToCameraSpacePoint(pos);
            
        }

        if (i_isUsingWheelChair)
        {
            ProcessWheelChair(points);
        }

        KinectSensor.GetDefault().CoordinateMapper.MapCameraPointsToColorSpace(points, cPoints);
        j = 0;
        for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++, j++)
        {
            Vector3 pos = Frame.ColorSpacePointToVector3(cPoints[j]);
            pos.x -= 960;
            pos.y = 540 - pos.y;

            if (_BoneMap[jt] != null && pos.y != Mathf.Infinity)
                _BoneMap[jt].transform.localPosition = Vector3.Lerp(_BoneMap[jt].transform.localPosition, pos, Time.deltaTime * lerpSpeed);
        }

        lrc.RefreshPoints();
    }


    CameraSpacePoint SubstractCSP(CameraSpacePoint a_p1, CameraSpacePoint a_p2)
    {
        return new CameraSpacePoint() { X = a_p1.X - a_p2.X, Y = a_p1.Y - a_p2.Y, Z = a_p1.Z - a_p2.Z };
    }

    CameraSpacePoint JointToJointDirection(JointType a_fromJt, JointType a_toJt, Body a_body)
    {
        return SubstractCSP(a_body.Joints[a_toJt].Position, a_body.Joints[a_fromJt].Position);
    }

    void PreprocessBody(CameraSpacePoint[] a_points, Body a_body)
    {

    }

    void ProcessWheelChair(CameraSpacePoint[] a_points)
    {
        Body l_body = BodySourceManager.instance.firstTrackedBody;
        if (l_body != null)
        {
            List<KeyValuePair<JointType, JointType>> JointPairs = new List<KeyValuePair<JointType, JointType>>()
            {
                new KeyValuePair<JointType, JointType>(JointType.SpineBase, JointType.HipLeft),
                new KeyValuePair<JointType, JointType>(JointType.SpineBase, JointType.HipRight),
                new KeyValuePair<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft),
                new KeyValuePair<JointType, JointType>(JointType.HipRight, JointType.KneeRight),
                new KeyValuePair<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft),
                new KeyValuePair<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight),
                new KeyValuePair<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft),
                new KeyValuePair<JointType, JointType>(JointType.AnkleRight, JointType.FootRight)
            };
            float[] recBoneLengths = new float[JointPairs.Count];
            for(int i = 0; i != recBoneLengths.Length; ++i)
            {
                recBoneLengths[i] = Length(SubstractCSP(a_points[(int)JointPairs[i].Value], a_points[(int)JointPairs[i].Key]));
            }
            for (int i = 0; i != recBoneLengths.Length; ++i)
            {
                CameraSpacePoint realBoneDirection = Normalize(JointToJointDirection(JointPairs[i].Key, JointPairs[i].Value, l_body));
                a_points[(int)JointPairs[i].Value] = SummCsp( MultiplyCSP(realBoneDirection, recBoneLengths[i]), a_points[(int)JointPairs[i].Key]);
            }
        }
        /*
        if (l_body != null)
        {
            CameraSpacePoint SBtoHL = JointToJointDirection(JointType.SpineBase, JointType.HipLeft, l_body);
            CameraSpacePoint SBtoHR = JointToJointDirection(JointType.SpineBase, JointType.HipRight, l_body);
            CameraSpacePoint HLtoKL = JointToJointDirection(JointType.HipLeft, JointType.KneeLeft, l_body);
            CameraSpacePoint HRtoKR = JointToJointDirection(JointType.HipRight, JointType.KneeRight, l_body);
            CameraSpacePoint KLToAL = JointToJointDirection(JointType.KneeLeft, JointType.AnkleLeft, l_body);
            CameraSpacePoint KRtoAR = JointToJointDirection(JointType.KneeRight, JointType.AnkleRight, l_body);
            CameraSpacePoint ALtoFL = JointToJointDirection(JointType.AnkleLeft, JointType.FootLeft, l_body);
            CameraSpacePoint ARtoFR = JointToJointDirection(JointType.AnkleRight, JointType.FootRight, l_body);
            CameraSpacePoint Recorded_SBtoHL = SubstractCSP(a_points[(int)JointType.HipLeft], a_points[(int)JointType.SpineBase]);
            CameraSpacePoint Recorded_SBtoHR = SubstractCSP(a_points[(int)JointType.HipRight], a_points[(int)JointType.SpineBase]);
            CameraSpacePoint Recorded_HLtoKL = SubstractCSP(a_points[(int)JointType.KneeLeft], a_points[(int)JointType.HipLeft]);
            CameraSpacePoint Recorded_HRtoKR = SubstractCSP(a_points[(int)JointType.KneeRight], a_points[(int)JointType.HipRight]);
            CameraSpacePoint Recorded_KLToAL = SubstractCSP(a_points[(int)JointType.AnkleLeft], a_points[(int)JointType.KneeLeft]);
            CameraSpacePoint Recorded_KRtoAR = SubstractCSP(a_points[(int)JointType.AnkleRight], a_points[(int)JointType.KneeRight]);
            CameraSpacePoint Recorded_ALtoFL = SubstractCSP(a_points[(int)JointType.FootLeft], a_points[(int)JointType.AnkleLeft]);
            CameraSpacePoint Recorded_ARtoFR = SubstractCSP(a_points[(int)JointType.FootRight], a_points[(int)JointType.AnkleRight]);
            a_points[(int)JointType.HipLeft] = MultiplyCSP(Normalize(SBtoHL), Length(Recorded_SBtoHL));
            
        }
        */
    }

    CameraSpacePoint Normalize(CameraSpacePoint a_vector)
    {

        return MultiplyCSP(a_vector, 1 / Length(a_vector));
    }

    float Length(CameraSpacePoint a_vector)
    {
        return Mathf.Sqrt(a_vector.X * a_vector.X + a_vector.Y * a_vector.Y + a_vector.Z * a_vector.Z);
    }

    CameraSpacePoint MultiplyCSP(CameraSpacePoint a_vector, float a_value)
    {
        return new CameraSpacePoint() { X = a_vector.X * a_value, Y = a_vector.Y * a_value, Z = a_vector.Z * a_value };
    }

    CameraSpacePoint SummCsp(CameraSpacePoint a_vec1, CameraSpacePoint a_vec2)
    {
        return new CameraSpacePoint() { X = a_vec1.X + a_vec2.X, Y = a_vec1.Y + a_vec2.Y, Z = a_vec1.Z + a_vec2.Z };
    }
}
