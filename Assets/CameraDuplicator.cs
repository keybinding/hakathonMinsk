using UnityEngine;
using System.Collections;

public class CameraDuplicator : MonoBehaviour {

    public Camera cameraToCopy = null;
    private Camera i_camera = null;

    void Awake()
    {
        i_camera = GetComponent<Camera>();
    }
    
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	    if(i_camera.transform.position != cameraToCopy.transform.position)
        {
            i_camera.transform.position = cameraToCopy.transform.position;
        }

        if (i_camera.rect != cameraToCopy.rect) i_camera.rect = cameraToCopy.rect;
	}
}
