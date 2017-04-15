using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class CameraSwitcher : MonoBehaviour {

    public List<Camera> cameras = new List<Camera>();

	// Use this for initialization
	void Start () {
	    foreach(Camera l_cam in cameras)
        {
            l_cam.gameObject.SetActive(true);
        }
	}
}
