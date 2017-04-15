using UnityEngine;
using System.Collections;
using System;

public class PlayerChecker : MonoBehaviour {

    public bool isTrainer = false;
    public GameObject p0;
    public GameObject p1;
	void Start () {
        if (Environment.GetCommandLineArgs().Length > 1)
        {
            isTrainer = false;
            foreach (string s in Environment.GetCommandLineArgs())
            {
                if (s.Equals("/trainer"))
                {
                    isTrainer = false;
                }
            }
        }
        if (isTrainer)
            p0.SetActive(true);
        else
            p1.SetActive(true);
	}

}
