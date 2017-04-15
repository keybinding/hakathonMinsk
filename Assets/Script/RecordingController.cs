using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Windows.Kinect;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Net;
using System;

public class RecordingController : MonoBehaviour {
    private static RecordingController instance_;
    public static RecordingController Instance
    {
        get {
            if (instance_ == null)
                instance_ = GameObject.FindObjectOfType<RecordingController>();
            return instance_;
        }
    }

    public List<Frame> recordedClip;
    bool recording = false;
    public VideoCutSlider vcs;
    public RecordedPointMan rpm;
    bool play = false;
    private BodySourceManager bodySourceManager;
    public void Start()
    {
        bodySourceManager = GameObject.FindObjectOfType<BodySourceManager>();
    }

    public GameObject quitDialog;
	void Update () {
        if (Input.GetKey(KeyCode.Escape))
            quitDialog.SetActive(true);

        if (recording)
        {
            Body body = GetTrackedBody();
            if (body!=null)
            {
                recordedClip.Add(new Frame(bodySourceManager.tiltRadians, body, Time.deltaTime));
                vcs.SetRange(0, recordedClip.Count - 1);
            }
           
        }
	}

    public void Quit()
    {
        Application.Quit();
    }

    public Body GetTrackedBody()
    {
        Body[] bodies = bodySourceManager.GetData();
        if (bodies == null)
            return null;

        foreach (Body b in bodySourceManager.GetData())
        {
            if (b.IsTracked)
                return b;
        }
        return null;
    }

    public void SetFrame()
    {
        rpm.SetFrame((int)vcs.current);
    }
    public void StartRecording()
    {
        recordedClip = new List<Frame>();
        recording = true;
        rpm.player = -1;
    }

    public void StopRecording()
    {
        recording = false;
    }

    public Frame[] GetRecordedFrames()
    {        
        return recordedClip.ToArray();
    }

    public void SaveRecord(string name)
    {
        
        string l_userName = "";
        l_userName = VTenvironment.Instance.userName;
        if (!Directory.Exists(Application.dataPath + "/../Records/" + l_userName)) Directory.CreateDirectory(Application.dataPath + "/../Records/" + l_userName);
        
        string l_reportFileName = name + "_" + DateTime.Now.ToString("MM-dd-yyyy_HH-mm") + ".rpt";
        string l_recordFileName = name + "_" + DateTime.Now.ToString("MM-dd-yyyy_HH-mm") + ".nrb";

        string filePath = Application.dataPath + "/../Records/" + l_userName + "/" + l_recordFileName;
        FileStream output = new FileStream(filePath, FileMode.Create);
        BinaryFormatter bf = new BinaryFormatter();
        Frame[] data = new Frame[vcs.end - vcs.start];
        for (int ii = (int)vcs.start; ii < (int)vcs.end; ii++)
            data[ii - vcs.start] = recordedClip[ii];
        bf.Serialize(output, data);
        output.Close();
        if (VTenvironment.Instance != null)
        {
            if (!VTenvironment.Instance.isOffline)
            {
                SaveReport(l_reportFileName, data);
            }
        }
        Debug.Log("Saved");
    }

    void SaveReport(string name, Frame[] data)
    {
        string reportFilePath = Application.dataPath + "/../Records/" + VTenvironment.Instance.userName + "/" + name;
        FileStream output = new FileStream(reportFilePath, FileMode.Create);
        float[] l_reportData = new float[data.Length * 20 * 3]; //20 joints 3 coords
        int l_idx = 0;
        foreach (var l_frame in data)
        {
            SaveReportNode(l_reportData, JointType.AnkleLeft, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.AnkleRight, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.ElbowLeft, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.ElbowRight, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.FootLeft, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.FootRight, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.HandLeft, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.HandRight, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.Head, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.SpineBase, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.HipLeft, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.HipRight, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.KneeLeft, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.KneeRight, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.SpineShoulder, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.ShoulderLeft, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.ShoulderRight, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.SpineMid, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.WristLeft, l_idx, l_frame);
            l_idx += 3;
            SaveReportNode(l_reportData, JointType.WristRight, l_idx, l_frame);
            l_idx += 3;
        }
        byte[] l_byteArr = new byte[l_reportData.Length * sizeof(float)];
        Buffer.BlockCopy(l_reportData, 0, l_byteArr, 0, l_reportData.Length);
        output.Write(l_byteArr, 0, l_byteArr.Length);
        output.Close();
        //send data to server if online
        if (VTenvironment.Instance != null)
        {
            if (!VTenvironment.Instance.isOffline)
            {
                FtpWebRequest l_rqst = (FtpWebRequest)WebRequest.Create("ftp://194.87.93.103:21/" + name);
                l_rqst.Method = WebRequestMethods.Ftp.UploadFile;
                l_rqst.Credentials = new NetworkCredential(VTenvironment.Instance.userName, VTenvironment.Instance.password);

                Stream l_rqstStream = l_rqst.GetRequestStream();
                l_rqstStream.Write(l_byteArr, 0, l_byteArr.Length);
                l_rqstStream.Close();

                {
                    FtpWebResponse l_resp = null;
                    try
                    {
                        l_resp = (FtpWebResponse)l_rqst.GetResponse();
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log(e.Message);
                        return;
                    }
                    finally
                    {
                        if (l_resp != null)
                        {
                            Debug.Log(l_resp.StatusDescription);
                            l_resp.Close();
                            ((System.IDisposable)l_resp).Dispose();
                        }
                    }
                }
            }
        }
    }

    void SaveReportNode(float[] a_reportData, JointType a_joint, int a_idx, Frame a_frame)
    {
        int l_jointIdx = (int)a_joint;
        a_reportData[a_idx] = a_frame.bones[l_jointIdx].x;
        a_reportData[a_idx + 1] = a_frame.bones[l_jointIdx].y;
        a_reportData[a_idx + 2] = a_frame.bones[l_jointIdx].z;
    }

    public void SaveRecord(Text t)
    {
        SaveRecord(t.text);
    }
}
