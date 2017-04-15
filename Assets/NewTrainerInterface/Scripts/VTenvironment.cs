using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VTenvironment : MonoBehaviour {

    public static VTenvironment Instance { get { return i_instance; } }

    private static VTenvironment i_instance = null;

    public string userName { get { return i_userName; } }

    public bool isOffline { get { return i_isOffline; } }

    public string password { get { return i_password; } }

    private string i_userName = "";
    private string i_password = "";
    private bool i_isOffline = true;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        i_instance = this;
    }

    public void OnLogin(string a_username, string a_password)
    {
        i_userName = a_username;
        i_password = a_password;
        i_isOffline = false;
    }

    public void OnWorkOffline()
    {
        i_isOffline = true;
    }
}
