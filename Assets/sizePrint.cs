using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class sizePrint : MonoBehaviour {

    [System.Serializable()]
    public class PanelResized : UnityEvent<RectTransform> { }
    [SerializeField]
    public PanelResized OnResizeFinished;

    private RectTransform i_rectTr;
    private Rect i_currentRect;

    private bool i_isResizeStarted = false;

    void Start()
    {
        i_rectTr = GetComponent<RectTransform>();
        i_currentRect = i_rectTr.rect;
    }

	void Update()
    {
        if(i_currentRect.width != i_rectTr.rect.width || i_currentRect.height != i_rectTr.rect.height)
        {
            if (!i_isResizeStarted) i_isResizeStarted = true;
        }
        else
        {
            if (i_isResizeStarted)
            {
                i_isResizeStarted = false;
                OnResizeFinished.Invoke(i_rectTr);
            }
        }
        i_currentRect = i_rectTr.rect;
    }
}
