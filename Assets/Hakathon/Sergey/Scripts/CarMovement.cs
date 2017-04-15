using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour {

    public float velocity = 0.2f;

    public float rotationVelocity = 1.0f;

    private Rigidbody i_rgdBody = null;
    
    private Vector3 lastSucceedLocation;

    private float timeToWait = 0;

    bool CheckForHanging()
    {
        if (Physics.Raycast(transform.position, -transform.up, 1.0f))
        {
            if (Physics.Raycast(transform.position, transform.right, 10.0f) &&
                Physics.Raycast(transform.position, -transform.right, 10.0f))
                lastSucceedLocation = transform.position;           
            return true;
        }
        if (Physics.Raycast(transform.position, -transform.up, 7.0f))
            return true;
        timeToWait = 2;
        return false;
    }

    bool IsGrounded()
    {
        //Ray downRay = new Ray(transform.position, -transform.up);
        bool flag = Physics.Raycast(transform.position, -transform.up, 1.0f);
        return flag;
    }

    // Use this for initialization
    void Start()
    {
        i_rgdBody = GetComponent<Rigidbody>();
        lastSucceedLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!i_rgdBody) return;

        if (timeToWait > 0)
        {
            timeToWait -= Time.deltaTime;
            return;
        }
        var rot = transform.localEulerAngles;
        //var rot = transform.rotation;
        //setting constraints
        if (rot.x > 40 && rot.x <= 180) rot.x = 40;
        if (rot.x > 180 && rot.x < 320) rot.x = 320;
        if (rot.x < -40) rot.x = -40;
        if (rot.z > 20 && rot.z <= 180) rot.z = 20;
        if(rot.z > 180 && rot.z < 340 ) rot.z = 340;
        if (rot.z < -20) rot.z = -20;
        transform.localEulerAngles = rot;
        //transform.rotation = rot;

        float l_displacement = Input.GetAxis("Vertical") * velocity;
        float l_rotation = Input.GetAxis("Horizontal") * rotationVelocity;

        //i_rgdBody.AddForce(transform.forward * l_displacement);
        //i_rgdBody.AddTorque(Vector3.up * l_rotation * 50);
        if (Input.GetKeyDown(KeyCode.F) && IsGrounded())
            i_rgdBody.AddForce(Vector3.up * 300.0f);
        transform.Rotate(Vector3.up * l_rotation);
        //transform.Rotate(new Vector3(0f, l_rotation, 0f));
        //var rot = transform.rotation;
        //rot.x = 0;
        //transform.rotation = rot;
        //Quaternion.Euler()
        transform.position += transform.forward * l_displacement;

        if (!CheckForHanging()) transform.position = lastSucceedLocation;
    }
}
