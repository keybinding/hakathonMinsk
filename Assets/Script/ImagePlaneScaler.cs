using UnityEngine;
using System.Collections;
using Windows.Kinect;

public class ImagePlaneScaler : MonoBehaviour {
    public float distance = 4f;
    public Camera outputCamera;
    public Camera secondary;

    void Awake()
    {
        float distance = transform.position.z - outputCamera.transform.position.z;
        float height = 2.0f * Mathf.Tan(0.5f * outputCamera.fieldOfView * Mathf.Deg2Rad) * (distance - 0.5f);
        float width = height * Screen.width / Screen.height;
        this.transform.localScale = new Vector3(width, height, 1f);
    }

    void Start()
    {
        if (secondary != null)
        {
            secondary.gameObject.SetActive(true);
        }
    }
   
    void AjustPositionToCamViewport()
    {
        
    }
}
