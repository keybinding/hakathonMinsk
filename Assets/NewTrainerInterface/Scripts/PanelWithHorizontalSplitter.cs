using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class PanelWithHorizontalSplitter : MonoBehaviour {

    public float splitterWidth = 5.0f;
    public RectTransform splitter = null;
    public RectTransform bottomPanel = null;
    public RectTransform topPanel = null;

    [SerializeField]
    public PanelWithVerticalSplitter.PanelResized OnBottomPanelResize;
    [SerializeField]
    public PanelWithVerticalSplitter.PanelResized OnTopPanelResize;
    // Use this for initialization
    void Start()
    {
        splitter.offsetMax = new Vector2(splitter.offsetMax.x, splitter.offsetMin.y + splitterWidth);
        AllignPanelsWithSplitter();
    }

    public void OnSplitterDrag(BaseEventData a_data)
    {
        PointerEventData l_data = (PointerEventData)a_data;
        splitter.offsetMin = new Vector2(splitter.offsetMin.x, splitter.offsetMin.y + l_data.delta.y);
        splitter.offsetMax = new Vector2(splitter.offsetMax.x, splitter.offsetMax.y + l_data.delta.y);
        AllignPanelsWithSplitter();
    }

    public void AllignPanelsWithSplitter()
    {
        bottomPanel.offsetMax = new Vector2(bottomPanel.offsetMax.x, splitter.offsetMin.y);
        OnBottomPanelResize.Invoke((RectTransform)bottomPanel.transform);
        topPanel.offsetMin = new Vector3(topPanel.offsetMin.x, splitter.offsetMax.y);
        OnTopPanelResize.Invoke((RectTransform)topPanel.transform);
    }

    public void OnSplitterDrop(BaseEventData a_data)
    {
    }

    public void OnPointerEnterSplitter(BaseEventData a_data)
    {

    }

    public void MoveSplitterToCenter()
    {
        splitter.position = bottomPanel.position;
        splitter.position = new Vector3(splitter.position.x, splitter.position.y + ((RectTransform)transform).rect.height / 2.0f, splitter.position.z);
        AllignPanelsWithSplitter();
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
