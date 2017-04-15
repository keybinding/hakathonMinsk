using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class VideoCutSlider : MonoBehaviour {
    public int max = 2;
    public int min = 0;
    public int start = 0;
    public int current = 1;
    public int end = 2;
    public SliderPointer startP;
    public SliderPointer currentP;
    public SliderPointer endP;
    public UnityEvent OnEndPointerChange;
    public UnityEvent OnStartPointerChange;
    public UnityEvent OnCurrentPointerChange;

    void Start () {
        startP = transform.FindChild("Start").GetComponent<SliderPointer>();
        endP = transform.FindChild("End").GetComponent<SliderPointer>();
        currentP = transform.FindChild("Current").GetComponent<SliderPointer>();
	}

    public void SetRange(int min_, int max_)
    {
        min = min_;
        max = max_;
        start = min_;
        end = max_;
        current = min_;

        startP.SetCurrent(min_);
        endP.SetCurrent(max_);
        if (startP!=null)
        startP.SetPosition();
        if (endP != null)
        endP.SetPosition();
        if (currentP != null)
        currentP.SetPosition();
    }

    public void SetEnd(int end_)
    {
        end = end_;
        endP.SetCurrent(end_);
        OnEndPointerChange.Invoke();
    }

    public void SetStart(int start_)
    {
        start = start_;
        startP.SetCurrent(start_);
        OnStartPointerChange.Invoke();
    }

    public void SetCurrent(int current_)
    {
        current = current_;
        currentP.SetCurrent(current);
        OnCurrentPointerChange.Invoke();
    }

    public void SetCurrentWihoutEvent(int current_)
    {
        current = current_;
        currentP.SetCurrent(current);
    }
}
