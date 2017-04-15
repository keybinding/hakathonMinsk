using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Net;
using System.IO;
using System;

public class LoginUI : MonoBehaviour {

    public GameObject errorMessage = null;

    public InputField userName = null;

    public InputField password = null;

    public void OnLoginClick() { if (userName != null && password != null) OnLoginClick(userName, password); }

    public void OnLoginClick(InputField a_userName, InputField a_password)
    {
        if (!ValidateInputs(a_userName.text, a_password.text)) { RiseError("Some fields are empty!"); return; }

        //get id of email

        FtpWebRequest l_webRqst = (FtpWebRequest) WebRequest.Create("ftp://194.87.93.103:21/");
        l_webRqst.Method = WebRequestMethods.Ftp.PrintWorkingDirectory;
        l_webRqst.Credentials = new NetworkCredential(a_userName.text, a_password.text);

        {
            FtpWebResponse l_resp = null;
            try
            {
                l_resp = (FtpWebResponse)l_webRqst.GetResponse();

                l_resp.Close();
                FtpWebRequest l_webRqstDir = (FtpWebRequest)WebRequest.Create("ftp://194.87.93.103:21/");
                l_webRqstDir.Method = WebRequestMethods.Ftp.ListDirectory;
                l_webRqstDir.Credentials = new NetworkCredential(a_userName.text, a_password.text);
                {
                    FtpWebResponse l_respDir = null;
                    try
                    {
                        l_respDir = (FtpWebResponse)l_webRqstDir.GetResponse();
                        Stream l_respStream = l_respDir.GetResponseStream();
                        StreamReader l_sr = new StreamReader(l_respStream);
                        string l_content = l_sr.ReadToEnd();
                        l_sr.Close();
                        l_respStream.Close();
                        VTenvironment.Instance.OnLogin(a_userName.text, a_password.text);
                        if (l_content.IndexOf("pro\r\n") > -1) SceneManager.LoadScene(1);
                        else SceneManager.LoadScene(2);
                        
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log(e.Message);
                        RiseError("Login failed!");
                        return;
                    }
                    finally
                    {
                        if (l_resp != null)
                        {
                            l_respDir.Close();
                            ((IDisposable)l_respDir).Dispose();
                        }
                    }
                }
                
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                RiseError("Login failed!");
                return;
            }
            finally
            {
                if (l_resp != null)
                {
                    l_resp.Close();
                    ((IDisposable)l_resp).Dispose();
                }
            }
        }

    }

    public void OnWorkOfflineClicked()
    {
        VTenvironment.Instance.OnWorkOffline();
        SceneManager.LoadScene(1);
    }

    private void RiseError(string a_errorMsg)
    {
        if (errorMessage == null) return;
        errorMessage.GetComponent<Text>().text = a_errorMsg;
        errorMessage.SetActive(true);
    }

    private bool ValidateInputs(string a_userName, string a_password)
    {
        return a_userName != "" && a_password != "";
    }
}
