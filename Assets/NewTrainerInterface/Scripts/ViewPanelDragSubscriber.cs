using UnityEngine;
using UnityEngine.EventSystems;
using Windows.Kinect;
using System.Collections.Generic;
using System.Collections;
using System;

[RequireComponent(typeof(Camera))]
public class ViewPanelDragSubscriber : MonoBehaviour {
    
    public BodySourceManager bodyManager = null;
    public GameObject rotationHelper = null;

    private static List<ViewPanelDragSubscriber> s_allSubscribers = new List<ViewPanelDragSubscriber>();
    private static List<ViewPanelDragSubscriber> s_selectedPanels = new List<ViewPanelDragSubscriber>();

    private Vector3 i_dragLateralAxis;
    private Vector3 i_dragVerticalAxis;
    private Vector3 i_dragPivotPoint;
    private Camera i_subscribedCamera;
    private float i_distance = 2f;

    void Awake()
    {
        s_allSubscribers.Add(this);
        i_subscribedCamera = GetComponent<Camera>();
    }

    void Update()
    {
        
        if (SimpleAvatar.instance.isFirsFrameArrived)
        {

            Vector3 l_bodyPos = SimpleAvatar.instance.jointsMap[JointType.SpineBase].transform.position;

            if (s_selectedPanels.Contains(this))
            {
                //i_subscribedCamera.transform.position +
                float delta = Input.mouseScrollDelta.y / 10f;
                i_distance = Mathf.Clamp(i_distance + delta, 0f, 5f);
            }

            Camera l_cam = GetComponent<Camera>();
            if (l_cam.transform.forward == new Vector3(0f, 0f, 1f))
            {
                l_cam.transform.position = new Vector3(l_bodyPos.x, l_cam.transform.position.y, l_bodyPos.z - i_distance);//l_cam.transform.position.z);
            }
            else
            {
                if (l_cam.transform.forward == new Vector3(1f, 0f, 0f))
                {
                    l_cam.transform.position = new Vector3(l_bodyPos.x - i_distance, l_cam.transform.position.y, l_bodyPos.z);
                }
                else
                {
                    if(l_cam.transform.forward == new Vector3(-1f, 0f, 0f))
                    {
                        l_cam.transform.position = new Vector3(l_bodyPos.x + i_distance, l_cam.transform.position.y, l_bodyPos.z);
                    }
                }
            }
        }
    }

    public void OnPanelDragStart(BaseEventData a_data)
    {
        DeselectAllPanels();
        //get a pivot axis
        i_dragPivotPoint = CalculateDragPivotPoint();
    }

    private Vector3 CalculateDragPivotPoint()
    {
        Vector3 l_result = Vector3.zero;
        Body l_body = null;
        if ((l_body = bodyManager.firstTrackedBody) != null)
        {
            l_result = BodySourceManager.CSPtoVector3(l_body.Joints[JointType.SpineMid].Position);
            l_result = new Vector3(l_result.x, 0f, l_result.z);
        }

        return l_result;
    }

    public void OnPanelDrag(BaseEventData a_data)
    {
        PointerEventData l_data = (PointerEventData)a_data;
        //get a pivot axis
        i_subscribedCamera.transform.RotateAround(i_dragPivotPoint, new Vector3(0f, 1f, 0f), l_data.delta.x);
    }

    public void OnPanelDragFinish(BaseEventData a_data)
    {
        s_selectedPanels.Add(this);
    }

    private Vector3 CalculateDragPivotAxis(bool a_isLateral, Vector3 a_pivotPoint)
    {
        Vector3 l_result = Vector3.zero;
        rotationHelper.transform.position = a_pivotPoint;
        rotationHelper.transform.forward = transform.position - a_pivotPoint;
        if (a_isLateral) l_result = rotationHelper.transform.up;
        else l_result = rotationHelper.transform.right;
        return l_result;
    }

    public void OnPanelSelect(BaseEventData a_data)
    {
        
        DeselectAllPanels();
        s_selectedPanels.Add(this);
    }

    private void DeselectAllPanels()
    {
        s_selectedPanels.Clear();
    }
}
