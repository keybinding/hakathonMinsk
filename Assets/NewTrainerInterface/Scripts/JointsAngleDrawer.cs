using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;
using System.Collections.Generic;

public class JointsAngleDrawer : MonoBehaviour
{

    public JointType Joint1;
    public JointType Joint2;
    public JointType ArcJoint;
    public GameObject Helper;
    public SimpleAvatar simpleAvatar;
    public Camera outPutCamera;
    public float angleSideSize = 0.1f;
    public Text angleLable = null;
    public float lableUpdatePerios = 1.0f;
    public float lableOffset = 0.1125f;

    virtual protected Vector3 Joint1Pos { get { return i_Joint1Go.position; } }
    virtual protected Vector3 Joint2Pos { get { return i_Joint2Go.position; } }
    virtual protected Vector3 ArcJointPos { get { return i_ArcJointGo.position; } }

    protected MeshFilter i_mf;
    protected Transform i_Joint1Go;
    protected Transform i_Joint2Go;
    protected Transform i_ArcJointGo;
    protected int i_angle = 0;
    public bool isFrontal;

    // Use this for initialization
    void Start()
    {
        i_mf = GetComponent<MeshFilter>();
        i_Joint1Go = simpleAvatar.jointsMap[Joint1].transform;
        i_Joint2Go = simpleAvatar.jointsMap[Joint2].transform;
        i_ArcJointGo = simpleAvatar.jointsMap[ArcJoint].transform;
        gameObject.SetActive(false);
        angleLable.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update ()
    {
        AngleDrawerUpdate();
    }

    protected void AngleDrawerStart()
    {
        i_mf = GetComponent<MeshFilter>();
        gameObject.SetActive(false);
        angleLable.gameObject.SetActive(false);
        i_ArcJointGo = simpleAvatar.jointsMap[Windows.Kinect.JointType.SpineBase].transform;
    }

    protected void AngleDrawerUpdate()
    {
        Vector3 l_diff1 = Joint1Pos - ArcJointPos;
        Vector3 l_diff2 = Joint2Pos - ArcJointPos;

        //Debug.Log(l_diff2);
        if (isFrontal)
        {
            l_diff1 = new Vector3(l_diff1.x, l_diff1.y, 0f);
            l_diff2 = new Vector3(l_diff2.x, l_diff2.y, 0f);
        }
        else
        {
            l_diff1 = new Vector3(0f, l_diff1.y, l_diff1.z);
            l_diff2 = new Vector3(0f, l_diff2.y, l_diff2.z);
        }

        Vector3 l_joint1End = Vector3.zero;
        Vector3 l_joint2End = Vector3.zero;
        if (l_diff1.sqrMagnitude > 0.01f && l_diff2.sqrMagnitude > 0.01f)
        {
            l_diff1 = l_diff1.normalized * angleSideSize;
            l_diff2 = l_diff2.normalized * angleSideSize;

            l_joint1End = ArcJointPos + l_diff1;
            l_joint2End = ArcJointPos + l_diff2;

            List<Vector3> segmentEndsPos = GetVertices(l_joint1End
                                                     , l_joint2End
                                                     , ArcJointPos, 5f);
            i_mf.mesh = GenerateArcMesh(segmentEndsPos);
        }
        else
        {
            l_joint1End = ArcJointPos;
            l_joint2End = ArcJointPos;
            i_mf.mesh = new Mesh();
            i_angle = 0;
        }

        if (angleLable != null)
        {
            Vector3 l_jointsMid = (l_joint1End + l_joint2End) / 2f;
            float l_frontal = isFrontal ? -1f : 1f;
            Vector3 l_axis = Vector3.Cross(l_joint1End - ArcJointPos, l_joint2End - ArcJointPos);
            //float l_sign = Mathf.Sign(Vector3.Dot(i_ArcJointGo.forward * l_frontal, l_axis));
            
            Vector3 l_lablePos = outPutCamera.WorldToScreenPoint(ArcJointPos + (l_jointsMid - ArcJointPos).normalized * lableOffset);

            angleLable.text = i_angle.ToString();
            
            angleLable.rectTransform.anchoredPosition = new Vector2(l_lablePos.x, l_lablePos.y);

            if(i_angle == 0)
            {
                angleLable.enabled = false;
            }
            else
            {
                angleLable.enabled = true;
            }
        }
        
    }

    protected List<Vector3> GetVertices(Vector3 a_bone1EndPos, Vector3 a_bon2EndPos, Vector3 a_arcOrigin, float a_segmentSize)
    {
        List<Vector3> l_result = new List<Vector3>();
        float l_angle = 0.0f;
        Vector3 l_axis = Vector3.zero;
        int segmentSize = 5;
        Vector3 l_oTo1Dir = a_bone1EndPos - a_arcOrigin;
        Vector3 l_oTo2Dir = a_bon2EndPos - a_arcOrigin;
        //Quaternion.FromToRotation(l_oTo1Dir, l_oTo2Dir).ToAngleAxis(out l_angle, out l_axis);
        

        //float l_frontal = isFrontal ? -1f : 1f;
        l_angle = Mathf.Acos( Vector3.Dot(l_oTo1Dir, l_oTo2Dir) / (l_oTo1Dir.magnitude * l_oTo2Dir.magnitude)) * Mathf.Rad2Deg;
        
        l_axis = Vector3.Cross(l_oTo1Dir, l_oTo2Dir);
        Vector3 l_crossPr1 = Vector3.Cross(l_axis, l_oTo1Dir);
        float l_sign = Mathf.Sign(Vector3.Dot(l_axis, l_crossPr1));

        if(l_sign < 0f)
        {
            l_axis *= -1f;
            l_angle = 360 - l_angle;
        }
        i_angle = (int)l_angle;
        int segmentCount = (int)l_angle / segmentSize;
        l_result.Add(a_arcOrigin);
        l_result.Add(a_bone1EndPos);
        Helper.transform.position = a_bone1EndPos;
        for (int i = 1; i <= segmentCount; ++i)
        {
            Helper.transform.RotateAround(a_arcOrigin, l_axis, segmentSize);
            l_result.Add(Helper.transform.position);
        }
        //create last vertex position
        l_result.Add(a_bon2EndPos);
        return l_result;
    }

    protected Mesh GenerateArcMesh(List<Vector3> a_vertices)
    {
        Mesh l_result = new Mesh();

        l_result.vertices = a_vertices.ToArray();
        List<int> l_triangles = new List<int>();

        for (int i = 1; i != a_vertices.Count - 1; ++i)
        {
            l_triangles.Add(0);
            l_triangles.Add(i);
            l_triangles.Add(i + 1);
        }

        for (int i = a_vertices.Count - 1; i != 1; --i)
        {
            l_triangles.Add(0);
            l_triangles.Add(i);
            l_triangles.Add(i - 1);
        }
        l_result.triangles = l_triangles.ToArray();
        l_result.normals = new Vector3[a_vertices.Count];
        Vector2[] l_uv = new Vector2[a_vertices.Count];
        float l_uvStep = 1.0f / (a_vertices.Count - 1);
        l_uv[0] = new Vector2();
        for (int i = 1; i != a_vertices.Count; ++i)
        {
            l_uv[i] = new Vector2(l_uvStep * i, 1);
        }
        l_result.uv = l_uv;
        return l_result;
    }

    public void SetObjectsActive(bool a_value)
    {
        gameObject.SetActive(a_value);
        angleLable.gameObject.SetActive(a_value);
    }

    public void SetProjection(bool a_isFrontal)
    {
        isFrontal = a_isFrontal;
    }
}
