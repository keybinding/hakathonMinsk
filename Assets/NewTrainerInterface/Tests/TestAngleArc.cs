using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TestAngleArc : MonoBehaviour {

    public GameObject Bone1;
    public GameObject Bone2;
    public GameObject dbg;
    public MeshFilter mf;

    private Vector3 i_bone1EndPos;
    private Vector3 i_bone2EndPos;
    private Vector3 i_arcOrigin;
    private Vector3 i_oTo1Dir;
    private Vector3 i_oTo2Dir;
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        Vector3 l_arcOrigin = Bone1.transform.parent.position;
        List<Vector3> segmentEndsPos = GetVertices(l_arcOrigin + Bone1.transform.up * 5.0f,
                                                   l_arcOrigin + Bone2.transform.up * 5.0f,
                                                   l_arcOrigin, 5f);
        
        mf.mesh = GenerateArcMesh(segmentEndsPos, l_arcOrigin);



    }

    List<Vector3> GetVertices(Vector3 a_bone1EndPos, Vector3 a_bon2EndPos, Vector3 a_arcOrigin, float a_segmentSize)
    {
        List<Vector3> l_result = new List<Vector3>();
        float l_angle = 0.0f;
        Vector3 l_axis = Vector3.zero;
        int segmentSize = 5;
        Vector3 l_oTo1Dir = a_bone1EndPos - a_arcOrigin;
        Vector3 l_oTo2Dir = a_bon2EndPos - a_arcOrigin;
        Vector3 l_cross = Vector3.Cross(l_oTo1Dir, l_oTo2Dir).normalized;

        /*
        float angle = Vector3.Angle(l_oTo1Dir, l_oTo2Dir);//
        Plane l_pl = new Plane(a_bone1EndPos, a_bon2EndPos, a_arcOrigin);//
        float sign = Mathf.Sign(Vector3.Dot(l_pl.normal, l_cross));//
        float signedAngle = sign * angle;//
        float angle360 = (signedAngle + 360) % 360;//
        */

        Quaternion.FromToRotation(l_oTo1Dir, l_oTo2Dir).ToAngleAxis(out l_angle, out l_axis);
        //l_axis = l_pl.normal;//
        /*
        if (Mathf.Abs(l_cross.x - l_axis.x) > 0.01f || (l_cross.y - l_axis.y) > 0.01f || Mathf.Abs(l_cross.z - l_axis.z) > 0.01f)
        {
            l_axis = l_cross;
            l_angle = 360f - l_angle;
        }
        */

        int segmentCount = (int)l_angle / segmentSize;

        //segmentCount = (int)angle360 / segmentSize;//

        l_result.Add(a_arcOrigin);
        l_result.Add(a_bone1EndPos);
        dbg.SetActive(true);
        dbg.transform.position = a_bone1EndPos;
        for (int i = 1; i <= segmentCount; ++i)
        {
            dbg.transform.RotateAround(a_arcOrigin, l_axis, segmentSize);
            l_result.Add(dbg.transform.position);
        }
        //create last vertex position
        l_result.Add(a_bon2EndPos);
        dbg.SetActive(false);
        return l_result;
    }

    Mesh GenerateArcMesh(List<Vector3> a_vertices, Vector3 a_arcOrigin)
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
        //l_result.RecalculateNormals();
        return l_result;
    }
    
}
