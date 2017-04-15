using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System;

public class PanelWithVerticalSplitter : MonoBehaviour {
    public float splitterWidth = 5.0f;
    public RectTransform splitter = null;
    public RectTransform leftPanel = null;
    public RectTransform rightPanel = null;

    [System.Serializable()]
    public class PanelResized : UnityEvent<RectTransform> { }
    [SerializeField]
    public PanelResized OnLeftPanelResize;
    [SerializeField]
    public PanelResized OnRightPanelResize;


    // Use this for initialization
    void Start () {
        splitter.offsetMax = new Vector2(splitter.offsetMin.x + splitterWidth, splitter.offsetMax.y);
        if(gameObject.tag == "ViewsPanel")
        {
            MoveSplitterToCenter();
        }
        AllignPanelsWithSplitter();
    }

    public void AllignPanelsWithSplitter()
    {
        leftPanel.offsetMax = new Vector2(splitter.offsetMin.x, leftPanel.offsetMax.y);
        OnLeftPanelResize.Invoke((RectTransform)leftPanel.transform);
        rightPanel.offsetMin = new Vector3(splitter.offsetMax.x, rightPanel.offsetMin.y);
        OnRightPanelResize.Invoke((RectTransform)rightPanel.transform);
    }

    public void OnSplitterDrag(BaseEventData a_data)
    {
        PointerEventData l_data = (PointerEventData) a_data;
        splitter.offsetMin = new Vector2(splitter.offsetMin.x + l_data.delta.x, splitter.offsetMin.y);
        splitter.offsetMax = new Vector2(splitter.offsetMax.x + l_data.delta.x, splitter.offsetMax.y);
        AllignPanelsWithSplitter();
    }

    public void MoveSplitterToCenter()
    {
        splitter.position = leftPanel.position;
        splitter.position = new Vector3(splitter.position.x + ((RectTransform)transform).rect.width / 2.0f, splitter.position.y, splitter.position.z);
        AllignPanelsWithSplitter();
    }

    public void OnSplitterDrop(BaseEventData a_data)
    {
    }

    public void OnPointerEnterSplitter(BaseEventData a_data)
    {
    }

    public void OnPointerExitSplitter(BaseEventData a_data)
    {
        PointerEventData l_data = (PointerEventData)a_data;
        if (!l_data.dragging)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    public void OnParentPanelResize(RectTransform a_parent)
    {
        AllignPanelsWithSplitter();
    }

    
}
