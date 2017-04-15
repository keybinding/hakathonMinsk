using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovementTest : NetworkBehaviour
{

    public float velocity = 1.0f;

    public float rotationVelocity = 1.0f;

    private Rigidbody i_rgdBody = null;

	// Use this for initialization
	void Start () {
        i_rgdBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
            return;

        if (i_rgdBody == null) return;
        float l_displacement = Input.GetAxis("Vertical") * velocity;
        float l_rotation = Input.GetAxis("Horizontal") * rotationVelocity;
        transform.Rotate(new Vector3(0f, l_rotation, 0f));
        transform.position += transform.forward * l_displacement;
        
	}
}
