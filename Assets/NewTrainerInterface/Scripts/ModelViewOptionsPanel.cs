using UnityEngine;
using System.Collections;
using UnityEngine.Events;

[System.Serializable]
public class ModelViewOptionsPanel : MonoBehaviour {

    [System.Serializable]
	public enum Perspectives { Front = 1, Right, Left, Top, Rear, None = 0}

    [System.Serializable]
    public class PerspectiveSwitchedEvent : UnityEvent<Perspectives> { }

    [SerializeField]
    public PerspectiveSwitchedEvent OnPerspectiveSwitched;

    public void OnPerspectiveButtonPressed(int a_perspective)
    {
        OnPerspectiveSwitched.Invoke((Perspectives) a_perspective);
    }

    public void OnPerspectiveButtonPressed(Perspectives a_perspective)
    {
        OnPerspectiveSwitched.Invoke(a_perspective);
    }
}
