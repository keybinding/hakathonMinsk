using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class MyVector {
    public float x;
    public float y;
    public float z;
    public MyVector()
    {
        x = z = y = 0f;
    }
    public MyVector(float x_, float y_, float z_)
    {
        x = x_;
        y = y_;
        z = z_;
    }
    public override string ToString()
    {
        return "( " + x + " , " + y + "," + z + ")";
    }

    public MyVector Clone()
    {
        return new MyVector(x, y, z);
    }
}
