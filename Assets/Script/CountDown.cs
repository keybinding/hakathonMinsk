using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class CountDown : MonoBehaviour {
    public UnityEvent OnCountdownEnds;
    float timer = -1;
    Text text;
    public void Start()
    {
        text = GetComponent<Text>();
        text.text = "";
    }

	void Update () {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            text.text = ((int)(timer)+1).ToString();
            if (timer <= 0)
            {
                OnCountdownEnds.Invoke();
                text.text = "";
            }
        }
	}

    public void StartTimer(float time)
    {
        timer = time;
    }
}
