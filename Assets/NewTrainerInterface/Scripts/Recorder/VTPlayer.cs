using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using Windows.Kinect;


public class VTPlayer : MonoBehaviour {

    public VideoCutSlider videoCutSlider;
    public VideoCutSlider videoCutSlider2;
    public bool useOffset = true;

    int currFrame = 0;
    bool isPlaying = false;
    private Frame[] skeletonFrames;
    float timer = 0f;

    [System.Serializable]
    public class FrameChanged : UnityEvent<Frame> { }
    [SerializeField]
    public FrameChanged onFrameChanged;


    private BodySourceManager i_bodySourceManager;
    // Use this for initialization
    void Start () {
        i_bodySourceManager = FindObjectOfType<BodySourceManager>();
    }
	
	// Update is called once per frame
	void Update () {
        if(!isPlaying && skeletonFrames == null)
        {
            if(i_bodySourceManager != null)
            {
                Body l_body = i_bodySourceManager.firstTrackedBody;
                if(l_body != null)
                {
                    Frame l_newFrame = new Frame(i_bodySourceManager.tiltRadians, l_body, Time.deltaTime);
                    onFrameChanged.Invoke(l_newFrame);
                }
            }
        }


        if (isPlaying)
        {
            if (skeletonFrames.Length < 1) return;
            timer += Time.deltaTime;
            int temp = currFrame;
            float dt = skeletonFrames[currFrame + 1].deltaTime;
            while (timer > dt)
            {
                if (timer > dt)
                {
                    currFrame++;
                    if (useOffset)
                    {
                        currFrame = currFrame % (skeletonFrames.Length - 1);
                    }
                    else
                    {
                        if (currFrame > videoCutSlider.end - 2)
                            currFrame = videoCutSlider.start;
                    }
                    timer -= dt;
                }
                dt = skeletonFrames[currFrame + 1].deltaTime;
            }

            if (temp != currFrame)
            {
                if (videoCutSlider2 != null) videoCutSlider2.SetCurrentWihoutEvent(currFrame);
                if (videoCutSlider != null)
                    videoCutSlider.SetCurrentWihoutEvent(currFrame);
                onFrameChanged.Invoke(skeletonFrames[currFrame]);
                //rpm.SetFrame(skeletonFrames[currFrame]);
            }
        }
    }

    public void StartPlaying()
    {
        isPlaying = true;
        if(currFrame > 0)
        {
            if(currFrame < videoCutSlider.start || currFrame > videoCutSlider.end)
            {
                currFrame = videoCutSlider.start;
            }
        }
        /*
        if (!useOffset)
            currFrame = videoCutSlider.start;
        else
            currFrame = 0;
            */
    }

    public void StartPlaying(VideoCutSlider vcs)
    {
        if (skeletonFrames == null) return;
        isPlaying = true;
        if (currFrame > 0)
        {
            if (currFrame < vcs.start || currFrame > vcs.end)
            {
                currFrame = vcs.start;
            }
        }
        /*
        if (!useOffset)
            currFrame = videoCutSlider.start;
        else
            currFrame = 0;
            */
    }

    public void SetCurrFrame(VideoCutSlider vcs)
    {
        this.SetCurrFrameWithInvoke(vcs.current);
    }

    public void StopPlaying()
    {
        isPlaying = false;
        currFrame = 0;
    }

    public void Pause()
    {
        isPlaying = false;
    }

    public void UseOffset(bool flag)
    {
        useOffset = flag;
    }

    public void SetClip(VTRecorder recordingController)
    {
        skeletonFrames = recordingController.GetRecordedFrames();
    }

    public void Load(string a_recordsPath)
    {
        skeletonFrames = null;
        this.SetCurrFrame(0);
        FileStream input = new FileStream(Application.dataPath + "/../" + a_recordsPath, FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        skeletonFrames = (Frame[])bf.Deserialize(input);
        input.Close();
        timer = 0f;
        if (videoCutSlider != null)
        {
            if (skeletonFrames.Length > 1)
            {
                videoCutSlider.end = skeletonFrames.Length - 2;
                if (videoCutSlider2 != null) { videoCutSlider2.end = skeletonFrames.Length - 2; videoCutSlider2.SetRange(0, skeletonFrames.Length - 1); }
            }
        }
        //offsetUsed = false;
    }

    public void SetCurrFrame(int num)
    {
        currFrame = num;
        timer = 0;
    }

    public void SetCurrFrameWithInvoke(int num)
    {
        currFrame = num;
        timer = 0;
        onFrameChanged.Invoke(skeletonFrames[currFrame]);
    }

    public void OnRecordListPopulated(List<FolderListNode> a_childrenNodes)
    {
        foreach(var child in a_childrenNodes)
        {
            child.SubscribePlayer(this);
        }
    }


    private FolderListNode i_curNode;
    public void OnPlayerButtonPressed(string a_action, FolderListNode a_listNode)
    {
        if(a_action == "play")
        {
            Load(a_listNode.filePath);
            StartPlaying();
            if (i_curNode != a_listNode)
            {
                if(i_curNode != null)
                {
                    i_curNode.playButton.GetComponent<Image>().sprite = a_listNode.playButtonPic;
                }
                i_curNode = a_listNode;
            }
        }
        else if(a_action == "stop")
        {
            StopPlaying();
        }
    }
}
