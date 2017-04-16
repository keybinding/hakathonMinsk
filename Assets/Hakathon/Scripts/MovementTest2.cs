using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MovementTest2 : NetworkBehaviour {

    private GameObject l_mainCamera = null;
    private Camera l_cam = null;

    private void Start()
    {
        if (isLocalPlayer)
        {
            l_mainCamera = GameObject.FindGameObjectWithTag("MainCamera") as GameObject;
            if (l_mainCamera != null)
            {
                l_cam = l_mainCamera.GetComponent<Camera>();
                /*GameObject CameraInitHelper = GameObject.FindGameObjectWithTag("CameraInit");
                if (CameraInitHelper != null)
                {
                    //l_cam.transform.position = CameraInitHelper.transform.position;
                    //l_cam.transform.rotation = CameraInitHelper.transform.rotation;
                    l_cam.transform.SetParent(gameObject.transform, false);
                }*/
            }
        }
    }

    private void Update()
    {
        if (l_cam != null)
        {
            l_cam.transform.localPosition = transform.localPosition + transform.up * 0.9f - transform.forward * 1f;
            l_cam.transform.localEulerAngles = transform.localEulerAngles;
        }
    }

    // Update is called once per frame
    public void UpdateServer()
    {
        if (isLocalPlayer && isClient)
        {
            CmdRecordPosition(transform.position, transform.rotation);
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
