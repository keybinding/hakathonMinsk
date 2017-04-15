using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine.UI;

public class PointManPlayer : MonoBehaviour {
    float playBackSpeed = 0.0333f;
    float timer = 0f;
    bool isPlaying;
    RecordedPointMan rpm;
    private Frame[] skeletonFrames;
    public KinectPointController kinectPointController;
    Vector3 offset = Vector3.zero;
    Vector3 startPosition;
    int currFrame = 0;
    public bool useOffset = true;
    public VideoCutSlider videoCutSlider;
    KinectPointController kpc;
    BodySourceManager bodySourceManager;    
    public Toggle playstoptoggle;
    public float playingStartTimer = -1;
    String name;
    public Text playingStartTimerText;
    bool offsetUsed;
    string i_recordName;
    public void Load(string name)
    {
        skeletonFrames = null;
        this.SetCurrFrame(0);
        i_recordName = name;
        FileStream input = new FileStream(Application.dataPath + "/../Records/" + name, FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        skeletonFrames = (Frame[])bf.Deserialize(input);
        input.Close();
        timer = 0f;
        offsetUsed = false;
    }

    public void StartCountDown(String name_)
    {
        playingStartTimer = 10f;
        playingStartTimerText.text = ((int)(playingStartTimer) + 1).ToString();
        this.name = name_;
        Load(name_);
        isPlaying = false;
        //skeletonFrames = null;
    }

    public void Start()
    {
        bodySourceManager = GameObject.FindObjectOfType<BodySourceManager>();
        startPosition = transform.position;
        rpm = this.GetComponent<RecordedPointMan>();
        kpc = GameObject.FindObjectOfType<KinectPointController>();

    }

    public void StartPlaying()
    {
        isPlaying = true;
        if (!useOffset)
            currFrame = videoCutSlider.start;
        else 
            currFrame = 0;
    }

    public void StopPlaying()
    {
        isPlaying = false;
        currFrame = 0;
        skeletonFrames = null;
        RecordingController.Instance.StopRecording();
        RecordingController.Instance.SaveRecord(i_recordName);
        Debug.Log("StopPlaying");
    }

    public void StopAndRemoveClip()
    {
        isPlaying = false;
        currFrame = 0;
        skeletonFrames = null;
    }

    public void UseOffset()
    {
        float currentAngle = (float)bodySourceManager.tiltRadians;
        float cos = Mathf.Cos(currentAngle);
        float sin = Mathf.Sin(currentAngle);
        Vector3 pos = rpm.isUsingWheelChair? kinectPointController.SpineBasePosition
                      : kinectPointController.LegsPosition;
        float newY = pos.y * cos + pos.z * sin;
        float newZ = pos.z * cos - pos.y * sin;

        pos.y = newY;
        pos.z = newZ;
        Vector3 legsPosition = rpm.isUsingWheelChair ? skeletonFrames[currFrame].GetPositionInVector3(0) 
                               : (skeletonFrames[currFrame].GetPositionInVector3(15) + skeletonFrames[currFrame].GetPositionInVector3(19)) / 2f;
        offset = (legsPosition - pos);
        //offset.x = 0f;
        float offsetY = offset.y * cos - offset.z * sin;
        float offsetZ = offset.z * cos - offset.y * sin;
        offset.y = offsetY;
        offset.z = offsetZ;
        rpm.offset = offset;
    }

	void Update () {
        if (playingStartTimer <= 0)
        {
            playingStartTimerText.text = "";
            if (skeletonFrames != null && offsetUsed == true)
            {
                isPlaying = true;
                offsetUsed = false;
                ChangeToggleToStop();
                ActivateStopPlayButton();
                RecordingController.Instance.StartRecording();
            }
        }
        else
        {
            playingStartTimer -= Time.deltaTime;
            playingStartTimerText.text = ((int)(playingStartTimer) + 1).ToString();
            UseOffset();
            offsetUsed = true;
        }

        if (!isPlaying && skeletonFrames == null)
            rpm.SetFrame(null);

        if (useOffset && skeletonFrames != null)
        {
            
        }

        if (isPlaying) {
            timer += Time.deltaTime;
            int temp = currFrame;
            float dt = skeletonFrames[currFrame+1].deltaTime;
            while (timer > dt)
            {
                if (timer > dt)
                {
                    currFrame++;
                    if (useOffset)
                    {
                        currFrame = currFrame % (skeletonFrames.Length - 1);
                    }
                    else {
                        if (currFrame > videoCutSlider.end-2)
                            currFrame = videoCutSlider.start;
                    }
                    timer -= dt;
                }
                dt = skeletonFrames[currFrame+1].deltaTime;
            }

            if (temp != currFrame) {
                if (videoCutSlider != null)
                    videoCutSlider.SetCurrentWihoutEvent(currFrame);
                rpm.SetFrame(skeletonFrames[currFrame]);
            }
        }
	}

    public void SetClip(RecordingController recordingController)
    {
        skeletonFrames = recordingController.GetRecordedFrames();
    }

    public void SetCurrFrame(int num)
    {
        this.currFrame = num;
        timer = 0;
    }

    public void SetCurrFrame(VideoCutSlider vcs)
    {
        this.SetCurrFrame(vcs.current);
    }

    public void UseOffset(bool flag)
    {
        useOffset = flag;
    }

    public void StopPlayButton(UnityEngine.UI.Toggle toggle)
    {
        if (toggle.isOn)
            StartPlaying();
        else
            StopPlaying();
    }

    public void ChangeToggleToStop()
    {
        if (playstoptoggle == null)
            playstoptoggle = GameObject.Find("PlayStopButton").GetComponent<Toggle>();
        playstoptoggle.isOn = true;
    }

    public void ActivateStopPlayButton()
    {
        if (playstoptoggle.gameObject.activeSelf == false)
            playstoptoggle.gameObject.SetActive(true);
    }
}
