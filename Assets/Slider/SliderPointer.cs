using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

public class SliderPointer : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    RectTransform slider;
    VideoCutSlider vcs;
    float state = 0f;
    public Type type;
    float maxPos, minPos;
    RectTransform rTransform;
    RectTransform backgr;
    public UnityEvent OnPointerClick;
    public void Start()
    {
        //print(type.ToString());
        slider = this.transform.parent.GetComponent<RectTransform>();
        vcs = slider.GetComponent<VideoCutSlider>();
        rTransform = this.GetComponent<RectTransform>();
        backgr = transform.parent.FindChild("RangeBackGr").GetComponent<RectTransform>();
        int intPos = 0;
        switch(type)
        {
            case Type.Start:
                intPos = vcs.start; break;
            case Type.Current:
                intPos = vcs.current; break;
            case Type.End:
                intPos = vcs.end; break;
        }
        state = intPos;
        SetPosition();
    }
    float totalDrag = 0f;
    float dragStartState;
    public void OnDrag(PointerEventData eventData)
    {
        totalDrag += eventData.delta.x;
        state = dragStartState + ((float)(vcs.max - vcs.min))*totalDrag/(maxPos-minPos);
        switch (type) {
            case Type.Current:
                state = Mathf.Clamp(state, vcs.min, vcs.max);
                vcs.SetCurrent(Mathf.RoundToInt(state));
                break;
            case Type.Start:
                state = Mathf.Clamp(state, vcs.min, vcs.end);
                vcs.SetStart(Mathf.RoundToInt(state));
                break;
            case Type.End:
                state = Mathf.Clamp(state, vcs.start, vcs.max);
                vcs.SetEnd(Mathf.RoundToInt(state));
                break;
        }
        SetPosition();
    }

    public void SetPosition()
    {
        if (slider == null)
            return;
        minPos = slider.rect.xMin;
        maxPos = slider.rect.xMax;
        rTransform.anchoredPosition = new Vector2(Mathf.Round(state) * (maxPos - minPos) / (vcs.max - vcs.min) + minPos, rTransform.anchoredPosition.y);
        backgr.anchorMin = new Vector2((float)vcs.start/(float)(vcs.max - vcs.min), 0f);
        backgr.anchorMax = new Vector2((float)vcs.end / (float)(vcs.max - vcs.min), 1f);
        float y = backgr.sizeDelta.y;
        backgr.sizeDelta = new Vector2(0f, y);
    }

    public void SetCurrent(int m)
    {
        this.state = m;
        SetPosition();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerClick.Invoke();
        totalDrag = 0f;
        dragStartState = state;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        state = dragStartState + ((float)(vcs.max - vcs.min)) * totalDrag / (maxPos - minPos);
        totalDrag = 0f;
        
    }
}
public enum Type {
    Start, End, Current
}