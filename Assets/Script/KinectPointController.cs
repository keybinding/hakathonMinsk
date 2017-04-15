using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Kinect;


public class KinectPointController : MonoBehaviour {
	
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
    private Dictionary<JointType, GameObject> _BoneMap;
	public int player;
	
	public float scale = 1.0f;
    LineRendererController lrc;
    BodySourceManager bodySourceManager;

    public float lerpSpeed = 10f;
    public Vector3 LegsPosition;
    public Vector3 SpineBasePosition;
    void Start () {
        lrc = GetComponent<LineRendererController>();
        bodySourceManager = GameObject.FindObjectOfType<BodySourceManager>();
		//_bonePos = new Vector4[(int)BoneIndex.Num_Bones];
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
	}

    Vector3 startPosition = new Vector3(0, 0, 1000);

    void Update()
    {
       
        Body body = GetTrackedBody();
        
        if (body != null)
        {
            var shoulder = body.Joints[JointType.ShoulderLeft];
            var elbow = body.Joints[JointType.ElbowRight];
            var wrist = body.Joints[JointType.WristRight];
            
            /*  float currentAngle = (float)bodySourceManager.tiltRadians;
              float cos = Mathf.Cos(currentAngle);
              float sin = Mathf.Sin(currentAngle);*/
            CameraSpacePoint[] points = new CameraSpacePoint[body.Joints.Count];
            int j = 0;
            ColorSpacePoint[] cPoints = new ColorSpacePoint[body.Joints.Count];
            LegsPosition = (Frame.CameraSpacePointToVector3(body.Joints[JointType.FootLeft].Position) + Frame.CameraSpacePointToVector3(body.Joints[JointType.FootRight].Position))/2f;
            SpineBasePosition = Frame.CameraSpacePointToVector3(body.Joints[JointType.SpineBase].Position);
            for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++, j++)
            {
                
                points[j] = body.Joints[jt].Position;
            }
            KinectSensor.GetDefault().CoordinateMapper.MapCameraPointsToColorSpace(points,cPoints);
            j = 0;
            for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++, j++)
            {
                //Vector3 pos = Frame.CameraSpacePointToVector3(body.Joints[jt].Position);
                Vector3 pos = Frame.ColorSpacePointToVector3(cPoints[j]);
                pos.x -= 960;
                pos.y = 540 - pos.y;
              /*  float newY = pos.y * cos + pos.z * sin;
                float newZ = pos.z * cos - pos.y * sin;
                pos.y = newY;
                pos.z = newZ;*/
                if (pos.y!=Mathf.Infinity)
                    _BoneMap[jt].transform.localPosition = Vector3.Lerp(_BoneMap[jt].transform.localPosition, pos, lerpSpeed * Time.deltaTime);
            }
        }
        else
        {
            for (JointType jt = JointType.SpineBase; jt <= JointType.ThumbRight; jt++)
            {
                _BoneMap[jt].transform.localPosition = startPosition;
            }
        }
        lrc.RefreshPoints();
	}

    

    public Body GetTrackedBody()
    {
        Body[] bodies = bodySourceManager.GetData();
        if (bodies == null)
            return null;
        player = -1;
        foreach (Body b in bodySourceManager.GetData())
        {
            if (b.IsTracked)
            {
                player = 1;
                return b;
            }
        }
        return null;
    }


    GameObject arc = null;
    public void DrawArc()
    {
        if (arc == null)
        {
            arc = new GameObject("arc", new System.Type[] { typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider) });

            arc.transform.position = ElbowRight.transform.position;
            arc.transform.localScale = ElbowRight.transform.localScale;
            arc.transform.SetParent(transform, false);
            arc.transform.position = ElbowRight.transform.position;
            arc.layer = gameObject.layer;
        }

        int num_segments = 30;
        List<Vector3> l_points = new List<Vector3>();
        
        Body body = GetTrackedBody();
        if (body == null) return;
        Vector3 velbow = ElbowRight.transform.position;
        Vector3 vshoulder = ShoulderRight.transform.position;
        Vector3 vwrist = WristRight.transform.position;
        Vector3 elbowToShoulder = vshoulder - velbow;
        Vector3 elbowToWrist = vwrist - velbow;
        float radius = elbowToShoulder.magnitude < elbowToWrist.magnitude ? elbowToShoulder.magnitude : elbowToWrist.magnitude ;
        radius /= 2.0f;
        elbowToWrist = elbowToWrist.normalized * radius;
        elbowToShoulder = elbowToShoulder.normalized * radius;
        Quaternion rotation = Quaternion.FromToRotation(elbowToShoulder, elbowToWrist);
        float angle;
        Vector3 axis;
        rotation.ToAngleAxis(out angle, out axis);
        float segmentAngle = angle / num_segments;
        rotation = Quaternion.AngleAxis(segmentAngle, axis);
        Vector3 beg = elbowToShoulder;
        l_points.Add(beg);
        for(int i = 0; i != num_segments; ++i)
        {
            beg = rotation * beg;
            l_points.Add(beg);
        }

        Mesh m = new Mesh();
        m.name = "arc";
        List<Vector3> withPivot = new List<Vector3>();
        withPivot.Add(Vector3.zero);
        foreach (var v in l_points) withPivot.Add(v);
        m.vertices = withPivot.ToArray();
        List<int> triangles = new List<int>();
        for(int i = 1; i!= withPivot.Count - 1; ++i)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        for (int i = withPivot.Count - 1; i != 1; --i)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i - 1);
        }
        List<Vector3> normals = new List<Vector3>();
        
        for (int i = 0; i != m.vertices.Length; ++i)
        {
            normals.Add(Vector3.zero);
        }
        m.triangles = triangles.ToArray();
        m.normals = normals.ToArray();
        arc.transform.position = velbow;
        arc.GetComponent<MeshFilter>().mesh = m;
    }


    GameObject arcl = null;
    public void DrawArcL()
    {
        if (arcl == null)
        {
            arcl = new GameObject("arc", new System.Type[] { typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider) });

            arcl.transform.position = ElbowRight.transform.position;
            arcl.transform.localScale = ElbowRight.transform.localScale;
            arcl.transform.SetParent(transform, false);
            arcl.transform.position = ElbowRight.transform.position;
            arcl.layer = gameObject.layer;
        }

        int num_segments = 30;
        List<Vector3> l_points = new List<Vector3>();

        Body body = GetTrackedBody();
        if (body == null) return;
        Vector3 velbow = ElbowLeft.transform.position;
        Vector3 vshoulder = ShoulderLeft.transform.position;
        Vector3 vwrist = WristLeft.transform.position;
        Vector3 elbowToShoulder = vshoulder - velbow;
        Vector3 elbowToWrist = vwrist - velbow;
        float radius = elbowToShoulder.magnitude < elbowToWrist.magnitude ? elbowToShoulder.magnitude : elbowToWrist.magnitude;
        radius /= 2.0f;
        elbowToWrist = elbowToWrist.normalized * radius;
        elbowToShoulder = elbowToShoulder.normalized * radius;
        Quaternion rotation = Quaternion.FromToRotation(elbowToShoulder, elbowToWrist);
        float angle;
        Vector3 axis;
        rotation.ToAngleAxis(out angle, out axis);
        float segmentAngle = angle / num_segments;
        rotation = Quaternion.AngleAxis(segmentAngle, axis);
        Vector3 beg = elbowToShoulder;
        l_points.Add(beg);
        for (int i = 0; i != num_segments; ++i)
        {
            beg = rotation * beg;
            l_points.Add(beg);
        }

        Mesh m = new Mesh();
        m.name = "arcl";
        List<Vector3> withPivot = new List<Vector3>();
        withPivot.Add(Vector3.zero);
        foreach (var v in l_points) withPivot.Add(v);
        m.vertices = withPivot.ToArray();
        List<int> triangles = new List<int>();
        for (int i = 1; i != withPivot.Count - 1; ++i)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i + 1);
        }

        for (int i = withPivot.Count - 1; i != 1; --i)
        {
            triangles.Add(0);
            triangles.Add(i);
            triangles.Add(i - 1);
        }
        List<Vector3> normals = new List<Vector3>();

        for (int i = 0; i != m.vertices.Length; ++i)
        {
            normals.Add(Vector3.zero);
        }
        m.triangles = triangles.ToArray();
        m.normals = normals.ToArray();
        arcl.transform.position = velbow;
        arcl.GetComponent<MeshFilter>().mesh = m;
    }
}
