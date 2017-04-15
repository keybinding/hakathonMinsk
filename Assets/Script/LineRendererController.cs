using UnityEngine;
using System.Collections;

public class LineRendererController : MonoBehaviour {
    public Transform[] points;
    LineRenderer lr;	
	void Start () {
        lr = GetComponent<LineRenderer>();
        lr.SetVertexCount(points.Length);
        Vector3[] pos = new Vector3[points.Length];
        for (int i =0;i<pos.Length; i++)
            pos[i] = new Vector3();
        lr.SetPositions(pos);
	}

   
    public void RefreshPoints()
    {
        for (int i = 0; i < points.Length; i++)
            lr.SetPosition(i, points[i].transform.position);
    }
}
