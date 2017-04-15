using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Windows.Kinect;

public class PrimaryView : MonoBehaviour {

    public
    class CameraDoesntExist : System.Exception
    {
        public CameraDoesntExist() { }
        public CameraDoesntExist(string message) : base(message) { }
        public CameraDoesntExist(string message, System.Exception inner) : base(message, inner) { }
        protected CameraDoesntExist(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    public Camera viewCamera;
    public ModelViewOptionsPanel.Perspectives initialPerspective;
    public GameObject OptionsPanel = null;
    public GameObject angleLables = null;
    public Color activeColor;
    public Color inactiveColor;
    public Image viewLablePanelImg = null;

    private RectTransform i_rt;
    private static List<PrimaryView> i_selectedViews = new List<PrimaryView>();
    private static List<PrimaryView> s_views = new List<PrimaryView>();


    void Awake()
    {
        i_rt = (RectTransform)transform;
    }

	// Use this for initialization
	void Start () {
        if (viewCamera == null) throw new CameraDoesntExist("no camera attached to that view");
        AdjustCameraViewPortRect();
        if (tag == "InitialView") SelectAndActivateOptions();
        s_views.Add(this);
        //OnPerspectiveSelected(initialPerspective);
    }

    private void AdjustCameraViewPortRect()
    {
        Rect l_viewPortRect = GetCanvasSize();
        
        Rect l_rect = ((RectTransform)transform).rect;
        float ll_cameraX = transform.position.x / l_viewPortRect.width;
        float ll_cameraY = transform.position.y / l_viewPortRect.height;
        float ll_cameraWidth = l_rect.width / l_viewPortRect.width;
        float ll_cameraHeight = l_rect.height / l_viewPortRect.height;
        viewCamera.rect = new Rect(ll_cameraX, ll_cameraY, ll_cameraWidth, ll_cameraHeight);
    }

    Rect GetCanvasSize()
    {
        RectTransform l_canvas = transform as RectTransform;
        
        while (l_canvas.parent != null)
        {
            l_canvas = l_canvas.parent as RectTransform;
        }
        return l_canvas.rect;
    }

    public void OnParentPanelResize(RectTransform a_panel)
    {
        AdjustCameraViewPortRect();
    }

    public void OnPerspectiveSelected(ModelViewOptionsPanel.Perspectives a_perspective)
    {
        
        Vector3 l_cameraPosition = viewCamera.transform.position;
        Dictionary<JointType, GameObject> l_body = SimpleAvatar.instance.jointsMap;
        float l_distance = l_cameraPosition.magnitude;
        Vector3 l_origin = Vector3.zero;
        
        if(l_body != null)
        {
            l_origin = l_body[JointType.SpineBase].transform.position;
            l_origin = new Vector3(l_origin.x, 0f, l_origin.z);
            l_distance = (l_cameraPosition - l_origin).magnitude;
        }

        switch (a_perspective)
        {
            case ModelViewOptionsPanel.Perspectives.Front:
                l_cameraPosition = l_origin + new Vector3(0f, 0f, -l_distance);
                break;
            case ModelViewOptionsPanel.Perspectives.Right:
                l_cameraPosition = l_origin + new Vector3(l_distance, 0f, 0f);
                break;
            case ModelViewOptionsPanel.Perspectives.Left:
                l_cameraPosition = l_origin + new Vector3(-l_distance, 0f, 0f);
                break;
            case ModelViewOptionsPanel.Perspectives.Top:
                break;
            case ModelViewOptionsPanel.Perspectives.Rear:
                break;
            default:
                viewCamera.transform.position = Vector3.zero;
                viewCamera.transform.forward = new Vector3(0f, 0f, 1f);
                return;
        }

        Vector3 l_lookDir = l_origin - l_cameraPosition;
        if(l_lookDir == Vector3.zero)
        {
            switch (a_perspective)
            {
                case ModelViewOptionsPanel.Perspectives.Front:
                    l_lookDir = new Vector3(0f, 0f, 1f);
                    break;
                case ModelViewOptionsPanel.Perspectives.Right:
                    l_lookDir = new Vector3(-1f, 0f, 0f);
                    break;
                case ModelViewOptionsPanel.Perspectives.Left:
                    l_lookDir = new Vector3(1f, 0f, 0f);
                    break;
                case ModelViewOptionsPanel.Perspectives.Top:
                    break;
                case ModelViewOptionsPanel.Perspectives.Rear:
                    break;
                default:
                    l_lookDir = new Vector3(0f, 0f, 1f);
                    break;
            }
        }
        viewCamera.transform.position = l_cameraPosition;        
        viewCamera.transform.forward = l_lookDir;
    }

    public void OnViewPanelClicked()
    {
        SelectAndActivateOptions();
    }

    public void SetActiveAndSelect(bool a_isActive)
    {
        gameObject.SetActive(a_isActive);
        viewCamera.gameObject.SetActive(a_isActive);
        if (angleLables != null) angleLables.SetActive(a_isActive);
        if (a_isActive)
        {
            SelectAndActivateOptions();
        }
        else
        {
            i_selectedViews.Remove(this);
            if (OptionsPanel != null)
            {
                OptionsPanel.SetActive(false);
            }
            foreach (var l_view in s_views)
            {
                if (l_view.isActiveAndEnabled)
                {
                    if (l_view.gameObject.tag != "KinectView")
                    {
                        l_view.SelectAndActivateOptions();
                        break;
                    }
                }
            }
        }
    }

    public void SelectAndActivateOptions()
    {
        foreach (var view in i_selectedViews) { view.OptionsPanel.SetActive(false);  view.viewLablePanelImg.color = inactiveColor; }
        i_selectedViews.Clear();

        if(OptionsPanel != null)
        {
            OptionsPanel.SetActive(true);
        }
        
        i_selectedViews.Add(this);
        if (viewLablePanelImg != null)
        {
            viewLablePanelImg.color = activeColor;
        }
    }

    public void OnPointerEnter()
    {
        print("pointer entered");
    }
}
