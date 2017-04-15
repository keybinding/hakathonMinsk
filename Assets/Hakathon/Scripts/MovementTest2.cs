using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovementTest2 : NetworkBehaviour {


    public float velocity = 0.2f;

    public float rotationVelocity = 1.0f;

    public float lerpK = 0.2f;

    private float i_curLerp = 0f;
    private float i_curRotLerp = 0f;

    private Vector3 lastVelocity = Vector3.zero;

    [SyncVar]
    Vector3 position = Vector3.zero;

    [SyncVar]
    Quaternion rotation = Quaternion.identity;
    

    Vector3 oldPosition = Vector3.zero;
    Quaternion oldRotation = Quaternion.identity;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update()
    {
        if (isClient && !isLocalPlayer)
        {
            /*
            if (oldPosition != position)
            {
                i_curLerp += lerpK;
                Vector3 newPosition = Vector3.Lerp(oldPosition, position, i_curLerp);
                lastVelocity = newPosition - oldPosition;
                oldPosition = newPosition;
                transform.position = oldPosition;
            }
            else
            {
                i_curLerp = 0f;
                transform.position += Time.deltaTime * lastVelocity;
            }
            if(oldRotation != rotation)
            {
                i_curRotLerp += lerpK;
                Quaternion newRot = Quaternion.Lerp(oldRotation, rotation, i_curRotLerp);
                oldRotation = newRot;
                transform.rotation = oldRotation;
            }
            else
            {
                i_curRotLerp = 0f;
            }
            */
        }
        else
        {
            if (isLocalPlayer && isClient)
            {
                float l_displacement = Input.GetAxis("Vertical") * velocity;
                float l_rotation = Input.GetAxis("Horizontal") * rotationVelocity;
                transform.Rotate(new Vector3(0f, l_rotation, 0f));
                transform.position += transform.forward * l_displacement;
                CmdRecordPosition(transform.position, transform.rotation);
            }
        }

	}

    [Command]
    void CmdRecordPosition(Vector3 a_vec, Quaternion a_rot)
    {
        RpcUpdateTransform(a_vec, a_rot);
    }

    [ClientRpc]
    void RpcUpdateTransform(Vector3 a_vec, Quaternion a_rot)
    {
        if (!isLocalPlayer)
        {
            transform.position = a_vec;
            transform.rotation = a_rot;
        }
    }
}
