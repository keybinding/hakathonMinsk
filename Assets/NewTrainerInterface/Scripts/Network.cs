using UnityEngine;
using System.Net;
using System.Text;
using System.IO;
using System.Collections;
using System.Diagnostics;

public class Network : MonoBehaviour {
    public static bool isOffLine = false;

    VTenvironment i_environment = null;
    private static string[] i_exerciseData = null;
	// Use this for initialization
    /*
	IEnumerator Start () {
        GameObject go = GameObject.FindGameObjectWithTag("Environment");
        if (go != null)
        {
            VTenvironment i_environment = go.GetComponent<VTenvironment>();
            WWW exerciseData = new WWW((isOffLine ? "http://localhost/?name=" : "http://194.87.93.103/?name=") + i_environment.userName);
            yield return exerciseData;
            i_exerciseData = exerciseData.text.Split(';');
        }
	}
    */

    public static string[] PopExerciseData()
    {
        string[] l_result = null;
        if(i_exerciseData != null)
        {
            int l_resLength = i_exerciseData.Length;
            if (i_exerciseData.Length > 2) l_resLength -= 1;
            l_result = new string[l_resLength];
            for (int i = 0; i != l_resLength; ++i) l_result[i] = i_exerciseData[i];
        }
        else
        {
            l_result = new string[0];
        }
        return l_result;
    }

    public static void RunDefaultBrowser()
    {
        Process.Start("http://194.87.93.103/");
    }

    public static IEnumerator TodayExercisesRqst()
    {
        GameObject l_environmentGO = GameObject.FindGameObjectWithTag("Environment");
        if (l_environmentGO != null)
        {
            VTenvironment l_env = l_environmentGO.GetComponent<VTenvironment>();
            if(l_env != null)
            {
                WWW l_exerciseRqst = new WWW(isOffLine ? "http://localhost/?name=" : "http://194.87.93.103/exersise?name=" + l_env.userName);
                
                yield return l_exerciseRqst;
                /*
                int l_bodyPos = -1;
                l_bodyPos = l_exerciseRqst.text.IndexOf("<body>") + 6;
                int l_bodyPosEnd = l_exerciseRqst.text.IndexOf("</body>");
                string l_respText = l_exerciseRqst.text.Substring(l_bodyPos, l_bodyPosEnd - l_bodyPos);*/
                if (l_exerciseRqst.text.Length > 0) {
                    if (l_exerciseRqst.text[0] != '!')
                    {
                        i_exerciseData = l_exerciseRqst.text.Split(';');
                    }
                }
            }
            else
            {
                UnityEngine.Debug.Log("!!!Environment component not found!!!");
            }
        }
        else
        {
            UnityEngine.Debug.Log("!!!Environment gameobject not found!!!");
        }
    }

    public static void DownloadFileViaFTP(string a_directory, string a_ftpDirectory, string a_fileName)
    {
        if (VTenvironment.Instance != null)
        {
            if (!VTenvironment.Instance.isOffline)
            {
                FtpWebRequest l_request = (FtpWebRequest)WebRequest.Create("ftp://194.87.93.103/" + a_ftpDirectory + "/" + a_fileName);
                l_request.Method = WebRequestMethods.Ftp.DownloadFile;
                l_request.Credentials = new NetworkCredential(VTenvironment.Instance.userName, VTenvironment.Instance.password);
                FtpWebResponse l_response = null;
                StreamReader l_reader = null;
                StreamWriter l_writer = null;
                try
                {
                    l_response = (FtpWebResponse)l_request.GetResponse();
                    Stream l_responseStream = l_response.GetResponseStream();
                    l_reader = new StreamReader(l_responseStream);
                    l_writer = new StreamWriter(a_directory + "/" + a_fileName);
                    l_writer.Write(l_reader.ReadToEnd());
                    l_reader.Close();
                    l_writer.Close();
                    l_reader.Dispose();
                    l_writer.Dispose();
                    l_response.Close();
                    ((System.IDisposable)l_response).Dispose();
                }
                catch (System.Exception e)
                {
                    UnityEngine.Debug.Log(e.Message);
                }
                finally
                {
                    if (l_response != null)
                    {
                        UnityEngine.Debug.Log(l_response.StatusDescription);
                        l_response.Close();
                        ((System.IDisposable)l_response).Dispose();
                    }
                    if (l_reader != null) { l_reader.Close(); l_reader.Dispose(); }
                    if (l_writer != null) { l_writer.Close(); l_writer.Dispose(); }
                }
            }
        }
    }

    public static void UploadFileViaFTP(string a_fileName, string a_localFilePath)
    {
        if (VTenvironment.Instance != null)
        {
            if (!VTenvironment.Instance.isOffline)
            {
                FtpWebRequest l_rqst = (FtpWebRequest)WebRequest.Create("ftp://194.87.93.103:21/" + a_fileName);
                l_rqst.Method = WebRequestMethods.Ftp.UploadFile;
                l_rqst.Credentials = new NetworkCredential(VTenvironment.Instance.userName, VTenvironment.Instance.password);

                Stream l_rqstStream = l_rqst.GetRequestStream();
                FileStream l_file = new FileStream(a_localFilePath, FileMode.Open, FileAccess.Read);
                BinaryReader l_bsr = new BinaryReader(l_file);
                byte[] buffer = new byte[l_file.Length];
                l_bsr.Read(buffer, 0, buffer.Length);
                l_rqstStream.Write(buffer, 0, buffer.Length);
                l_rqstStream.Close();

                {
                    FtpWebResponse l_resp = null;
                    try
                    {
                        l_resp = (FtpWebResponse)l_rqst.GetResponse();
                    }
                    catch (System.Exception e)
                    {
                        UnityEngine.Debug.Log(e.Message);
                        return;
                    }
                    finally
                    {
                        if (l_resp != null)
                        {
                            UnityEngine.Debug.Log(l_resp.StatusDescription);
                            l_resp.Close();
                            ((System.IDisposable)l_resp).Dispose();
                        }
                    }
                }
            }
        }
    }
}
