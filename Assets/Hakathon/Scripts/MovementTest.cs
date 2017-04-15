using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class MovementTest : NetworkBehaviour
{

    public float velocity = 1.0f;

    public float rotationVelocity = 1.0f;

    private Rigidbody i_rgdBody = null;

    private Vector3 i_syncPos = new Vector3();
    private Quaternion i_syncRotation = new Quaternion();
    [SerializeField] Transform syncTransform = null;
    [SerializeField] float lerprate = 15f;

    private Text i_debugMessage = null;

	// Use this for initialization
	void Start () {
        i_rgdBody = GetComponent<Rigidbody>();
	}

    private void Update()
    {
        if (!isLocalPlayer) return;
        
        float l_displacement = Input.GetAxis("Vertical") * velocity;
        float l_rotation = Input.GetAxis("Horizontal") * rotationVelocity;
        transform.Rotate(new Vector3(0f, l_rotation, 0f));
        transform.position += transform.forward * l_displacement;
    }

    private void FixedUpdate()
    {
        TransmitPos();
        LerpPosition();
    }

    void LerpPosition()
    {
        if (isLocalPlayer) return;
        syncTransform.position = Vector3.Lerp(syncTransform.position, i_syncPos, lerprate * Time.deltaTime);
        syncTransform.rotation = Quaternion.Lerp(syncTransform.rotation, i_syncRotation, lerprate * Time.deltaTime);
    }

    [Command]
    void CmdSendPosToServer(Vector3 a_pos, Quaternion a_rot)
    {
        i_syncPos = a_pos;
        i_syncRotation = a_rot;
    }

    [ClientCallback]
    void TransmitPos()
    {
        if (isLocalPlayer)
        {
            CmdSendPosToServer(syncTransform.position, syncTransform.rotation);
        }
    }
    
}
