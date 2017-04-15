using UnityEngine;
using System.Collections.Generic;
using System;
using Windows.Kinect;

public class audioTestManager : MonoBehaviour {

    KinectSensor i_sensor = null;
    AudioBeamFrameReader i_reader = null;
    List<AudioBeamFrame> i_fl = null;

	// Use this for initialization
	void Start () {
        i_sensor = KinectSensor.GetDefault();
        if (i_sensor != null)
        {
            if (!i_sensor.IsOpen)
            {
                i_sensor.Open();
            }
            Windows.Kinect.AudioSource l_as = i_sensor.AudioSource;
            i_reader = l_as.OpenReader();
            //l_as.AudioBeams[0].AudioBeamMode = AudioBeamMode.Manual;
            //l_as.AudioBeams[0].BeamAngle = 0;
        }
	}

    int i = 0;
	// Update is called once per frame
	void Update () {
        var l_frames = i_reader.AcquireLatestBeamFrames();
        print(l_frames.Count);
        if (l_frames != null)
        {
            //using (l_frames)
            {
                foreach (AudioBeamFrame l_frame in l_frames)
                {
                    if (l_frame == null)
                    {
                        print("null");
                        continue;
                    }
                    print(l_frame.SubFrames.Count);
                    foreach (var l_subframe in l_frame.SubFrames)
                    {
                        l_subframe.Dispose();
                    }
                    l_frame.Dispose();
                }
            }
        }
        //((IDisposable)l_frames).Dispose();
        ++i;
	}

    void OnApplicationQuit()
    {
        print("quit");
        if (i_reader != null)
        {
            i_reader.Dispose();
            i_reader = null;
        }

        if (i_sensor != null)
        {
            if (i_sensor.IsOpen) { i_sensor.Close(); i_sensor = null; }
        }
    }
}
